using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     Represents a Files manifest
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FileManifest : Serializable
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("mimetype")] public string MimeType { get; set; }

        [JsonProperty("size")] public int Size { get; set; }
    }
}