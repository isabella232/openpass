namespace OpenPass.IdController.Models
{
    /// <summary>
    /// Model for generating open pass password
    /// </summary>
    public class GenerateRequest : GenericRequest
    {
        public EventType EventType { get; set; }
    }
}
