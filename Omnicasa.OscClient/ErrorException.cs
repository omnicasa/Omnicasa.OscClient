using System;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public class ErrorException : Exception
    {
        /// <inheritdoc/>
        public ErrorException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Get error code.
        /// </summary>
        public string Code { get; }
    }
}