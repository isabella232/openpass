using Criteo.UserIdentification;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Models.Tracking
{
    public class TrackingContext
    {
        public EventType EventType { get; set; }
        public LocalWebId? LocalWebId { get; set; }
        public CriteoId? Uid { get; set; }
        public UserCentricAdId? Ifa { get; set; }
        public string View { get; set; }
        public string Variant { get; set; }
        public string Session { get; set; }
        public string Provider { get; set; }
    }
}
