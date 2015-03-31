using System;

namespace iCal.PCL.Serialization
{
    /// <summary>
    /// Some exception has been thrown. The inner exception contains the actual error.
    /// </summary>
    class iCalDeserializationException : Exception
    {
        /// <summary>
        /// Returns the parse text
        /// </summary>
        public string ParseText { get; set; }

        public iCalDeserializationException(string message, Exception inner, string parseText)
            : base(message, inner)
        {
            ParseText = parseText;
        }
    }
}
