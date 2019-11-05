using GraphQL.Types;
using Neo.Network.RPC.Models;

namespace Neo.Network.GraphQL
{
    class PeersType : ObjectGraphType<RpcPeers>
    {
        public PeersType()
        {
            Field(x => x.Unconnected, type: typeof(ListGraphType<PeerType>));
            Field(x => x.Bad, type: typeof(ListGraphType<PeerType>));
            Field(x => x.Connected, type: typeof(ListGraphType<PeerType>));
        }
    }

    public class PeerType: ObjectGraphType<RpcPeer>
    {
        public PeerType()
        {
            Field(x => x.Address);
            Field(x => x.Port);
        }
    }    
}
