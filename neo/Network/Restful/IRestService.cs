using Neo.Network.P2P.Payloads;
using Neo.IO.Json;
using Neo.SmartContract;
using Microsoft.AspNetCore.Mvc;

namespace Neo.Network.Restful
{
    public interface IRestService
    {
         ActionResult<JObject> GetBestBlockHash();

         ActionResult<JObject> GetBlock(JObject key, bool verbose);

         ActionResult<JObject> GetBlockCount();

         ActionResult<JObject> GetBlockHash(uint height);

         ActionResult<JObject> GetBlockHeader(JObject key, bool verbose);

         ActionResult<JObject> GetBlockSysFee(uint height);

         ActionResult<JObject> GetContractState(UInt160 script_hash);

         ActionResult<JObject> GetRawMemPool(bool shouldGetUnverified);

         ActionResult<JObject> GetRawTransaction(UInt256 hash, bool verbose);

         ActionResult<JObject> GetStorage(UInt160 script_hash, byte[] key);

         ActionResult<JObject> GetTransactionHeight(UInt256 hash);

         ActionResult<JObject> GetValidators();

         ActionResult<JObject> GetVersion();

         ActionResult<JObject> InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args);

         ActionResult<JObject> InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying);

         ActionResult<JObject> ListPlugins();

        ActionResult<JObject> SendRawTransaction(Transaction tx);

         ActionResult<JObject> SubmitBlock(Block block);

         ActionResult<JObject> ValidateAddress(string address);

        //  ActionResult<ActionResult<JObject>> GetAccountState();

        //  ActionResult<ActionResult<JObject>> ListAddress();

        //  ActionResult<ActionResult<JObject>> ImportPrivKey();

        //  ActionResult<ActionResult<JObject>> ClaimGas();

        //  ActionResult<ActionResult<JObject>> GetWalletHeight();

        //  ActionResult<ActionResult<JObject>> GetNewAddress();

        //  ActionResult<ActionResult<JObject>> GetUnclaimedGas();

        //  ActionResult<ActionResult<JObject>> GetAllUnclaimedGas();

        //  ActionResult<ActionResult<JObject>> GetAssetBalance();

        //  ActionResult<ActionResult<JObject>> GetAssetState();

        //  ActionResult<ActionResult<JObject>> GetClaimbleGas();

        //  ActionResult<ActionResult<JObject>> GetNep5Balance();

        //  ActionResult<ActionResult<JObject>> ListAsync();

        //  ActionResult<ActionResult<JObject>> GetContractState();

        //  ActionResult<ActionResult<JObject>> GetNep5Transfers();

        //  ActionResult<ActionResult<JObject>> GetUnspents();

    }
}