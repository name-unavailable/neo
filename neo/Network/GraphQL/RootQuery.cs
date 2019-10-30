using GraphQL.Types;
using Neo.Network.Restful;
using Neo.Network.P2P.Payloads;
using Neo.IO.Json;
using Neo.Network.P2P;

namespace Neo.Network.GraphQL
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IRestService restService)
        {
            Field<StringGraphType>("bestblockhash", resolve:
            context => restService.GetBestBlockHash());

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
                    return Block.FromJson(restService.GetBlock(index, verbose));
                }
                else
                {
                    JObject hash = new JString(context.GetArgument<string>("hash"));
                    return Block.FromJson(restService.GetBlock(hash, verbose));
                }
            });

            Field<IntGraphType>("blockcount", resolve: context =>
            {
                return restService.GetBlockCount();
            });

            Field<StringGraphType>("blocksysfee",
                arguments: new QueryArguments(
                    new QueryArgument<UIntGraphType> { Name = "index" }
                ), resolve: context =>
            {
                var index = context.GetArgument<uint>("index");
                return restService.GetBlockSysFee(index);
            });

            Field<ContractsType>("contract",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "scripthash" }
            ), resolve: context =>
            {
                var script_hash = UInt160.Parse(context.GetArgument<string>("scripthash"));
                return restService.GetContractState(script_hash);
            });


            Field<LocalNodeType>("nodeconnections", resolve: context =>
            {
                return LocalNode.Singleton;
                // JObject peers = restService.GetPeers();
                // JObject version = restService.GetVersion();

            });
        }
    }
}
