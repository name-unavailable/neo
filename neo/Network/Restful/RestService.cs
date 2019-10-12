using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.Wallets;
using System;

namespace Neo.Network.Restful
{
    public class RestService : QueryServer, IRestService
    {
        public RestService(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default)
        {
            this.NeoSystem = system;
            this.Wallet = wallet;
            this.MaxGasInvoke = maxGasInvoke;
        }

        public new ActionResult<string> GetBestBlockHash() => base.GetBestBlockHash().AsString();

        public new ActionResult<JObject> GetBlock(JObject key, bool verbose)
        {
            return base.GetBlock(key, verbose);
        }

        public new ActionResult<double> GetBlockCount() => base.GetBlockCount().AsNumber();

        public new ActionResult<string> GetBlockHash(uint height) => base.GetBlockHash(height).AsString();

        public new ActionResult<JObject> GetBlockHeader(JObject key, bool verbose) => base.GetBlockHeader(key, verbose);

        public new ActionResult<string> GetBlockSysFee(uint height) => base.GetBlockSysFee(height).AsString();

        public new ActionResult<JObject> GetContractState(UInt160 script_hash) => base.GetContractState(script_hash);

        public new ActionResult<JObject> GetRawMemPool(bool shouldGetUnverified) => base.GetRawMemPool(shouldGetUnverified);

        public new ActionResult<JObject> GetRawTransaction(UInt256 hash, bool verbose) => base.GetRawTransaction(hash, verbose);

        public new ActionResult<JObject> GetStorage(UInt160 script_hash, byte[] key) => base.GetStorage(script_hash, key);

        public new ActionResult<double> GetTransactionHeight(UInt256 hash) => base.GetTransactionHeight(hash).AsNumber();

        public new ActionResult<JObject> GetValidators() => base.GetValidators();

        public new ActionResult<JObject> GetVersion() => base.GetVersion();

        public new ActionResult<JObject> InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args) => InvokeFunction(script_hash, operation, args);

        public ActionResult<JObject> InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying)
        {
            CheckWitnessHashes checkWitnessHashes = null;
            if (scriptHashesForVerifying != null)
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
