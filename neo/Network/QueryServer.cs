using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Plugins;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using Neo.Network.RPC;

namespace Neo.Network
{
    public class QueryServer
    {
        protected class CheckWitnessHashes : IVerifiable
        {
            private readonly UInt160[] _scriptHashesForVerifying;
            public Witness[] Witnesses { get; set; }
            public int Size { get; }

            public CheckWitnessHashes(UInt160[] scriptHashesForVerifying)
            {
                _scriptHashesForVerifying = scriptHashesForVerifying;
            }

            public void Serialize(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }

            public void Deserialize(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public void DeserializeUnsigned(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public UInt160[] GetScriptHashesForVerifying(Snapshot snapshot)
            {
                return _scriptHashesForVerifying;
            }

            public void SerializeUnsigned(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }
        }
        private readonly NeoSystem system;
        public long MaxGasInvoke { get; }

        protected JObject GetBestBlockHash()
        {
            return Blockchain.Singleton.CurrentBlockHash.ToString();
        }

        protected JObject GetBlock(JObject key, bool verbose)
        {
            Block block;
            if (key is JNumber)
            {
                uint index = uint.Parse(key.AsString());
                block = Blockchain.Singleton.Store.GetBlock(index);
            }
            else
            {
                UInt256 hash = UInt256.Parse(key.AsString());
                block = Blockchain.Singleton.Store.GetBlock(hash);
            }
            if (block == null)
                throw new RpcException(-100, "Unknown block");
            if (verbose)
            {
                JObject json = block.ToJson();
                json["confirmations"] = Blockchain.Singleton.Height - block.Index + 1;
                UInt256 hash = Blockchain.Singleton.Store.GetNextBlockHash(block.Hash);
                if (hash != null)
                    json["nextblockhash"] = hash.ToString();
                return json;
            }
            return block.ToArray().ToHexString();
        }

        protected JObject GetBlockCount()
        {
            return Blockchain.Singleton.Height + 1;
        }

        protected JObject GetBlockHash(uint height)
        {
            if (height <= Blockchain.Singleton.Height)
            {
                return Blockchain.Singleton.GetBlockHash(height).ToString();
            }
            throw new RpcException(-100, "Invalid Height");
        }

        protected JObject GetBlockHeader(JObject key, bool verbose)
        {
            Header header;
            if (key is JNumber)
            {
                uint height = uint.Parse(key.AsString());
                header = Blockchain.Singleton.Store.GetHeader(height);
            }
            else
            {
                UInt256 hash = UInt256.Parse(key.AsString());
                header = Blockchain.Singleton.Store.GetHeader(hash);
            }
            if (header == null)
                throw new RpcException(-100, "Unknown block");

            if (verbose)
            {
                JObject json = header.ToJson();
                json["confirmations"] = Blockchain.Singleton.Height - header.Index + 1;
                UInt256 hash = Blockchain.Singleton.Store.GetNextBlockHash(header.Hash);
                if (hash != null)
                    json["nextblockhash"] = hash.ToString();
                return json;
            }

            return header.ToArray().ToHexString();
        }

        protected JObject GetBlockSysFee(uint height)
        {
            if (height <= Blockchain.Singleton.Height)
                using (ApplicationEngine engine = NativeContract.GAS.TestCall("getSysFeeAmount", height))
                {
                    return engine.ResultStack.Peek().GetBigInteger().ToString();
                }
            throw new RpcException(-100, "Invalid Height");
        }

        protected JObject GetConnectionCount()
        {
            return LocalNode.Singleton.ConnectedCount;
        }

        protected JObject GetContractState(UInt160 script_hash)
        {
            ContractState contract = Blockchain.Singleton.Store.GetContracts().TryGet(script_hash);
            return contract?.ToJson() ?? throw new RpcException(-100, "Unknown contract");
        }

        protected JObject GetPeers()
        {
            JObject json = new JObject();
            json["unconnected"] = new JArray(LocalNode.Singleton.GetUnconnectedPeers().Select(p =>
            {
                JObject peerJson = new JObject();
                peerJson["address"] = p.Address.ToString();
                peerJson["port"] = p.Port;
                return peerJson;
            }));
            json["bad"] = new JArray(); //badpeers has been removed
            json["connected"] = new JArray(LocalNode.Singleton.GetRemoteNodes().Select(p =>
            {
                JObject peerJson = new JObject();
                peerJson["address"] = p.Remote.Address.ToString();
                peerJson["port"] = p.ListenerTcpPort;
                return peerJson;
            }));
            return json;
        }

        protected JObject GetRawMemPool(bool shouldGetUnverified)
        {
            if (!shouldGetUnverified)
                return new JArray(Blockchain.Singleton.MemPool.GetVerifiedTransactions().Select(p => (JObject)p.Hash.ToString()));

            JObject json = new JObject();
            json["height"] = Blockchain.Singleton.Height;
            Blockchain.Singleton.MemPool.GetVerifiedAndUnverifiedTransactions(
                out IEnumerable<Transaction> verifiedTransactions,
                out IEnumerable<Transaction> unverifiedTransactions);
            json["verified"] = new JArray(verifiedTransactions.Select(p => (JObject)p.Hash.ToString()));
            json["unverified"] = new JArray(unverifiedTransactions.Select(p => (JObject)p.Hash.ToString()));
            return json;
        }

        protected JObject GetRawTransaction(UInt256 hash, bool verbose)
        {
            Transaction tx = Blockchain.Singleton.GetTransaction(hash);
            if (tx == null)
                throw new RpcException(-100, "Unknown transaction");
            if (verbose)
            {
                JObject json = tx.ToJson();
                uint? height = Blockchain.Singleton.Store.GetTransactions().TryGet(hash)?.BlockIndex;
                if (height != null)
                {
                    Header header = Blockchain.Singleton.Store.GetHeader((uint)height);
                    json["blockhash"] = header.Hash.ToString();
                    json["confirmations"] = Blockchain.Singleton.Height - header.Index + 1;
                    json["blocktime"] = header.Timestamp;
                }
                return json;
            }
            return tx.ToArray().ToHexString();
        }

        protected JObject GetStorage(UInt160 script_hash, byte[] key)
        {
            StorageItem item = Blockchain.Singleton.Store.GetStorages().TryGet(new StorageKey
            {
                ScriptHash = script_hash,
                Key = key
            }) ?? new StorageItem();
            return item.Value?.ToHexString();
        }

        protected JObject GetTransactionHeight(UInt256 hash)
        {
            uint? height = Blockchain.Singleton.Store.GetTransactions().TryGet(hash)?.BlockIndex;
            if (height.HasValue) return height.Value;
            throw new RpcException(-100, "Unknown transaction");
        }

        protected JObject GetValidators()
        {
            using (Snapshot snapshot = Blockchain.Singleton.GetSnapshot())
            {
                var validators = NativeContract.NEO.GetValidators(snapshot);
                return NativeContract.NEO.GetRegisteredValidators(snapshot).Select(p =>
                {
                    JObject validator = new JObject();
                    validator["publickey"] = p.PublicKey.ToString();
                    validator["votes"] = p.Votes.ToString();
                    validator["active"] = validators.Contains(p.PublicKey);
                    return validator;
                }).ToArray();
            }
        }

        protected JObject GetVersion()
        {
            JObject json = new JObject();
            json["tcpPort"] = LocalNode.Singleton.ListenerTcpPort;
            json["wsPort"] = LocalNode.Singleton.ListenerWsPort;
            json["nonce"] = LocalNode.Nonce;
            json["useragent"] = LocalNode.UserAgent;
            return json;
        }

        protected JObject InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                script = sb.EmitAppCall(script_hash, operation, args).ToArray();
            }
            return GetInvokeResult(script);
        }

