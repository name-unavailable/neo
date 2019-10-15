using Neo.SmartContract;

namespace Neo.Network.Restful
{
    public class RequestParameters
    {
        public string ScriptHash { get; set; }
        public string Operation { get; set; }
        public ContractParameter[] Params { get; set; }

    }
}
