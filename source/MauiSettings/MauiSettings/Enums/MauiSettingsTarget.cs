namespace AndreasReitberger.Maui.Enums
{
    public enum MauiSettingsTarget
    {
        Local,
#if IOS
        ICloud,
#endif
    }
}
