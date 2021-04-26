using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Models
{
    public class GenerateRequest : GenericRequest
    {
        public EventType EventType { get; set; }
    }
}
