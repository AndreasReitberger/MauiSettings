using AndreasReitberger.Shared.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSettings.Example.Models.Settings;
using System.Collections.ObjectModel;

namespace MauiSettings.Example.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        #region Settings
        [ObservableProperty]
        bool isLoading = false;

        [ObservableProperty]
        string currentVersionAvailable = string.Empty;
        #endregion

        #region Ctor
        public SettingsPageViewModel()
        {
            LoadSettings();
        }
        #endregion

        #region Methods
        void LoadSettings()
        {
            IsLoading = true;

            CurrentVersionAvailable = SettingsApp.ResourcesCurrentVersionAvailable;

            IsLoading = false;
        }
        #endregion

        #region Commands
        [RelayCommand]
        void SaveSettings()
        {
            SettingsApp.ResourcesCurrentVersionAvailable = CurrentVersionAvailable;
            SettingsApp.SaveSettings();
        }

        [RelayCommand]
        void LoadSettingsFromDevice()
        {
            try
            {
                SettingsApp.LoadSettings();
                LoadSettings();
            }
            catch (Exception)
            {
                // Throus if the key missmatches
            }
        }
        #endregion
    }
}
