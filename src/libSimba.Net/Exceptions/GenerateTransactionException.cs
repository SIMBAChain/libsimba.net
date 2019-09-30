using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue generating a transaction
    /// </summary>
    internal class GenerateTransactionException : Exception
    {
        public GenerateTransactionException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public GenerateTransactionException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}