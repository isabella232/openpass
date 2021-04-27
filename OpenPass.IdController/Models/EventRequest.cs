using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Models
{
    public class EventRequest
    {
        public EventType EventType { get; set; }
        public string OriginHost { get; set; }
        public string LocalWebId { get; set; }
        public string Uid { get; set; }
        public string Ifa { get; set; }
    }
}