        protected JObject InvokeScript(byte[] script, CheckWitnessHashes checkWitnessHashes = null)
        {
            return GetInvokeResult(script, checkWitnessHashes);
        }

        protected JObject ListPlugins()
        {
            return new JArray(Plugin.Plugins
                .OrderBy(u => u.Name)
                .Select(u => new JObject
                {
                    ["name"] = u.Name,
                    ["version"] = u.Version.ToString(),
                    ["interfaces"] = new JArray(u.GetType().GetInterfaces()
                        .Select(p => p.Name)
                        .Where(p => p.EndsWith("Plugin"))
                        .Select(p => (JObject)p))
                }));
        }

        protected JObject SendRawTransaction(Transaction tx)
        {
            RelayResultReason reason = system.Blockchain.Ask<RelayResultReason>(tx).Result;
            return GetRelayResult(reason, tx.Hash);
        }

        protected JObject SubmitBlock(Block block)
        {
            RelayResultReason reason = system.Blockchain.Ask<RelayResultReason>(block).Result;
            return GetRelayResult(reason, block.Hash);
        }

        protected JObject ValidateAddress(string address)
        {
            JObject json = new JObject();
            UInt160 scriptHash;
            try
            {
                scriptHash = address.ToScriptHash();
            }
            catch
            {
                scriptHash = null;
            }
            json["address"] = address;
            json["isvalid"] = scriptHash != null;
            return json;
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
                    throw new RpcException(-501, "Block or transaction already exists and cannot be sent repeatedly.");
                case RelayResultReason.OutOfMemory:
                    throw new RpcException(-502, "The memory pool is full and no more transactions can be sent.");
                case RelayResultReason.UnableToVerify:
                    throw new RpcException(-503, "The block cannot be validated.");
                case RelayResultReason.Invalid:
                    throw new RpcException(-504, "Block or transaction validation failed.");
                case RelayResultReason.PolicyFail:
                    throw new RpcException(-505, "One of the Policy filters failed.");
                default:
                    throw new RpcException(-500, "Unknown error.");
            }
        }
    }
}