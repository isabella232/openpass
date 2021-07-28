namespace OpenPass.IdController.Models.Tracking
{
    public class TrackingContext
    {
        public EventType EventType { get; set; }
        public string Uid2 { get; set; }
        public string Ifa { get; set; }
        public string View { get; set; }
        public string Variant { get; set; }
        public string Session { get; set; }
        public string Provider { get; set; }
    }
}
