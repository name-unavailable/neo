using GraphQL.Types;
using Neo.Network.RPC.Models;

namespace Neo.Network.GraphQL
{
    public class ValidatorsType: ObjectGraphType<RpcValidator>
    {
        public ValidatorsType()
        {
            Field(x => x.PublicKey);
            Field("Votes", x => x.Votes.ToString());
            Field(x => x.Active);
        }
    }
}