using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.Network.P2P.Payloads;


namespace Neo.Network.Restful
{
    public class RestService : QueryServer, IRestService
    {
        new ActionResult<JObject> GetBestBlockHash() => base.GetBestBlockHash();

        new ActionResult<JObject> GetBlock(JObject key, int verbose)
        {

        }

        new ActionResult<JObject> GetBlockCount() => base.GetBlockCount();

        new ActionResult<JObject> GetBlockHash(uint height) => base.GetBlockHash(height);

        new ActionResult<JObject> GetBlockHeader(JObject key, bool verbose) => base.GetBlockHeader(key, verbose);

        new ActionResult<JObject> GetBlockSysFee(uint height) => base.GetBlockSysFee(height);

        new ActionResult<JObject> GetContractState(UInt160 script_hash) => base.GetContractState(script_hash);

        new ActionResult<JObject> GetRawMemPool(bool shouldGetUnverified) => base.GetRawMemPool(shouldGetUnverified);

        new ActionResult<JObject> GetRawTransaction(UInt256 hash, bool verbose) => base.GetRawTransaction(hash, verbose);

        new ActionResult<JObject> GetStorage(UInt160 script_hash, byte[] key) => base.GetStorage(script_hash, key);

        new ActionResult<JObject> GetTransactionHeight(UInt256 hash) => base.GetTransactionHeight(hash);

        new ActionResult<JObject> GetValidators() => base.GetValidators();

        new ActionResult<JObject> GetVersion() => base.GetVersion();

        new ActionResult<JObject> InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args) => InvokeFunction(script_hash, operation, args);

        ActionResult<JObject> InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying)
        {
            CheckWitnessHashes checkWitnessHashes = null;
            if(scriptHashesForVerifying != null)
            {
                checkWitnessHashes = new CheckWitnessHashes(scriptHashesForVerifying);
            }
            return base.InvokeScript(script, checkWitnessHashes);
        }

        new ActionResult<JObject> ListPlugins() => base.ListPlugins();

        new ActionResult<JObject> SendRawTransaction(Transaction tx) => base.SendRawTransaction(tx);

        new ActionResult<JObject> SubmitBlock(Block block) => base.SubmitBlock(block);

        new ActionResult<JObject> ValidateAddress(string address) => base.ValidateAddress(address);

    }
}