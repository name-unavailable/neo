using GraphQL.Types;
using Neo.SmartContract;

namespace Neo.Network.GraphQL
{
    public class ContractsType : ObjectGraphType<Contract>
    {
        public ContractsType()
        {
            // Field(x => x.Script);
            // Field(x => x.ParameterList);
            Field(x => x.Address);
            // Field(x => x.ScriptHash);
        }
    }
}