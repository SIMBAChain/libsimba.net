using System;

namespace libSimba.Net.Exceptions
{
    /// <summary>
    ///     Exception indicating that there was a HTTP issue
    /// </summary>
    public class SimbaHttpException : Exception
    {
        protected internal SimbaHttpException(string message) : base(message)
        {
        }
    }
}