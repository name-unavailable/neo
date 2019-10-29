using GraphQL.Types;
using System;
using GraphQL.Language.AST;
using Neo.Cryptography.ECC;

namespace Neo.Network.GraphQL
{
    public class UInt256GraphType : ScalarGraphType
    {
        public UInt256GraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return UInt256.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return UInt256.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }
    public class UIntGraphType : ScalarGraphType
    {
        public UIntGraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return UInt32.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return UInt32.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }

    public class UInt64GraphType : ScalarGraphType
    {
        public UInt64GraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return UInt64.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return UInt64.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }

    public class UInt160GraphType : ScalarGraphType
    {
        public UInt160GraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return UInt160.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return UInt160.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }

    public class LongGraphType : ScalarGraphType
    {
        public LongGraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return Int64.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return Int64.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }

    public class ByteGraphType : ScalarGraphType
    {
        public ByteGraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return Byte.Parse(Convert.ToString(value));
        }

        public override object ParseValue(object value)  // Json format value
        {
            return Byte.Parse(Convert.ToString(value));
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }

    public class ECPointGraphType : ScalarGraphType
    {
        public ECPointGraphType() { }

        public override object ParseLiteral(IValue value)  // inline value
        {
            return ECPoint.Parse(Convert.ToString(value), ECCurve.Secp256r1);
        }

        public override object ParseValue(object value)  // Json format value
        {
            return ECPoint.Parse(Convert.ToString(value), ECCurve.Secp256r1);
        }

        public override object Serialize(object value)
        {
            return value.ToString();
        }
    }
}