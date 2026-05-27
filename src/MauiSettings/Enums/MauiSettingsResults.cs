namespace AndreasReitberger.Maui.Enums
{
#if MauiAppSettings
    public enum MauiAppSettingsResults
#else
    public enum MauiSettingsResults
#endif
    {
        Success,
        Skipped,
        EncryptionError,
        Failed,
    }
}
