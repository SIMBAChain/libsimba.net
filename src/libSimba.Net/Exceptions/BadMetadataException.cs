using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue with the Apps Metadata
    /// </summary>
    internal class BadMetadataException : Exception
    {
        public BadMetadataException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public BadMetadataException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}