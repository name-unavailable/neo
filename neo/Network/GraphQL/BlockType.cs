using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.Wallets;

namespace Neo.Network.GraphQL
{
    public class BlockType : ObjectGraphType<RpcBlock>
    {
        public BlockType()
        {
            Field("Hash", x => x.Block.Hash.ToString()); 
            Field(x => x.Block.Size); 
            Field(x => x.Block.Version, type: typeof(UIntGraphType)); 
            Field("PreviousBlockHash", x => x.Block.PrevHash.ToString()); 
            Field("MerkleRoot", x => x.Block.MerkleRoot.ToString()); 
            Field(x => x.Block.Timestamp, type: typeof(ULongGraphType)); 
            Field(x => x.Block.Index, type: typeof(UIntGraphType)); 
            Field("NextConsensus", x => x.Block.NextConsensus.ToAddress());
            Field(x => x.Block.Witness, type: typeof(WitnessesType)); 
            Field(x => x.Block.ConsensusData, type: typeof(ConsensusDataType)); 
            Field(x => x.Block.Transactions, type: typeof(ListGraphType<TransactionType>)); 
            Field(x => x.Confirmations, type: typeof(IntGraphType));  
            Field("NextBlockHash", x => x.NextBlockHash.ToString());
        }
    }

    public class ConsensusDataType: ObjectGraphType<ConsensusData>
    {
        public ConsensusDataType()
        {
            Field("Nonce", x => x.Nonce.ToString("x16"));  
            Field(x => x.PrimaryIndex, type: typeof(UIntGraphType));
        }
    }
}
