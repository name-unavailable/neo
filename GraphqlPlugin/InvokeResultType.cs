using GraphQL.Types;
using Neo.Network.RPC.Models;
using Neo.SmartContract;

namespace GraphqlPlugin
{
    public class InvokeResultType: ObjectGraphType<RpcInvokeResult>
    {
        public InvokeResultType()
        {
            Field(x => x.Script);
            Field(x => x.State);
            Field(x => x.GasConsumed);
            Field("Stack", x => x.Stack, type: typeof(ListGraphType<ContractParameterGraphType>));
            // Field(x => x.Tx);
        }
    }

    public class ContractParameterGraphType: ObjectGraphType<ContractParameter>
    {
        public ContractParameterGraphType()
        {
            Field(x => x.Type, type: typeof(ContractParameterEnumType));
            Field("Value", x => x.Value.ToString());
        }
    }

     public class RpcStackInputType: InputObjectGraphType
    {
        public RpcStackInputType()
        {
            Field<StringGraphType>("Type");
            Field<StringGraphType>("Value");
        }
    }

    public class ContractParameterEnumType: EnumerationGraphType<ContractParameterType> { }
}
