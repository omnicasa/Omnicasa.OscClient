using Newtonsoft.Json.Converters;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    internal class StringEnumConverterCameCase : StringEnumConverter
    {
        /// <inheritdoc/>
        public StringEnumConverterCameCase()
            : base(null,  true)
        {
        }
    }
}