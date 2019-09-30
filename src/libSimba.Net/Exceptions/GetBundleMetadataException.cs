using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was an issue fetching a bundle
    /// </summary>
    internal class GetBundleMetadataException : Exception
    {
        public GetBundleMetadataException(
            string message,
            Exception innerException = null) : base(message, innerException)
        {
        }

        public GetBundleMetadataException(
            Exception innerException = null) : base(innerException?.Message, innerException)
        {
        }
    }
}