using GraphQL.Types;
using Neo.Network.P2P.Payloads;

namespace Neo.Network.GraphQL
{
    public class WitnessesType : ObjectGraphType<Witness>
    {
        public WitnessesType()
        {
            Field(x => x.InvocationScript, type: typeof(ListGraphType<ByteGraphType>));
            Field(x => x.VerificationScript, type: typeof(ListGraphType<ByteGraphType>));
        }
    }

    
}