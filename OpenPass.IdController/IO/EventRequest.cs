namespace OpenPass.IdController.Models
{
    /// <summary>
    /// Model for sending events
    /// </summary>
    public class EventRequest
    {
        public EventType EventType { get; set; }
        public string OriginHost { get; set; }
    }
}
