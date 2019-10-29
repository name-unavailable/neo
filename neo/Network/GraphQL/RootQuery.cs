using GraphQL.Types;
using Neo.Network.Restful;
using Neo.Network.P2P.Payloads;

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
            new QueryArgument<IntGraphType> { Name = "verbose" }
            ), resolve: context =>
            {
                var id = context.GetArgument<uint>("index");
                var verbose = context.GetArgument<uint>("verbose") == 0 ? false : true;
                return Block.FromJson(restService.GetBlock(id, verbose));
            });
        }
    }
}
