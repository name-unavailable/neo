using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neo.IO;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using System.Linq;
using Neo.Persistence;

namespace Neo.Network.Restful
{
    [Route("")]
    [Produces("application/json")]
    [ApiController]
    public class RestController : Controller
    {
        private readonly IRestService _restService;
        public RestController(IRestService restService)
        {
            _restService = restService;
        }

        [HttpGet("blocks/bestblockhash")]
        public ActionResult<JObject> GetBestBlockHash()
        {
            return _restService.GetBestBlockHash();
        }

        [HttpGet("blocks/{index}/{verbose?}")]
        public ActionResult<JObject> GetBlock(JObject index, int verbose = 0)
        {
            bool isVerbose = verbose == 0 ? false :true;
            return _restService.GetBlock(index, isVerbose);
        }

        [HttpGet("blocks/count")]
        public ActionResult<JObject> GetBlockCount()
        {
            return _restService.GetBlockCount();
        }

        [HttpGet("blocks/{index}/hash")]
        public ActionResult<JObject> GetBlockHash(string index)
        {
            uint height = uint.Parse(index);
            return _restService.GetBlockHash(height);
        }

        [HttpGet("blocks/{index}/header/{verbose?}")]
        public ActionResult<JObject> GetBlockHeader(JObject index, int verbose = 0)
        {
            bool isVerbose = verbose == 0 ? false : true;
            return _restService.GetBlockHeader(index, isVerbose);
        }

        [HttpGet("blocks/{index}/sysfee")]
        public ActionResult<JObject> GetBlockSysFee(string index)
        {
            uint height = uint.Parse(index);
            return _restService.GetBlockSysFee(height);
        }

        [HttpGet("contracts/{scriptHash}")]
        public ActionResult<JObject> GetContractState(string scriptHash)
        {
            UInt160 script_hash = UInt160.Parse(scriptHash);
            return _restService.GetContractState(script_hash);
        }

        [HttpGet("network/localnode/rawmempool/{getUnverified?}")]
        public ActionResult<JObject> GetRawMemPool(int getUnverified = 0)
        {
            bool shouldGetUnverified = getUnverified == 0 ? false : true;
            return _restService.GetRawMemPool(shouldGetUnverified);
        }

        [HttpGet("transactions/{txid}/{verbose?}")]
        public ActionResult<JObject> GetRawTransaction(string txid, int verbose = 0)
        {
            UInt256 hash = UInt256.Parse(txid);
            bool isVerbose = verbose == 0 ? false : true;
            return _restService.GetRawTransaction(hash, isVerbose);
        }

        [HttpGet("contracts/{scriptHash}/storage/{key}/value")]
        public ActionResult<JObject> GetStorage(string scriptHash, string key)
        {
            UInt160 script_hash = UInt160.Parse(scriptHash);
            return _restService.GetStorage(script_hash, key.HexToBytes());
        }

        [HttpGet("transactions/{txid}/height")]
        public ActionResult<JObject> GetTransactionHeight(string txid)
        {
            UInt256 hash = UInt256.Parse(txid);
            return _restService.GetTransactionHeight(hash);
        }

        [HttpGet("validators/latest")]
        public ActionResult<JObject> GetValidators(string assetId)
        {
            return _restService.GetValidators();
        }

        [HttpGet("network/localnode/version")]
        public ActionResult<JObject> GetVersion()
        {
            return _restService.GetVersion();
        }

        [HttpPost("contracts/invokingfunction")]
        public ActionResult<JObject> InvokeFunction(dynamic param)
        {
            UInt160 script_hash = UInt160.Parse(param.scriptHash);
            string operation = param.operation;
            ContractParameter[] args = param.ContainsKey("args") ? ((JArray)param.args).Select(p => ContractParameter.FromJson(p)).ToArray() : new ContractParameter[0];
            return _restService.InvokeFunction(script_hash, operation, args);
        }

        [HttpPost("contracts/invokingscript")]
        public ActionResult<JObject> InvokeScript(dynamic param)
        {
            byte[] script = param.script.HexToBytes();
            UInt160[] scriptHashesForVerifying = null;
            if(param.ContainsKey("scriptHashes")){
              scriptHashesForVerifying = ((JArray)param.scriptHashes).Select(u => UInt160.Parse(u.AsString())).ToArray();
            }
            return _restService.InvokeScript(script, scriptHashesForVerifying);
        }

        [HttpGet("network/localnode/plugins")]
        public ActionResult<JObject> ListPlugins()
        {
            return _restService.ListPlugins();
        }

        [HttpGet("transactions/broadcasting/{hex}")]
        public ActionResult<JObject> SendRawTransaction(string hex)
        {
            Transaction tx = hex.HexToBytes().AsSerializable<Transaction>();
            return _restService.SendRawTransaction(tx);
        }

        [HttpGet("validators/submitblock/{hex}")]
        public ActionResult<JObject> SubmitBlock(string hex)
        {
            Block block = hex.HexToBytes().AsSerializable<Block>();
            return _restService.SubmitBlock(block);
        }

        [HttpGet("wallets/verifyingaddress/{address}")]
        public ActionResult<JObject> ValidateAddress(string address)
        {
            return _restService.ValidateAddress(address);
        }

        
    }
}