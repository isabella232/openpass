using System.Runtime.Serialization;

namespace OpenPass.IdController.Models.Tracking
{
    public enum View
    {
        [EnumMember(Value = "native")]
        Native = 1,
        [EnumMember(Value = "modal")]
        Modal
    }
}
