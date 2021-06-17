using System.Runtime.Serialization;

namespace OpenPass.IdController.Models.Tracking
{
    public enum Variant
    {
        [EnumMember(Value = "dialog")]
        Dialog = 1,
        [EnumMember(Value = "in-site")]
        InSite,
        [EnumMember(Value = "redirect")]
        Redirect
    }
}
