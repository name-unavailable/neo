using GraphQL.Types;
using Neo.Network.P2P.Payloads;

namespace Neo.Network.GraphQL
{
    public class BlockType : ObjectGraphType<Block>
    {
      
        public BlockType()
        {
            Field(x => x.Index, type: typeof(UIntGraphType));
            Field(x => x.Hash, type: typeof(UInt256GraphType));
            Field(x => x.Size, type: typeof(IntGraphType));
            Field(x => x.Header, type: typeof(BlockHeaderType));
            Field(x => x.ConsensusData.Nonce, type: typeof(UInt64GraphType));
            Field(x => x.Transactions, type: typeof(ListGraphType<TransactionsType>));
        }
    } 
}
