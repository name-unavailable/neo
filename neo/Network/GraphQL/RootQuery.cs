using GraphQL.Types;
using Neo.Network.Restful;
using Neo.Network.P2P.Payloads;
using Neo.IO.Json;
using Neo.IO;
using Neo.Network.RPC.Models;
using System.Linq;
using System.Collections.Generic;
using Neo.SmartContract;
using Neo.Ledger;

namespace Neo.Network.GraphQL
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IQueryService queryService)
        {
            Field<StringGraphType>("bestblockhash", resolve:
            context => queryService.GetBestBlockHash());

            Field<BlockType>("block",
                arguments: new QueryArguments(
            new QueryArgument<UIntGraphType> { Name = "index" },
            new QueryArgument<StringGraphType> { Name = "hash" }
            ), resolve: context =>
            {
                uint? index = context.GetArgument<uint>("index");
                bool verbose = true;
                if (index != null)
                {
                    return RpcBlock.FromJson(queryService.GetBlock(index, verbose));
                }
                else
                {
                    JObject hash = new JString(context.GetArgument<string>("hash"));
                    return Block.FromJson(queryService.GetBlock(hash, verbose));
                }
            });

            Field<IntGraphType>("blockcount", resolve: context =>
            {
                return queryService.GetBlockCount();
            });

            Field<StringGraphType>("blocksysfee",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<UIntGraphType>> { Name = "index" }
                ), resolve: context =>
            {
                var index = context.GetArgument<uint>("index");
                return queryService.GetBlockSysFee(index);
            });

            Field<TransactionsType>("transaction",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "txid" }
                ), resolve: context =>
            {
                var txid = UInt256.Parse(context.GetArgument<string>("txid"));
                bool verbose = true;
                return RpcTransaction.FromJson(queryService.GetRawTransaction(txid, verbose));
            });

            Field<UIntGraphType>("transactionheight",
              arguments: new QueryArguments(
                  new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "txid" }
              ), resolve: context =>
            {
                var txid = UInt256.Parse(context.GetArgument<string>("txid"));
                return queryService.GetTransactionHeight(txid);
            });

            Field<ContractsType>("contract",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scripthash" }
            ), resolve: context =>
            {
                var script_hash = UInt160.Parse(context.GetArgument<string>("scripthash"));
                return ContractState.FromJson(queryService.GetContractState(script_hash));
            });

            Field<ListGraphType<ValidatorsType>>("validators", resolve: context =>
            {
                return ((JArray)queryService.GetValidators()).Select(p => RpcValidator.FromJson(p)).ToArray();
            });

            Field<PeersType>("peers", resolve: context =>
            {
                return RpcPeers.FromJson(queryService.GetPeers());
            });

            Field<VertionType>("version", resolve: context =>
            {
                return queryService.GetVersion();
            });

            Field<IntGraphType>("connecttioncount", resolve: context =>
            {
                return queryService.GetConnectionCount();
            });

            Field<ListGraphType<PluginsType>>("plugins", resolve: context =>
            {
                return ((JArray)queryService.ListPlugins()).Select(p => RpcPlugin.FromJson(p)).ToArray();
            });

            Field<BooleanGraphType>("addressverification",
               arguments: new QueryArguments(
                   new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "address" }
           ), resolve: context =>
           {
               var address = context.GetArgument<string>("address");
               return queryService.ValidateAddress(address)["isvalid"].AsBoolean();
           });

            Field<InvokeResultType>("scriptinvocation", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "script" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "hashes" }
            ), resolve: context =>
            {
                byte[] script = context.GetArgument<string>("script").HexToBytes();
                var hashes = context.GetArgument<List<string>>("hashes")?.ToArray();
                UInt160[] scriptHashesForVerifying = null;
                if (hashes != null && hashes.Length > 0)
                {
                    scriptHashesForVerifying = hashes.Select(u => UInt160.Parse(u)).ToArray();
                }
                return RpcInvokeResult.FromJson(queryService.InvokeScript(script, scriptHashesForVerifying));
            });

            Field<InvokeResultType>("functioninvocation", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scriptHash" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "operation" },
                    new QueryArgument<ListGraphType<RpcStackInputType>> { Name = "params" }
            ), resolve: context =>
            {
                UInt160 script_hash = UInt160.Parse(context.GetArgument<string>("scriptHash"));
                string operation = context.GetArgument<string>("operation");
                ContractParameter[] args = context.GetArgument<List<RpcStack>>("params")?.Select(p => ContractParameter.FromJson(p.ToJson()))?.ToArray() ?? new ContractParameter[0];
                return RpcInvokeResult.FromJson(queryService.InvokeFunction(script_hash, operation, args));
            });

            Field<BooleanGraphType>("txbroadcasting", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "hex" }
            ), resolve: context =>
            {
                var hex = context.GetArgument<string>("hex");
                Transaction tx = hex.HexToBytes().AsSerializable<Transaction>();
                return queryService.SendRawTransaction(tx).AsBoolean();
            });

            Field<BooleanGraphType>("blockrelaying", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "hex" }
            ), resolve: context =>
            {
                var hex = context.GetArgument<string>("hex");
                Block block = hex.HexToBytes().AsSerializable<Block>();
                return queryService.SubmitBlock(block).AsBoolean();
            });

            Field<StringGraphType>("getstorage", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scriptHash" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key" }
            ), resolve: context =>
            {
                var scriptHash = UInt160.Parse(context.GetArgument<string>("scriptHash"));
                var key = context.GetArgument<string>("key").HexToBytes();
                return queryService.GetStorage(scriptHash, key)?.AsString();
            });
        }
    }
}
