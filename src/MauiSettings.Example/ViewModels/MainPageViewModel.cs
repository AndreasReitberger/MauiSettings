using AndreasReitberger.Shared.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSettings.Example.Models.Settings;
using System.Collections.ObjectModel;

namespace MauiSettings.Example.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
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
                SettingsApp.LicenseInfo = value;
                SettingsApp.SettingsChanged = true;
            }
        }

        [ObservableProperty]
        public partial ObservableCollection<SettingsItem> Settings { get; set; } = [];
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
        static async Task SaveSettings() => await SettingsApp.SaveSettingsAsync(key: App.Hash);

        [RelayCommand]
        async Task LoadSettingsFromDevice()
        {
            try
            {
                await SettingsApp.LoadSettingsAsync(key: App.Hash);
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
            await SettingsApp.ExhangeKeyAsync(oldKey: App.Hash, newKey: newKey);
            App.Hash = HashKey = newKey;
            LoadSettings();
        }

        [RelayCommand]
        async Task ToDictionary()
        {
            // All "SkipForExport" should be missing here.
            Dictionary<string, Tuple<object?, Type>> dict = await SettingsApp.ToDictionaryAsync();
            Settings = [.. dict.Select(kp => new SettingsItem() { Key = kp.Key, Value = kp.Value?.Item1?.ToString() ?? string.Empty })];
        }
        #endregion
    }
}
