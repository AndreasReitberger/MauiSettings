#if false
using AndreasReitberger.Maui.Interfaces;
using System.Runtime.Versioning;

namespace AndreasReitberger.Maui
{
    [SupportedOSPlatform(SPlatforms.AndroidVersion)]
    [SupportedOSPlatform(SPlatforms.IOSVersion)]
    [SupportedOSPlatform(SPlatforms.MACCatalystVersion)]
    [SupportedOSPlatform(SPlatforms.WindowsVersion)]
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder ConfigureMauiSettings<T>(this MauiAppBuilder builder, T settingsObject, string? passphrase, string? hashedPassword, IDispatcher? dispatcher = null)
        {
            dispatcher ??= builder.Services.BuildServiceProvider().GetService<IDispatcher>();
            var settings = new MauiSettings<T>(passphrase, hashedPassword);
            builder.Services.AddSingleton<IMauiSettings<T>>(settings);
            return builder;
        }
    }
}
#endif