using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Neo;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RpcPlugin
{
    public sealed class RpcServer : QueryServer, IDisposable
    {
        public RpcServer(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default) : base(system, wallet, maxGasInvoke) { }

        private static JObject CreateErrorResponse(JObject id, int code, string message, JObject data = null)
        {
            JObject response = CreateResponse(id);
            response["error"] = new JObject();
            response["error"]["code"] = code;
            response["error"]["message"] = message;
            if (data != null)
                response["error"]["data"] = data;
            return response;
        }

        private static JObject CreateResponse(JObject id)
        {
            JObject response = new JObject();
            response["jsonrpc"] = "2.0";
            response["id"] = id;
            return response;
        }

        private JObject GetInvokeResult(byte[] script, IVerifiable checkWitnessHashes = null)
        {
            ApplicationEngine engine = ApplicationEngine.Run(script, checkWitnessHashes, extraGAS: MaxGasInvoke);
            JObject json = new JObject();
            json["script"] = script.ToHexString();
            json["state"] = engine.State;
            json["gas_consumed"] = engine.GasConsumed.ToString();
            try
            {
                json["stack"] = new JArray(engine.ResultStack.Select(p => p.ToParameter().ToJson()));
            }
            catch (InvalidOperationException)
            {
                json["stack"] = "error: recursive reference";
            }
            return json;
        }

        private static JObject GetRelayResult(RelayResultReason reason, UInt256 hash)
        {
            switch (reason)
            {
                case RelayResultReason.Succeed:
                    {
                        var ret = new JObject();
                        ret["hash"] = hash.ToString();
                        return ret;
                    }
                case RelayResultReason.AlreadyExists:
                    throw new QueryException(-501, "Block or transaction already exists and cannot be sent repeatedly.");
                case RelayResultReason.OutOfMemory:
                    throw new QueryException(-502, "The memory pool is full and no more transactions can be sent.");
                case RelayResultReason.UnableToVerify:
                    throw new QueryException(-503, "The block cannot be validated.");
                case RelayResultReason.Invalid:
                    throw new QueryException(-504, "Block or transaction validation failed.");
                case RelayResultReason.PolicyFail:
                    throw new QueryException(-505, "One of the Policy filters failed.");
                default:
                    throw new QueryException(-500, "Unknown error.");
            }
        }

        private JObject Process(string method, JArray _params)
        {
            switch (method)
            {
                case "getbestblockhash":
                    {
                        return GetBestBlockHash();
                    }
                case "getblock":
                    {
                        JObject key = _params[0];
                        bool verbose = _params.Count >= 2 && _params[1].AsBoolean();
                        return GetBlock(key, verbose);
                    }
                case "getblockcount":
                    {
                        return GetBlockCount();
                    }
                case "getblockhash":
                    {
                        uint height = uint.Parse(_params[0].AsString());
                        return GetBlockHash(height);
                    }
                case "getblockheader":
                    {
                        JObject key = _params[0];
                        bool verbose = _params.Count >= 2 && _params[1].AsBoolean();
                        return GetBlockHeader(key, verbose);
                    }
                case "getblocksysfee":
                    {
                        uint height = uint.Parse(_params[0].AsString());
                        return GetBlockSysFee(height);
                    }
                case "getconnectioncount":
                    {
                        return GetConnectionCount();
                    }
                case "getcontractstate":
                    {
                        UInt160 script_hash = UInt160.Parse(_params[0].AsString());
                        return GetContractState(script_hash);
                    }
                case "getpeers":
                    {
                        return GetPeers();
                    }
                case "getrawmempool":
                    {
                        bool shouldGetUnverified = _params.Count >= 1 && _params[0].AsBoolean();
                        return GetRawMemPool(shouldGetUnverified);
                    }
                case "getrawtransaction":
                    {
                        UInt256 hash = UInt256.Parse(_params[0].AsString());
                        bool verbose = _params.Count >= 2 && _params[1].AsBoolean();
                        return GetRawTransaction(hash, verbose);
                    }
                case "getstorage":
                    {
                        UInt160 script_hash = UInt160.Parse(_params[0].AsString());
                        byte[] key = _params[1].AsString().HexToBytes();
                        return GetStorage(script_hash, key);
                    }
                case "gettransactionheight":
                    {
                        UInt256 hash = UInt256.Parse(_params[0].AsString());
                        return GetTransactionHeight(hash);
                    }
                case "getvalidators":
                    {
                        return GetValidators();
                    }
                case "getversion":
                    {
                        return GetVersion();
                    }
                case "invokefunction":
                    {
                        UInt160 script_hash = UInt160.Parse(_params[0].AsString());
                        string operation = _params[1].AsString();
                        ContractParameter[] args = _params.Count >= 3 ? ((JArray)_params[2]).Select(p => ContractParameter.FromJson(p)).ToArray() : new ContractParameter[0];
                        return InvokeFunction(script_hash, operation, args);
                    }
                case "invokescript":
                    {
                        byte[] script = _params[0].AsString().HexToBytes();
                        CheckWitnessHashes checkWitnessHashes = null;
                        if (_params.Count > 1)
                        {
                            UInt160[] scriptHashesForVerifying = _params.Skip(1).Select(u => UInt160.Parse(u.AsString())).ToArray();
                            checkWitnessHashes = new CheckWitnessHashes(scriptHashesForVerifying);
                        }
                        return GetInvokeResult(script, checkWitnessHashes);
                    }
                case "listplugins":
                    {
                        return ListPlugins();
                    }
                case "sendrawtransaction":
                    {
                        Transaction tx = _params[0].AsString().HexToBytes().AsSerializable<Transaction>();
                        return SendRawTransaction(tx);
                    }
                case "submitblock":
                    {
                        Block block = _params[0].AsString().HexToBytes().AsSerializable<Block>();
                        return SubmitBlock(block);
                    }
                case "validateaddress":
                    {
                        string address = _params[0].AsString();
                        return ValidateAddress(address);
                    }
                default:
                    throw new QueryException(-32601, "Method not found");
            }
        }

        private async Task ProcessAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";
            if (context.Request.Method != "GET" && context.Request.Method != "POST") return;
            JObject request = null;
            if (context.Request.Method == "GET")
            {
                string jsonrpc = context.Request.Query["jsonrpc"];
                string id = context.Request.Query["id"];
                string method = context.Request.Query["method"];
                string _params = context.Request.Query["params"];
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(method) && !string.IsNullOrEmpty(_params))
                {
                    try
                    {
                        _params = Encoding.UTF8.GetString(Convert.FromBase64String(_params));
                    }
                    catch (FormatException) { }
                    request = new JObject();
                    if (!string.IsNullOrEmpty(jsonrpc))
                        request["jsonrpc"] = jsonrpc;
                    request["id"] = id;
                    request["method"] = method;
                    request["params"] = JObject.Parse(_params);
                }
            }
            else if (context.Request.Method == "POST")
            {
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    try
                    {
                        request = JObject.Parse(reader);
                    }
                    catch (FormatException) { }
                }
            }
            JObject response;
            if (request == null)
            {
                response = CreateErrorResponse(null, -32700, "Parse error");
            }
            else if (request is JArray array)
            {
                if (array.Count == 0)
                {
                    response = CreateErrorResponse(request["id"], -32600, "Invalid Request");
                }
                else
                {
                    response = array.Select(p => ProcessRequest(context, p)).Where(p => p != null).ToArray();
                }
            }
            else
            {
                response = ProcessRequest(context, request);
            }
            if (response == null || (response as JArray)?.Count == 0) return;
            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);
        }

        private JObject ProcessRequest(HttpContext context, JObject request)
        {
            if (!request.ContainsProperty("id")) return null;
            if (!request.ContainsProperty("method") || !request.ContainsProperty("params") || !(request["params"] is JArray))
            {
                return CreateErrorResponse(request["id"], -32600, "Invalid Request");
            }
            JObject result = null;
            try
            {
                string method = request["method"].AsString();
                JArray _params = (JArray)request["params"];
                //foreach (IRpcPlugin plugin in Plugin.RpcPlugins)
                //    plugin.PreProcess(context, method, _params);
                //foreach (IRpcPlugin plugin in Plugin.RpcPlugins)
                //{
                //    result = plugin.OnProcess(context, method, _params);
                //    if (result != null) break;
                //}
                //if (result == null)
                result = Process(method, _params);
                //foreach (IRpcPlugin plugin in Plugin.RpcPlugins)
                //    plugin.PostProcess(context, method, _params, result);
            }
            catch (FormatException)
            {
                return CreateErrorResponse(request["id"], -32602, "Invalid params");
            }
            catch (IndexOutOfRangeException)
            {
                return CreateErrorResponse(request["id"], -32602, "Invalid params");
            }
            catch (Exception ex)
            {
#if DEBUG
                return CreateErrorResponse(request["id"], ex.HResult, ex.Message, ex.StackTrace);
#else
                return CreateErrorResponse(request["id"], ex.HResult, ex.Message);
#endif
            }
            JObject response = CreateResponse(request["id"]);
            response["result"] = result;
            return response;
        }

        public void Start(IPAddress bindAddress, int port, string sslCert = null, string password = null, string[] trustedAuthorities = null)
        {
            host = new WebHostBuilder().UseKestrel(options => options.Listen(bindAddress, port, listenOptions =>
            {
                if (string.IsNullOrEmpty(sslCert)) return;
                listenOptions.UseHttps(sslCert, password, httpsConnectionAdapterOptions =>
                {
                    if (trustedAuthorities is null || trustedAuthorities.Length == 0)
                        return;
                    httpsConnectionAdapterOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    httpsConnectionAdapterOptions.ClientCertificateValidation = (cert, chain, err) =>
                    {
                        if (err != SslPolicyErrors.None)
                            return false;
                        X509Certificate2 authority = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                        return trustedAuthorities.Contains(authority.Thumbprint);
                    };
                });
            }))
            .Configure(app =>
            {
                app.UseResponseCompression();
                app.Run(ProcessAsync);
            })
            .ConfigureServices(services =>
            {
                services.AddResponseCompression(options =>
                {
                    // options.EnableForHttps = false;
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json-rpc" });
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            })
            .Build();

            host.Start();
        }
    }
}
