using System;

namespace Ex03.GarageLogic
{
    public class ValueRangeException : Exception
    {
        private float m_MinValue;
        private float m_MaxValue;

        public ValueRangeException(float i_MinValue, float i_MaxValue)
            : base($"The value must be between {i_MinValue} and {i_MaxValue}.")
        {
            m_MinValue = i_MinValue;
            m_MaxValue = i_MaxValue;
        }

        public ValueRangeException(float i_MinValue, float i_MaxValue, string i_Message)
            : base($"{i_Message} Allowed range: {i_MinValue} to {i_MaxValue}.")
        {
            m_MinValue = i_MinValue;
            m_MaxValue = i_MaxValue;
        }

        public ValueRangeException(string i_Message)
            : base(i_Message)
        {
            m_MinValue = float.NaN;
            m_MaxValue = float.NaN;
        }
    }
}