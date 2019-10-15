using Neo.Network.RPC.Models;

namespace Neo.Network.Restful
{
    public class RequestParameters
    {
        public string ScriptHash { get; set; }
        public string Operation { get; set; }
        public RpcStack[] Params { get; set; }
    }
}
