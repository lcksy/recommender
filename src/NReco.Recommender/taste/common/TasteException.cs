using System;

namespace NReco.CF.Taste.Common
{
    /// <summary>
    /// An exception thrown when an error occurs inside the Taste engine.
    /// </summary>
    public class TasteException : Exception
    {
        public TasteException() { }

        public TasteException(string message)
            : base(message)
        {
        }

        public TasteException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}