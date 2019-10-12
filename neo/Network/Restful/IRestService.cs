using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;

namespace Neo.Network.Restful
{
    public interface IRestService
    {
        string GetBestBlockHash();

        JObject GetBlock(JObject key, bool verbose);

        double GetBlockCount();

        string GetBlockHash(uint height);

        JObject GetBlockHeader(JObject key, bool verbose);

        string GetBlockSysFee(uint height);

        JObject GetContractState(UInt160 script_hash);

        JObject GetRawMemPool(bool shouldGetUnverified);

        JObject GetRawTransaction(UInt256 hash, bool verbose);

        JObject GetStorage(UInt160 script_hash, byte[] key);

        double GetTransactionHeight(UInt256 hash);

        JObject GetValidators();

        JObject GetVersion();

        JObject InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args);

        JObject InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying);

        JObject ListPlugins();

        JObject SendRawTransaction(Transaction tx);

        JObject SubmitBlock(Block block);

        JObject ValidateAddress(string address);

        //  JObject GetAccountState();

        //  JObject ListAddress();

        //  JObject ImportPrivKey();

        //  JObject ClaimGas();

        //  JObject GetWalletHeight();

        //  JObject GetNewAddress();

        //  JObject GetUnclaimedGas();

        //  JObject GetAllUnclaimedGas();

        //  JObject GetAssetBalance();

        //  JObject GetAssetState();

        //  JObject GetClaimbleGas();

        //  JObject GetNep5Balance();

        //  JObject ListAsync();

        //  JObject GetContractState();

        //  JObject GetNep5Transfers();

        //  JObject GetUnspents();

    }
}
