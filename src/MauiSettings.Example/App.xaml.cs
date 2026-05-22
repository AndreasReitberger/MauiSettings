using MauiSettings.Example.Interfaces;

namespace MauiSettings.Example
{
    public partial class App : Application
    {
        public static string Hash { get; set; } = "mYGUbR61NUNjIvdEv/veySPxQEWcCRUZ3SZ7TT72IuI=";

        readonly IServiceProvider? serviceProvider;
#if MauiAppSettings
        readonly IAppSettingsService? appSettings;
#endif
#if MauiAppSettings
        public App(IServiceProvider serviceProvider, IAppSettingsService appSettings)
#else
        public App(IServiceProvider serviceProvider)
#endif
        {
            this.serviceProvider = serviceProvider;
#if MauiAppSettings
            this.appSettings = appSettings;
#endif
            InitializeComponent();
            // Example of how to generate a new key
            //string t = EncryptionManager.GenerateBase64Key();
#if !MauiAppSettings
            // Only Async methods do support encryption!
            SettingsApp.Dispatcher = DispatcherProvider.Current.GetForCurrentThread();
            // Set the context globally, so you don't have to pass it for each method.
            SettingsApp.Context = AppSourceGenerationContext.Default;
            //_ = Task.Run(async () => await SettingsApp.LoadSettingsAsync(Hash));
#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
#if MauiAppSettings
            /*
            appSettings!.Dispatcher = DispatcherProvider.Current.GetForCurrentThread();
            appSettings!.LoadSettingsAsync()
                .GetAwaiter()
                .GetResult();
            */
#endif
            return new Window(new AppShell());
        }

        protected override void OnSleep()
        {
            base.OnSleep();
#if MauiAppSettings
            if (appSettings?.SettingsChanged is true)
            {
                try
                {
                    appSettings.SaveSettings(Hash);
                }
                catch (Exception)
                {

                }
            }
#else
            if (SettingsApp.SettingsChanged)
            {
                try
                {
                    SettingsApp.SaveSettings(Hash);
                }
                catch (Exception)
                {

                }
            }
#endif
        }
    }
}
