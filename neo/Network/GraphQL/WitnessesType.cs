using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;

namespace Neo.Network.GraphQL
{
    public class WitnessesType : ObjectGraphType<Witness>
    {
        public WitnessesType()
        {
            Field("Invocation", x => x.InvocationScript.ToHexString());
            Field("Verification", x => x.VerificationScript.ToHexString());
        }
    }
}