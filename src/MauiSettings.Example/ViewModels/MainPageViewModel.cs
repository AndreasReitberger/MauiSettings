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
        void SaveSettings()
        {
            SettingsApp.SaveSettings(App.Hash);
        }

        [RelayCommand]
        void ToDictionary()
        {
            // All "SkipForExport" should be missing here.
            var dict = SettingsApp.ToDictionaryAsync();
        }
        #endregion
    }
}
