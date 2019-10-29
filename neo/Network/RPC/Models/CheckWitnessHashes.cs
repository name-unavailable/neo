using Neo.Network.P2P.Payloads;
using System;
using System.IO;
using Snapshot = Neo.Persistence.Snapshot;

namespace Neo.Network.RPC.Models
{
    public class CheckWitnessHashes : IVerifiable
    {
        private readonly UInt160[] _scriptHashesForVerifying;
        public Witness[] Witnesses { get; set; }
        public int Size { get; }

        public CheckWitnessHashes(UInt160[] scriptHashesForVerifying)
        {
            _scriptHashesForVerifying = scriptHashesForVerifying;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void DeserializeUnsigned(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public UInt160[] GetScriptHashesForVerifying(Snapshot snapshot)
        {
            return _scriptHashesForVerifying;
        }

        public void SerializeUnsigned(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
