using GraphQL.Types;
using Neo.Network.RPC.Models;

namespace Neo.Network.GraphQL
{
    public class VertionType: ObjectGraphType<RpcVersion>
    {
        public VertionType()
        {
            Field(x => x.TcpPort);
            Field(x => x.WsPort);
            Field(x => x.Nonce, type: typeof(UIntGraphType));
            Field(x => x.UserAgent);
        }
    }
}