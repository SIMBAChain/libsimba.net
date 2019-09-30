using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Swagger
{
    /// <summary>
    ///     Info
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Info : Serializable
    {
        [JsonProperty("x-simba-attrs")] public ApplicationMetadata SimbaAttrs { get; set; }
    }
}