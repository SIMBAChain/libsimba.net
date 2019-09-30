using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was a HTTP issue
    /// </summary>
    public class HttpException : Exception
    {
        protected internal HttpException(string message) : base(message)
        {
        }
    }
}