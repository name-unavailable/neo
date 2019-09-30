using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.Network.P2P.Payloads;


namespace Neo.Network.Restful
{
    public class RestService : QueryServer, IRestService
    {
        public new ActionResult<JObject> GetBestBlockHash() => base.GetBestBlockHash();

        public new ActionResult<JObject> GetBlock(JObject key, bool verbose) => base.GetBlock(key, verbose);

        public new ActionResult<JObject> GetBlockCount() => base.GetBlockCount();

        public new ActionResult<JObject> GetBlockHash(uint height) => base.GetBlockHash(height);

        public new ActionResult<JObject> GetBlockHeader(JObject key, bool verbose) => base.GetBlockHeader(key, verbose);

        public new ActionResult<JObject> GetBlockSysFee(uint height) => base.GetBlockSysFee(height);

        public new ActionResult<JObject> GetContractState(UInt160 script_hash) => base.GetContractState(script_hash);

        public new ActionResult<JObject> GetRawMemPool(bool shouldGetUnverified) => base.GetRawMemPool(shouldGetUnverified);

        public new ActionResult<JObject> GetRawTransaction(UInt256 hash, bool verbose) => base.GetRawTransaction(hash, verbose);

        public new ActionResult<JObject> GetStorage(UInt160 script_hash, byte[] key) => base.GetStorage(script_hash, key);

        public new ActionResult<JObject> GetTransactionHeight(UInt256 hash) => base.GetTransactionHeight(hash);

        public new ActionResult<JObject> GetValidators() => base.GetValidators();

        public new ActionResult<JObject> GetVersion() => base.GetVersion();

        public new ActionResult<JObject> InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args) => InvokeFunction(script_hash, operation, args);

        public ActionResult<JObject> InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying)
        {
            CheckWitnessHashes checkWitnessHashes = null;
            if(scriptHashesForVerifying != null)
            {
                checkWitnessHashes = new CheckWitnessHashes(scriptHashesForVerifying);
            }
            return base.InvokeScript(script, checkWitnessHashes);
        }

        public new ActionResult<JObject> ListPlugins() => base.ListPlugins();

        public new ActionResult<JObject> SendRawTransaction(Transaction tx) => base.SendRawTransaction(tx);

        public new ActionResult<JObject> SubmitBlock(Block block) => base.SubmitBlock(block);

        public new ActionResult<JObject> ValidateAddress(string address) => base.ValidateAddress(address);

    }
}