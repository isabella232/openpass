using System.Runtime.Serialization;

namespace OpenPass.IdController.Models.Tracking
{
    public enum Provider
    {
        [EnumMember(Value = "publisher")]
        Publisher = 1,
        [EnumMember(Value = "advertiser")]
        Advertiser
    }
}
