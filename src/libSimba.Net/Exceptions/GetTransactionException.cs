using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue fetching a transaction
    /// </summary>
    internal class GetTransactionException : Exception
    {
        public GetTransactionException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public GetTransactionException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}