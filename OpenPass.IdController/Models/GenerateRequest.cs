using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Models
{
    public class GenerateRequest : GenericRequest
    {
        public EventType EventType { get; set; }
    }
}
