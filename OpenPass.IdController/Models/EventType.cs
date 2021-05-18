namespace OpenPass.IdController.Models
{
    public enum EventType
    {
        Unknown = 0,
        BannerRequest = 1,
        ConsentGranted = 2,
        ConsentNotGranted = 3,
        BannerIgnored = 4,
        LearnMore = 5,
        EmailShared = 6,
        EmailEntered = 7,
        EmailValidated = 8,
        OpenPassSso = 9,
        OpenPassFacebook = 10,
        OpenPassGoogle = 11
    }
}
