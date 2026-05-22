using AndreasReitberger.Shared.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSettings.Example.Interfaces;
using MauiSettings.Example.Models.Settings;
using System.Collections.ObjectModel;

namespace MauiSettings.Example.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region Dependencies
#if MauiAppSettings
        readonly IAppSettingsService? appSettings;
#endif
        #endregion

        #region Settings
        [ObservableProperty]
        public partial bool IsLoading { get; set; } = false;

        [ObservableProperty]
        public partial string HashKey { get; set; } = App.Hash;

        [ObservableProperty]
        public partial string LicenseInfo { get; set; } = string.Empty;
        partial void OnLicenseInfoChanged(string value)
        {
            if (!IsLoading)
            {
#if MauiAppSettings
                appSettings?.LicenseInfo = value;
                appSettings?.SaveSetting(setting => appSettings.LicenseInfo);
#else
                SettingsApp.LicenseInfo = value;
                SettingsApp.SaveSetting(setting => SettingsApp.LicenseInfo);
#endif
            }
        }

        [ObservableProperty]
        public partial ObservableCollection<SettingsItem> Settings { get; set; } = [];
#endregion

        #region Ctor
#if MauiAppSettings
        public MainPageViewModel(IAppSettingsService appSettings)
        {
            this.appSettings = appSettings;
            LoadSettings();
        }
#else
        public MainPageViewModel()
        {
            LoadSettings();
        }
#endif
#endregion

        #region Methods
        void LoadSettings()
        {
            IsLoading = true;

#if MauiAppSettings
            if (appSettings is not null)
            {
                LicenseInfo = appSettings.LicenseInfo;
            }
#else
            LicenseInfo = SettingsApp.LicenseInfo;
#endif
            IsLoading = false;
        }
#endregion

        #region Commands
        [RelayCommand]       
#if MauiAppSettings
        Task SaveSettings() => appSettings!.SaveSettingsAsync(AppSourceGenerationContext.Default, key: App.Hash);
#else
        static Task SaveSettings() => SettingsApp.SaveSettingsAsync(AppSourceGenerationContext.Default, key: App.Hash);
#endif

        [RelayCommand]
        async Task LoadSettingsFromDevice()
        {
            try
            {

#if MauiAppSettings
                await appSettings!.LoadSettingsAsync(AppSourceGenerationContext.Default, key: App.Hash);
#else
                await SettingsApp.LoadSettingsAsync(AppSourceGenerationContext.Default, key: App.Hash);
#endif
                LoadSettings();
            }
            catch (Exception)
            {
                // Throus if the key missmatches
            }
        }

        [RelayCommand]
        async Task ExchangeHashKey()
        {
            string newKey = EncryptionManager.GenerateBase64Key();
#if MauiAppSettings
            await appSettings!.ExhangeKeyAsync(newKey, AppSourceGenerationContext.Default, oldKey: App.Hash);
#else
            await SettingsApp.ExhangeKeyAsync(newKey, AppSourceGenerationContext.Default, oldKey: App.Hash);
#endif
            App.Hash = HashKey = newKey;
            LoadSettings();
        }

        [RelayCommand]
        async Task ToDictionary()
        {
            // All "SkipForExport" should be missing here.
#if MauiAppSettings
            Dictionary<string, Tuple<object?, Type>> dict = await appSettings!.ToDictionaryAsync(AppSourceGenerationContext.Default);
#else
            Dictionary<string, Tuple<object?, Type>> dict = await SettingsApp.ToDictionaryAsync(AppSourceGenerationContext.Default);
#endif
            Settings = [.. dict.Select(kp => new SettingsItem() { Key = kp.Key, Value = kp.Value?.Item1?.ToString() ?? string.Empty })];
        }
        #endregion

        #region LifeCycle

        public async void Pages_Loaded(object? sender, EventArgs e)
        {
            await LoadSettingsFromDevice();
        }
        #endregion
    }
}
