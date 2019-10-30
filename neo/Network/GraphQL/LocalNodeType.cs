using GraphQL.Types;
using Neo.Network.P2P;
using System.Net;

namespace Neo.Network.GraphQL
{
    class LocalNodeType : ObjectGraphType<LocalNode>
    {
        public LocalNodeType()
        {
            Field(x => LocalNode.Singleton.ConnectedCount);
            Field(x => LocalNode.Singleton.GetRemoteNodes(), type: typeof(ListGraphType<IPEndPointType>));
            // Field(x => LocalNode.Singleton.GetRemoteNodes(), type: typeof(ListGraphType<RemoteNodeType>));
            Field(x => LocalNode.Singleton.ListenerTcpPort);
            Field(x => LocalNode.Singleton.ListenerWsPort);
            Field(x => LocalNode.Nonce, type: typeof(UIntGraphType));
            Field(x => LocalNode.UserAgent);
        }
    }



    class IPEndPointType : ObjectGraphType<IPEndPoint>
    {
        public IPEndPointType()
        {
            Field(x => x.Address);
            Field(x => x.Port);
        }
    }

    class RemoteNodeType : ObjectGraphType<RemoteNode>
    {
        public RemoteNodeType()
        {
            Field(x => x.Remote.Address);
            Field(x => x.ListenerTcpPort);
        }
    }
}
