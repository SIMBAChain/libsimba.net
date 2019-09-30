using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     Represents a bundle manifest
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BundleManifest : Serializable
    {
        [JsonProperty("manifest")] public FileManifest[] Files;
    }
}