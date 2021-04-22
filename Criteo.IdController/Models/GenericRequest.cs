namespace Criteo.IdController.Models
{
    public abstract class GenericRequest
    {
        public string Email { get; set; }
        public string OriginHost { get; set; }
    }
}
