using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
#if MauiAppSettings
using MauiSettings.Example.Interfaces;
#else
using MauiSettings.Example.Models.Settings;
#endif

namespace MauiSettings.Example.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
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
        public partial bool UseCustomSharedName { get; set; } = false;
        partial void OnUseCustomSharedNameChanged(bool value)
        {
            if (!value)
                SharedName = null;
        }

        [ObservableProperty]
        public partial string? SharedName { get; set; } = null;

        [ObservableProperty]
        public partial string CurrentVersionAvailable { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SomeTextValue { get; set; } = "Example text incoming...";

        [ObservableProperty]
        public partial int SomeIntValue { get; set; } = 93216;

        [ObservableProperty]
        public partial double SomeDoubleValue { get; set; } = 2651.65;

        [ObservableProperty]
        public partial bool SomeBoolValue { get; set; } = true;
        #endregion

        #region Ctor
#if MauiAppSettings
        public SettingsPageViewModel(IAppSettingsService appSettings)
        {
            this.appSettings = appSettings;
            LoadSettings();
        }
#else
        public SettingsPageViewModel()
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
                CurrentVersionAvailable = appSettings.ResourcesCurrentVersionAvailable;
                SomeBoolValue = appSettings.Misc_Boolean;
                SomeDoubleValue = appSettings.Misc_Numeric;
                SomeTextValue = appSettings.Misc_String;
                SomeIntValue = appSettings.Misc_Counter;
            }
#else
            CurrentVersionAvailable = SettingsApp.ResourcesCurrentVersionAvailable;
            SomeBoolValue = SettingsApp.Misc_Boolean;
            SomeDoubleValue = SettingsApp.Misc_Numeric;
            SomeTextValue = SettingsApp.Misc_String;
            SomeIntValue = SettingsApp.Misc_Counter;
#endif

            IsLoading = false;
        }
        #endregion

        #region Commands
        [RelayCommand]
        async Task SaveSettings()
        {
#if MauiAppSettings
            appSettings!.ResourcesCurrentVersionAvailable = CurrentVersionAvailable;
            appSettings!.Misc_Boolean = SomeBoolValue;
            appSettings!.Misc_Numeric = SomeDoubleValue;
            appSettings!.Misc_String = SomeTextValue;
            appSettings!.Misc_Counter = SomeIntValue;
            appSettings!.SaveSettings(AppSourceGenerationContext.Default, SharedName);
#else
            SettingsApp.ResourcesCurrentVersionAvailable = CurrentVersionAvailable;
            SettingsApp.Misc_Boolean = SomeBoolValue;
            SettingsApp.Misc_Numeric = SomeDoubleValue;
            SettingsApp.Misc_String = SomeTextValue;
            SettingsApp.Misc_Counter = SomeIntValue;
            SettingsApp.SaveSettings(AppSourceGenerationContext.Default, SharedName);
#endif
            await Shell.Current.DisplayAlertAsync("Settings saved", "Settings saved successfully...", "OK");
        }

        [RelayCommand]
        async Task LoadSettingsFromDevice()
        {
            try
            {
#if MauiAppSettings
                appSettings!.LoadSettings(AppSourceGenerationContext.Default, SharedName);
#else
                SettingsApp.LoadSettings(AppSourceGenerationContext.Default, SharedName);
#endif
                LoadSettings();
                await Shell.Current.DisplayAlertAsync("Settings loaded", "Settings loaded successfully...", "OK");
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
