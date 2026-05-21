namespace AndreasReitberger.Maui.Enums
{
#if MauiAppSettings
    public enum MauiAppSettingsTarget
#else
    public enum MauiSettingsTarget
#endif
    {
        Local,
#if IOS
        ICloud,
#endif
    }
}
