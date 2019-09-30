using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue generating a paged response
    /// </summary>
    internal class GetPagedResponseException : Exception
    {
        public GetPagedResponseException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public GetPagedResponseException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}