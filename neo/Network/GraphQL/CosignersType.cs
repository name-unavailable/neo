using GraphQL.Types;
using Neo.Network.P2P.Payloads;

namespace Neo.Network.GraphQL
{
    public class CosignersType: ObjectGraphType<Cosigner>
    {
        public CosignersType()
        {
            Field(x => x.Account, type: typeof(UInt160GraphType)); 
            Field(x => x.Scopes, type: typeof(WitnessScopeEnum));
            Field(x => x.AllowedContracts, type: typeof(ListGraphType<UInt160GraphType>));
            Field(x => x.AllowedGroups, type: typeof(ListGraphType<ECPointGraphType>));
        }
    }

    public class WitnessScopeEnum : EnumerationGraphType<WitnessScope>
    {
        public WitnessScopeEnum()
        {
            Name = "WitnessScope";
            AddValue("Global", "allowed in all contexts", 0x00);
            AddValue("CalledByEntry", "EntryScriptHash == CallingScriptHash", 0x01);
            AddValue("CustomContracts", "Custom hash for contract-specific", 0x10);
            AddValue("CustomGroups", "Custom pubkey for group members", 0x20);
        }
    }
}