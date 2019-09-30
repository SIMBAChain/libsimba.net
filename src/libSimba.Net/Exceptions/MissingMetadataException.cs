using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that the app metadata is missing from the instance
    /// </summary>
    internal class MissingMetadataException : Exception
    {
        public MissingMetadataException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public MissingMetadataException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}