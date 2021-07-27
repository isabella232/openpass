using System.Runtime.Serialization;

namespace OpenPass.IdController.Models.Tracking
{
    public enum Session
    {
        [EnumMember(Value = "authenticated")]
        Authenticated = 1,
        [EnumMember(Value = "unauthenticated")]
        Unauthenticated
    }
}
