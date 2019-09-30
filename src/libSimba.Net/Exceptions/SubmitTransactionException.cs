using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue submitting the transaction
    /// </summary>
    internal class SubmitTransactionException : Exception
    {
        public SubmitTransactionException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public SubmitTransactionException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}