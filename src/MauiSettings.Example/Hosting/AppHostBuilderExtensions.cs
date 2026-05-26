#if MauiAppSettings
using MauiSettings.Example.Interfaces;
using MauiSettings.Example.Services;
#endif
using CommunityToolkit.Maui;
using MauiSettings.Example.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MauiSettings.Example.Hosting
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder ConfigureApp(this MauiAppBuilder builder)
        {
            builder
                .RegisterMainViews()
#if MauiAppSettings
                .ConfigureSettings(hash: App.Hash)
#endif
                ;
            return builder;
        }

        public static MauiAppBuilder RegisterMainViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingletonWithShellRoute<MainPage, MainPageViewModel>(nameof(MainPage));
            builder.Services.AddSingletonWithShellRoute<SettingsPage, SettingsPageViewModel>(nameof(SettingsPage));
            return builder;
        }

#if MauiAppSettings
        public static MauiAppBuilder ConfigureSettings(this MauiAppBuilder builder, string? hash = null, IDispatcher? dispatcher = null)
        {
            dispatcher ??= builder.Services.BuildServiceProvider().GetService<IDispatcher>();
            AppSettingsService settings = new()
            {
                Dispatcher = dispatcher,
                PassPhrase = hash,
                Context = AppSourceGenerationContext.Default
            };
            builder.Services.TryAddSingleton<IAppSettingsService>(settings);
            return builder;
        }
#endif

    }
}