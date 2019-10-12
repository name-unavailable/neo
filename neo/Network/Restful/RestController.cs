using Microsoft.AspNetCore.Mvc;
using Neo.IO;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using System.Linq;
using Neo.Network.RPC;

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


        /// <summary>
        /// Get the lastest block hash of the blockchain 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">block hash</response>
        [HttpGet("blocks/bestblockhash")]
        [ProducesResponseType(typeof(string), 200)]
        public ActionResult<string> GetBestBlockHash()
        {
            return _restService.GetBestBlockHash();
        }


        /// <summary>
        /// Get a block at a certain height or with the specified hash
        /// </summary>
        /// <param name="index">block height or block hash</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">block information</response>
        /// <response code="400">Unknown Block</response>
        /// <response code="500">Format Invalid</response>
        [HttpGet("blocks/{indexorhash}/{verbose}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<JObject> GetBlock(string indexorhash, int verbose)
        {
            JObject _key = JObject.Parse(indexorhash);
            bool isVerbose = verbose == 0 ? false :true;
            return _restService.GetBlock(_key, isVerbose);
        }

        /// <summary>
        /// Get the block count of the blockchain
        /// </summary>
        /// <returns></returns>
        /// <response code="200">block count</response>
        [HttpGet("blocks/count")]
        [ProducesResponseType(typeof(double), 200)]
        public ActionResult<double> GetBlockCount()
        {
            return _restService.GetBlockCount();
        }


        /// <summary>
        /// Get the block hash with the specified index
        /// </summary>
        /// <param name="index">block height</param>
        /// <returns></returns>
        /// <response code="200">Block Hash</response>
        /// <response code="400">Invalid Height</response>
        [HttpGet("blocks/{index}/hash")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public ActionResult<string> GetBlockHash(uint index)
        {
            return _restService.GetBlockHash(index);
        }


        /// <summary>
        /// Get the block header with the specified hash
        /// </summary>
        /// <param name="index">block height</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">block header</response>
        /// <response code="400">Unknown Block</response>
        [HttpGet("blocks/{index}/header/{verbose}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public ActionResult<JObject> GetBlockHeader(uint index, int verbose = 0)
        {

            bool isVerbose = verbose == 0 ? false : true;
            return _restService.GetBlockHeader(index, isVerbose);
        }


        /// <summary>
        /// Get the system fees before the block with the specified index
        /// </summary>
        /// <param name="index">block height</param>
        /// <returns></returns>
        /// <response code="200">Block System Fee</response>
        /// <response code="400">Invalid Height</response>
        [HttpGet("blocks/{index}/sysfee")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public ActionResult<string> GetBlockSysFee(uint index)
        {
            return _restService.GetBlockSysFee(index);
        }


        /// <summary>
        /// Get a contract with the specified script hash
        /// </summary>
        /// <param name="scriptHash">contract scriptHash</param>
        /// <returns></returns>
        /// <response code="200">Contract State</response>
        /// <response code="400">Unknown contract</response>
        [HttpGet("contracts/{scriptHash}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public ActionResult<JObject> GetContractState(string scriptHash)
        {
            UInt160 script_hash = UInt160.Parse(scriptHash);
            return _restService.GetContractState(script_hash);
        }


        /// <summary>
        /// Gets unconfirmed transactions in memory
        /// </summary>
        /// <param name="getUnverified">0: get all transactions; 1: get verified transactions</param>
        /// <returns></returns>
        /// <response code="200">Transactions in Memory Pool</response>
        [HttpGet("network/localnode/rawmempool/{getUnverified}")]
        [ProducesResponseType(typeof(JObject), 200)]
        public ActionResult<JObject> GetRawMemPool(int getUnverified = 0)
        {
            bool shouldGetUnverified = getUnverified == 0 ? false : true;
            return _restService.GetRawMemPool(shouldGetUnverified);
        }


        /// <summary>
        /// Get a transaction with the specified hash value	
        /// </summary>
        /// <param name="txid">transaction hash</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">Transaction Information</response>
        /// <response code="400">Unknown Transaction</response>
        [HttpGet("transactions/{txid}/{verbose}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public ActionResult<JObject> GetRawTransaction(string txid, int verbose = 0)
        {
            UInt256 hash = UInt256.Parse(txid);
            bool isVerbose = verbose == 0 ? false : true;
            return _restService.GetRawTransaction(hash, isVerbose);
        }


        /// <summary>
        /// Get the stored value with the contract script hash and the key
        /// </summary>
        /// <param name="scriptHash">contract scriptHash</param>
        /// <param name="key">stored key</param>
        /// <returns></returns>
        /// <response code="200">Stored Value</response>
        [HttpGet("contracts/{scriptHash}/storage/{key}/value")]
        [ProducesResponseType(typeof(JObject), 200)]
        public ActionResult<JObject> GetStorage(string scriptHash, string key)
        {
            UInt160 script_hash = UInt160.Parse(scriptHash);
            return _restService.GetStorage(script_hash, key.HexToBytes());
        }


        /// <summary>
        /// Get the block index in which the transaction is found
        /// </summary>
        /// <param name="txid">transaction hash</param>
        /// <returns></returns>
        /// <response code="200">Transaction Height</response>
        /// <response code="400">Unknown Transaction</response>
        [HttpGet("transactions/{txid}/height")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(400)]
        public ActionResult<double> GetTransactionHeight(string txid)
        {
            UInt256 hash = UInt256.Parse(txid);
            return _restService.GetTransactionHeight(hash);
        }


        /// <summary>
        /// Get latest validators
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Latest Validators</response>
        [HttpGet("validators/latest")]
        [ProducesResponseType(typeof(double), 200)]
        public ActionResult<JObject> GetValidators()
        {
            return _restService.GetValidators();
        }


        /// <summary>
        /// Get version of the connected node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Node Version</response>
        [HttpGet("network/localnode/version")]
        [ProducesResponseType(typeof(JObject), 200)]
        public ActionResult<JObject> GetVersion()
        {
            return _restService.GetVersion();
        }


        /// <summary>
        /// Invoke a smart contract with specified script hash, passing in an operation and the corresponding params	
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("contracts/invokingfunction")]
        public ActionResult<JObject> InvokeFunction(dynamic param)
        {
            UInt160 script_hash = UInt160.Parse(param.scriptHash);
            string operation = param.operation;
            ContractParameter[] args = param.ContainsKey("args") ? ((JArray)param.args).Select(p => ContractParameter.FromJson(p)).ToArray() : new ContractParameter[0];
            return _restService.InvokeFunction(script_hash, operation, args);
        }


        /// <summary>
        /// Run a script through the virtual machine and get the result
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get plugins loaded by the node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Plugins Loaded</response>
        [HttpGet("network/localnode/plugins")]
        [ProducesResponseType(typeof(JObject), 200)]
        public ActionResult<JObject> ListPlugins()
        {
            return _restService.ListPlugins();
        }


        /// <summary>
        /// Broadcast a transaction over the network
        /// </summary>
        /// <param name="hex">hexstring of the transaction</param>
        /// <returns></returns>
        [HttpGet("transactions/broadcasting/{hex}")]
        public ActionResult<JObject> SendRawTransaction(string hex)
        {
            Transaction tx = hex.HexToBytes().AsSerializable<Transaction>();
            return _restService.SendRawTransaction(tx);
        }


        /// <summary>
        /// Relay a new block to the network
        /// </summary>
        /// <param name="hex">hexstring of the block</param>
        /// <returns></returns>
        [HttpGet("validators/submitblock/{hex}")]
        public ActionResult<JObject> SubmitBlock(string hex)
        {
            Block block = hex.HexToBytes().AsSerializable<Block>();
            return _restService.SubmitBlock(block);
        }


        /// <summary>
        /// Verify whether the address is a correct NEO address	
        /// </summary>
        /// <param name="address">address to be veirifed</param>
        /// <returns></returns>
        /// <response code="200">Verification Result</response>
        [HttpGet("wallets/verifyingaddress/{address}")]
        [ProducesResponseType(typeof(JObject), 200)]
        public ActionResult<JObject> ValidateAddress(string address)
        {
            return _restService.ValidateAddress(address);
        }

        
    }
}
