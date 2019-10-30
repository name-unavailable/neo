using GraphQL.Types;
using Neo.SmartContract;

namespace Neo.Network.GraphQL
{
    public class ContractsType : ObjectGraphType<Contract>
    {
        public ContractsType()
        {
            Field(x => x.Script, type: typeof(ListGraphType<ByteGraphType>));
            Field(x => x.ParameterList, type: typeof(ContractParameterType));
            Field(x => x.Address);
            Field(x => x.ScriptHash, type: typeof(UInt160GraphType));
        }
    }

    public class ContractParameterType : EnumerationGraphType
    {
        public ContractParameterType()
        {
            Name = "ContractParameter";
            AddValue("Signature", "", 0x00);
            AddValue("Boolean", "", 0x00);
            AddValue("Integer", "", 0x00);
            AddValue("Hash160", "", 0x00);
            AddValue("Hash256", "", 0x00);
            AddValue("ByteArray", "", 0x00);
            AddValue("PublicKey", "", 0x00);
            AddValue("String", "", 0x00);
            AddValue("Array", "", 0x00);
            AddValue("Map", "", 0x00);
            AddValue("InteropInterface", "", 0x00);
            AddValue("Any", "", 0x00);
            AddValue("Void", "", 0x00);
        }
    }
}