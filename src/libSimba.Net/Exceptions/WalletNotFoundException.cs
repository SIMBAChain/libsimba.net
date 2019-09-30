using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was no wallet found on this instance
    /// </summary>
    internal class WalletNotFoundException : Exception
    {
        public WalletNotFoundException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public WalletNotFoundException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}