using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Network.Restful
{
    public class RestService : QueryServer, IRestService
    {
        public RestService(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default) : base(system, wallet, maxGasInvoke) { }

        public new string GetBestBlockHash() => base.GetBestBlockHash().AsString();

        public new JObject GetBlock(JObject key, bool verbose)
        {
            return base.GetBlock(key, verbose);
        }

        public new double GetBlockCount() => base.GetBlockCount().AsNumber();

        public new string GetBlockHash(uint height) => base.GetBlockHash(height).AsString();

        public new JObject GetBlockHeader(JObject key, bool verbose) => base.GetBlockHeader(key, verbose);

        public new string GetBlockSysFee(uint height) => base.GetBlockSysFee(height).AsString();

        public new JObject GetContractState(UInt160 script_hash) => base.GetContractState(script_hash);

        public new JObject GetRawMemPool(bool shouldGetUnverified) => base.GetRawMemPool(shouldGetUnverified);

        public new JObject GetRawTransaction(UInt256 hash, bool verbose) => base.GetRawTransaction(hash, verbose);

        public new JObject GetStorage(UInt160 script_hash, byte[] key) => base.GetStorage(script_hash, key);

        public new double GetTransactionHeight(UInt256 hash) => base.GetTransactionHeight(hash).AsNumber();

        public new JObject GetValidators() => base.GetValidators();

        public new JObject GetVersion() => base.GetVersion();

        public new JObject InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args) => base.InvokeFunction(script_hash, operation, args);

        public JObject InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying)
        {
            CheckWitnessHashes checkWitnessHashes = null;
            if (scriptHashesForVerifying != null)
            {
                checkWitnessHashes = new CheckWitnessHashes(scriptHashesForVerifying);
            }
            return base.InvokeScript(script, checkWitnessHashes);
        }

        public new JObject ListPlugins() => base.ListPlugins();

        public new JObject SendRawTransaction(Transaction tx) => base.SendRawTransaction(tx);

        public new JObject SubmitBlock(Block block) => base.SubmitBlock(block);

        public new JObject ValidateAddress(string address) => base.ValidateAddress(address);

    }
}
