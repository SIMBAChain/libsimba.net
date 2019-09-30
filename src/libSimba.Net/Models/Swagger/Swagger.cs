using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Swagger
{
    /// <summary>
    ///     Swagger
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Swagger : Serializable
    {
        [JsonProperty("info")] public Info Info { get; set; }
    }
}