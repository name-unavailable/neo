using GraphQL.Types;
using Neo.Network.P2P.Payloads;

namespace Neo.Network.GraphQL
{
    public class TransactionsType : ObjectGraphType<Transaction>
    {
        public TransactionsType()
        {
            Field(x => x.Hash, type: typeof(UInt256GraphType));
            Field(x => x.Size, type: typeof(IntGraphType));
            Field(x => x.Nonce, type: typeof(UIntGraphType));
            Field(x => x.Sender, type: typeof(UInt160GraphType));
            Field(x => x.SystemFee, type: typeof(LongGraphType));
            Field(x => x.NetworkFee, type: typeof(LongGraphType));
            Field(x => x.ValidUntilBlock, type: typeof(UIntGraphType));
            Field(x => x.Attributes, type: typeof(ListGraphType<TransactionAttributeType>));
            Field(x => x.Cosigners, type: typeof(CosignersType));
            Field(x => x.Script, type: typeof(ListGraphType<ByteGraphType>));
            Field(x => x.Witnesses,type: typeof(WitnessesType));
        }
    }

    public class TransactionAttributeType: ObjectGraphType<TransactionAttribute>
    {
        public TransactionAttributeType()
        {
            Field(x => x.Usage, type: typeof(TransactionAttributeUsageType));
            Field(x => x.Data, type: typeof(ListGraphType<ByteGraphType>));
        }
    }

    public class TransactionAttributeUsageType: EnumerationGraphType<TransactionAttributeUsage>
    {
        public TransactionAttributeUsageType()
        {
            Name = "TransactionAttributeUsage";
            AddValue("Url", "Data resource url", 0x81);
        }
    }
}