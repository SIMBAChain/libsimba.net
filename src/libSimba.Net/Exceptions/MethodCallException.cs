using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue calling a method
    /// </summary>
    internal class MethodCallException : Exception
    {
        public MethodCallException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public MethodCallException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}