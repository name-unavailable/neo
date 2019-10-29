using GraphQL.Types;
using Neo.Network.Restful;

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
            new QueryArgument<UIntGraphType> { Name = "index" }
            ), resolve: context =>
            {
                var id = context.GetArgument<uint>("index");
                var verbose = false;
                return restService.GetBlock(id, verbose);
            });
        }


    }
}
