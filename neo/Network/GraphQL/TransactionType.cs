using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.Wallets;
using System.Linq;
using Neo.IO;
using Neo.IO.Json;

namespace Neo.Network.GraphQL
{
    public class TransactionsType : ObjectGraphType<RpcTransaction>
    {
        public TransactionsType()
        {
            Field("Transaction", x => x.Transaction, type: typeof(TransactionType));
            Field("BlockHash", x => x.BlockHash.ToString());
            Field(x => x.Confirmations, type: typeof(IntGraphType)); 
            Field(x => x.BlockTime, type: typeof(UIntGraphType)); 
        }
    }

    public class TransactionType : ObjectGraphType<Transaction>
    {
        public TransactionType()
        {
            Field("Hash", x => x.Hash.ToString());
            Field(x => x.Size);
            Field(x => x.Version, type: typeof(UIntGraphType));
            Field(x => x.Nonce, type: typeof(UIntGraphType));
            Field("Sender", x => x.Sender.ToAddress());  
            Field("SystemFee", x => x.SystemFee.ToString());
            Field("NetworkFee", x => x.NetworkFee.ToString());
            Field(x => x.ValidUntilBlock, type: typeof(UIntGraphType));
            Field(x => x.Attributes, type: typeof(ListGraphType<TransactionAttributeType>));
            Field(x => x.Cosigners, type: typeof(ListGraphType<CosignersType>));
            Field("Script", x => x.Script.ToHexString());
            Field(x => x.Witnesses, type: typeof(ListGraphType<WitnessesType>));
        }
    }

    public class TransactionAttributeType : ObjectGraphType<TransactionAttribute>
    {
        public TransactionAttributeType()
        {
            Field(x => x.Usage, type: typeof(TransactionAttributeUsageType));
            Field("Data", x => x.Data.ToHexString());
        }
    }

    public class TransactionAttributeUsageType : EnumerationGraphType<TransactionAttributeUsage> { }

    public class CosignersType : ObjectGraphType<Cosigner>
    {
        public CosignersType()
        {
            Field("Account", x => x.Account.ToString());
            Field(x => x.Scopes, type: typeof(WitnessScopeEnum));
            Field("AllowedContracts", x => x.AllowedContracts.Select(p => (JObject)p.ToString()).ToArray(), type: typeof(ListGraphType<StringGraphType>));
            Field("AllowedGroups", x => x.AllowedGroups.Select(p => (JObject)p.ToString()).ToArray(), type: typeof(ListGraphType<StringGraphType>));
        }
    }

    public class WitnessScopeEnum : EnumerationGraphType<WitnessScope> { }
}