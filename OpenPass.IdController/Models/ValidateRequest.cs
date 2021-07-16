namespace OpenPass.IdController.Models
{
    /// <summary>
    /// Model for validating one time password
    /// </summary>
    public class ValidateRequest : GenericRequest
    {
        public string Otp { get; set; }
    }
}
