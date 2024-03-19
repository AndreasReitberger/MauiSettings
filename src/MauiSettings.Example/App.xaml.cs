using AndreasReitberger.Shared.Core.Utilities;
using MauiSettings.Example.Models.Settings;

namespace MauiSettings.Example
{
    public partial class App : Application
    {
        //public static string Hash = "96ug+F5VbYkQjqyolK8SO/R/HHeBZ2srDfMRR7uwrKA=";
        public static string Hash = "mYGUbR61NUNjIvdEv/veySPxQEWcCRUZ3SZ7TT72IuI=";
        public App()
        {
            InitializeComponent();
            // Example of how to generate a new key
            //string t = EncryptionManager.GenerateBase64Key();
            //SettingsApp.LoadSettings();
            // Needed in order to load only the secure settings (for instance the license)
            //Dispatcher.DispatchAsync(async() => await SettingsApp.LoadSecureSettingsAsync(Hash));

            // Only Async methods do support encryption!
            _ = Task.Run(async () => await SettingsApp.LoadSettingsAsync(Hash));
            /*
            Dispatcher.DispatchAsync(async () =>
            { 
                await SettingsApp.LoadSettingsAsync(Hash);
            });*/
            MainPage = new AppShell();
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
