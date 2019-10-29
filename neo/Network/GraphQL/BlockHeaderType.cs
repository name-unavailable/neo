using GraphQL.Types;
using Neo.Network.P2P.Payloads;

namespace Neo.Network.GraphQL
{
    public class BlockHeaderType : ObjectGraphType<Header>
    {
        public BlockHeaderType()
        {
            Field(x => x.Size, type: typeof(IntGraphType));
            Field(x => x.Version, type: typeof(UIntGraphType));
            Field(x => x.PrevHash, type: typeof(UInt256GraphType));
            Field(x => x.MerkleRoot, type: typeof(UInt256GraphType));
            Field(x => x.Timestamp, type: typeof(UInt64GraphType));
            Field(x => x.Index, type: typeof(UIntGraphType));
            Field(x => x.NextConsensus, type: typeof(UInt160GraphType));
            Field(x => x.Witness, type: typeof(WitnessesType));
        }
    }
}