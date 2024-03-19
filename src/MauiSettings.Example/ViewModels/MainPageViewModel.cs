using AndreasReitberger.Shared.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSettings.Example.Models.Settings;

namespace MauiSettings.Example.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region Settings
        [ObservableProperty]
        bool isLoading = false;

        [ObservableProperty]
        string hashKey = App.Hash;

        [ObservableProperty]
        string licenseInfo = string.Empty;
        partial void OnLicenseInfoChanged(string value)
        {
            if (!IsLoading)
            {
                SettingsApp.LicenseInfo = value;
                SettingsApp.SettingsChanged = true;
            }
        }

        #endregion

        #region Ctor
        public MainPageViewModel()
        {
            LoadSettings();
        }
        #endregion

        #region Methods
        void LoadSettings()
        {
            IsLoading = true;

            LicenseInfo = SettingsApp.LicenseInfo;

            IsLoading = false;
        }
        #endregion

        #region Commands
        [RelayCommand]
        async Task SaveSettings() => await SettingsApp.SaveSettingsAsync(key: App.Hash);

        [RelayCommand]
        async Task LoadSettingsFromDevice()
        {
            await SettingsApp.LoadSettingsAsync(key: App.Hash);
            LoadSettings();
        }

        [RelayCommand]
        async Task ExchangeHashKey()
        {
            string newKey = EncryptionManager.GenerateBase64Key();
            await SettingsApp.ExhangeKeyAsync(oldKey: App.Hash, newKey: newKey);
            App.Hash = HashKey = newKey;
            LoadSettings();
        }

        [RelayCommand]
        async Task ToDictionary()
        {
            // All "SkipForExport" should be missing here.
            var dict = await SettingsApp.ToDictionaryAsync();
        }
        #endregion
    }
}
