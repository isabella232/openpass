using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenPass.IdController.Models.Tracking
{
    public class TrackingModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public View View { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Variant Variant { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Session Session { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Provider Provider { get; set; }
    }
}
