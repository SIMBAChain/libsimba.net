using Newtonsoft.Json;

namespace libSimba.Net.Models.Swagger
{
    /// <summary>
    ///     Parameter
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Parameter
    {
        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("type")] public string Type { get; set; }
    }
}