using Microsoft.AspNetCore.Mvc;
using Neo.IO;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using System.Linq;
using System;
using Neo.Network.RPC;

namespace Neo.Network.Restful
{
    [Route("")]
    [Produces("application/json")]
    [ApiController]
    public class RestController : Controller
    {
        private readonly IQueryService _queryService;
        public RestController(IQueryService queryService)
        {
            _queryService = queryService;
        }

        /// <summary>
        /// Get the lastest block hash of the blockchain 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">block hash</response>
        [HttpGet("blocks/bestblockhash")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetBestBlockHash()
        {
            return Ok(_queryService.GetBestBlockHash());
        }

        /// <summary>
        /// Get a block at a certain height
        /// </summary>
        /// <param name="index">block height</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">block information</response>
        [HttpGet("blocks/getblockbyindex")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetBlockByIndex(int index, int verbose)
        {
            try
            {
                JObject _key = new JNumber(index);
                bool isVerbose = verbose == 0 ? false : true;
                return Content(_queryService.GetBlock(_key, isVerbose).ToString(), "application/json");
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a block with the specified hash
        /// </summary>
        /// <param name="hash">block hash</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">block information</response>
        [HttpGet("blocks/getblockbyhash")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetBlockByHash(string hash, int verbose)
        {
            try
            {
                JObject _key = new JString(hash);
                bool isVerbose = verbose == 0 ? false : true;
                return Content(_queryService.GetBlock(_key, isVerbose).ToString(), "application/json");
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the block count of the blockchain
        /// </summary>
        /// <returns></returns>
        /// <response code="200">block count</response>
        [HttpGet("blocks/count")]
        [ProducesResponseType(typeof(double), 200)]
        public IActionResult GetBlockCount()
        {
            return Ok(_queryService.GetBlockCount());
        }

        /// <summary>
        /// Get the block hash with the specified index
        /// </summary>
        /// <param name="index">block height</param>
        /// <returns></returns>
        /// <response code="200">Block Hash</response>
        [HttpGet("blocks/{index}/hash")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetBlockHash(uint index)
        {
            try
            {
                return Ok(_queryService.GetBlockHash(index));
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the block header with the specified hash
        /// </summary>
        /// <param name="index">block height</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">block header</response>
        [HttpGet("blocks/{index}/header/{verbose}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetBlockHeader(uint index, int verbose = 0)
        {
            try
            {
                bool isVerbose = verbose == 0 ? false : true;
                return Content(_queryService.GetBlockHeader(index, isVerbose).ToString(), "application/json");
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
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
        public IActionResult GetBlockSysFee(uint index)
        {
            try
            {
                return Ok(_queryService.GetBlockSysFee(index));
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a contract with the specified script hash
        /// </summary>
        /// <param name="scriptHash">contract scriptHash</param>
        /// <returns></returns>
        /// <response code="200">Contract State</response>
        /// <response code="400">Unknown Contract</response>
        [HttpGet("contracts/{scriptHash}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetContractState(string scriptHash)
        {
            try
            {
                UInt160 script_hash = UInt160.Parse(scriptHash);
                return Content(_queryService.GetContractState(script_hash).ToString(), "application/json");
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets unconfirmed transactions in memory
        /// </summary>
        /// <param name="getUnverified">0: get all transactions; 1: get verified transactions</param>
        /// <returns></returns>
        /// <response code="200">Transactions in Memory Pool</response>
        [HttpGet("network/localnode/rawmempool/{getUnverified}")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult GetRawMemPool(int getUnverified = 0)
        {
            bool shouldGetUnverified = getUnverified == 0 ? false : true;
            return Content(_queryService.GetRawMemPool(shouldGetUnverified).ToString(), "application/json");
        }

        /// <summary>
        /// Get a transaction with the specified hash value	
        /// </summary>
        /// <param name="txid">transaction hash</param>
        /// <param name="verbose">0:get block serialized in hexstring; 1: get block in Json format</param>
        /// <returns></returns>
        /// <response code="200">Transaction Information</response>
        [HttpGet("transactions/{txid}/{verbose}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetRawTransaction(string txid, int verbose = 0)
        {
            try
            {
                UInt256 hash = UInt256.Parse(txid);
                bool isVerbose = verbose == 0 ? false : true;
                return Content(_queryService.GetRawTransaction(hash, isVerbose).ToString(), "application/json");
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
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
        public IActionResult GetStorage(string scriptHash, string key)
        {
            UInt160 script_hash = UInt160.Parse(scriptHash);
            return Content(_queryService.GetStorage(script_hash, key.HexToBytes()).ToString(), "application/json");
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
        public IActionResult GetTransactionHeight(string txid)
        {
            try
            {
                UInt256 hash = UInt256.Parse(txid);
                return Ok(_queryService.GetTransactionHeight(hash));
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get latest validators
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Latest Validators</response>
        [HttpGet("validators/latest")]
        [ProducesResponseType(typeof(double), 200)]
        public IActionResult GetValidators()
        {
            return Content(_queryService.GetValidators().ToString(), "application/json");
        }

        /// <summary>
        /// Get version of the connected node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Node Version</response>
        [HttpGet("network/localnode/version")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult GetVersion()
        {
            return Content(_queryService.GetVersion().ToString(), "application/json");
        }

        /// <summary>
        /// Get the current number of connections for the node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Connections Count</response>
        [HttpGet("network/localnode/connections")]
        [ProducesResponseType(typeof(double), 200)]
        public IActionResult GetConnectionCount()
        {
            return Ok(_queryService.GetConnectionCount());
            
        }

        /// <summary>
        /// Get the peers of the node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Peers Information</response>
        [HttpGet("network/localnode/peers")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult GetPeers()
        {
            return Content(_queryService.GetPeers().ToString(), "application/json"); 
        }

        /// <summary>
        /// Invoke a smart contract with specified script hash, passing in an operation and the corresponding params	
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("contracts/invokingfunction")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult InvokeFunction(InvokeFunctionParameter param)
        {
            try
            {
                UInt160 script_hash = UInt160.Parse(param.ScriptHash);
                string operation = param.Operation;
                ContractParameter[] args = param.Params?.Select(p => ContractParameter.FromJson(p.ToJson()))?.ToArray() ?? new ContractParameter[0];
                return Content(_queryService.InvokeFunction(script_hash, operation, args).ToString(), "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Run a script through the virtual machine and get the result
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("contracts/invokingscript")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult InvokeScript(InvokeScriptParameter param)
        {
            try
            {
                byte[] script = param.Script.HexToBytes();
                UInt160[] scriptHashesForVerifying = null;
                if (param.Hashes != null && param.Hashes.Length > 0)
                {
                    scriptHashesForVerifying = param.Hashes.Select(u => UInt160.Parse(u)).ToArray();
                }
                return Content(_queryService.InvokeScript(script, scriptHashesForVerifying).ToString(), "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get plugins loaded by the node
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Plugins Loaded</response>
        [HttpGet("network/localnode/plugins")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult ListPlugins()
        {
            return Content(_queryService.ListPlugins().ToString(), "application/json");
        }

        /// <summary>
        /// Broadcast a transaction over the network
        /// </summary>
        /// <param name="hex">hexstring of the transaction</param>
        /// <returns></returns>
        [HttpGet("transactions/broadcasting/{hex}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult SendRawTransaction(string hex)
        {
            try
            {
                Transaction tx = hex.HexToBytes().AsSerializable<Transaction>();
                return Ok(_queryService.SendRawTransaction(tx));
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Relay a new block to the network
        /// </summary>
        /// <param name="hex">hexstring of the block</param>
        /// <returns></returns>
        [HttpGet("validators/submitblock/{hex}")]
        [ProducesResponseType(typeof(JObject), 200)]
        [ProducesResponseType(400)]
        public IActionResult SubmitBlock(string hex)
        {
            try
            {
                Block block = hex.HexToBytes().AsSerializable<Block>();
                return Ok(_queryService.SubmitBlock(block));
            }
            catch (RpcException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Verify whether the address is a correct NEO address	
        /// </summary>
        /// <param name="address">address to be veirifed</param>
        /// <returns></returns>
        /// <response code="200">Verification Result</response>
        [HttpGet("wallets/verifyingaddress/{address}")]
        [ProducesResponseType(typeof(JObject), 200)]
        public IActionResult ValidateAddress(string address)
        {
            return Content(_queryService.ValidateAddress(address).ToString(), "application/json");
        }
    }
}
