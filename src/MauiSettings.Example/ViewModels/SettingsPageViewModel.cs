using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSettings.Example.Models.Settings;

namespace MauiSettings.Example.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        #region Settings
        [ObservableProperty]
        bool isLoading = false;

        [ObservableProperty]
        string currentVersionAvailable = string.Empty;

        [ObservableProperty]
        string someTextValue = "Example text incoming...";

        [ObservableProperty]
        int someIntValue = 93216;

        [ObservableProperty]
        double someDoubleValue = 2651.65;

        [ObservableProperty]
        bool someBoolValue = true;
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

            SomeBoolValue= SettingsApp.Misc_Boolean;
            SomeDoubleValue = SettingsApp.Misc_Numeric;
            SomeTextValue = SettingsApp.Misc_String;
            SomeIntValue = SettingsApp.Misc_Counter;

            IsLoading = false;
        }
        #endregion

        #region Commands
        [RelayCommand]
        async Task SaveSettings()
        {
            SettingsApp.ResourcesCurrentVersionAvailable = CurrentVersionAvailable;
            SettingsApp.Misc_Boolean = SomeBoolValue;
            SettingsApp.Misc_Numeric = SomeDoubleValue;
            SettingsApp.Misc_String = SomeTextValue;
            SettingsApp.Misc_Counter = SomeIntValue;

            SettingsApp.SaveSettings();

            await Shell.Current.DisplayAlert("Settings saved", "Settings saved successfully...", "OK");
        }

        [RelayCommand]
        async Task LoadSettingsFromDevice()
        {
            try
            {
                SettingsApp.LoadSettings();
                LoadSettings();
                await Shell.Current.DisplayAlert("Settings loaded", "Settings loaded successfully...", "OK");
            }
            catch (Exception)
            {
                // Throus if the key missmatches
            }
        }

        [RelayCommand]
        void ButtonCountUp()
        {
            SomeIntValue += 1;
        }

        [RelayCommand]
        void ButtonCountDown()
        {
            SomeIntValue -= 1;
        }
        #endregion
    }
}
