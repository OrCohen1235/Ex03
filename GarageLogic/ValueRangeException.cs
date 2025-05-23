using System;

namespace Ex03.GarageLogic
{
    public class ValueRangeException : Exception
    {
        public float MinValue { get; }
        public float MaxValue { get; }

        public ValueRangeException(float i_MinValue, float i_MaxValue)
            : base($"The value must be between {i_MinValue} and {i_MaxValue}.")
        {
            MinValue = i_MinValue;
            MaxValue = i_MaxValue;
        }

        public ValueRangeException(float i_MinValue, float i_MaxValue, string i_Message)
            : base($"{i_Message} Allowed range: {i_MinValue} to {i_MaxValue}.")
        {
            MinValue = i_MinValue;
            MaxValue = i_MaxValue;
        }

        public ValueRangeException(string i_Message)
            : base(i_Message)
        {
            MinValue = float.NaN;
            MaxValue = float.NaN;
        }
    }
}