using System;

namespace B25_Ex03_OriCohen_207008590_AlonZylberberg_315853739
{
    public class ValueRangeException : Exception
    {
        public ValueRangeException() : base("The value is out of the allowed range.") {}
        public ValueRangeException(string message) : base(message) {}
    }
    
    public class FormatException : Exception
    {
        public FormatException() : base("The format of the value is incorrect.") {}
        public FormatException(string message) : base(message) {}
    }
    
    public class ArgumentException : Exception
    {
        public ArgumentException() : base("The argument is invalid.") {}
        public ArgumentException(string message) : base(message) {}
    }
}

