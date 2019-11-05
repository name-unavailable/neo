using GraphQL.Types;
using System;
using GraphQL.Language.AST;

namespace Neo.Network.GraphQL
{
    public class UIntGraphType : ScalarGraphType
    {
        public UIntGraphType() { }

        public override object ParseLiteral(IValue value)  
        {
            return UInt32.Parse(Convert.ToString(value.Value));
        }

        public override object ParseValue(object value)  
        {
            return UInt32.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return Convert.ToUInt32(value);
        }
    }

    public class ULongGraphType : ScalarGraphType
    {
        public ULongGraphType() { }

        public override object ParseLiteral(IValue value)  
        {
            return UInt64.Parse(Convert.ToString(value.Value));
        }

        public override object ParseValue(object value)  
        {
            return UInt64.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return Convert.ToUInt64(value);
        }
    }
}
