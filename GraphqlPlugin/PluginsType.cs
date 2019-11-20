using GraphQL.Types;
using Neo.Network.RPC.Models;

namespace GraphqlPlugin
{
    public class PluginsType: ObjectGraphType<RpcPlugin>
    {
        public PluginsType()
        {
            Field(x => x.Name);
            Field(x => x.Version);
            Field(x => x.Interfaces, type: typeof(ListGraphType<StringGraphType>));
        }
    }
}