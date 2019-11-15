using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Network
{
    public class QueryException : Exception
    {
        public QueryException(int code, string message) : base(message)
        {
            HResult = code;
        }
    }
}
