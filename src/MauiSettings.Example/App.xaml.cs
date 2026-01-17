using MauiSettings.Example.Models.Settings;

namespace MauiSettings.Example
{
    public partial class App : Application
    {
        public static string Hash = "mYGUbR61NUNjIvdEv/veySPxQEWcCRUZ3SZ7TT72IuI=";
        public App()
        {
            InitializeComponent();
            // Example of how to generate a new key
            //string t = EncryptionManager.GenerateBase64Key();

            // Only Async methods do support encryption!
            SettingsApp.Dispatcher = DispatcherProvider.Current.GetForCurrentThread();
            // Set the context globally, so you don't have to pass it for each method.
            SettingsApp.Context = AppSourceGenerationContext.Default;
            //_ = Task.Run(async () => await SettingsApp.LoadSettingsAsync(Hash));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override void OnSleep()
        {
            base.OnSleep();
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
        }
    }
}
