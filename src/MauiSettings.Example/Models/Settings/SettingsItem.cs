using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiSettings.Example.Models.Settings
{
    public partial class SettingsItem : ObservableObject
    {
        [ObservableProperty]
        public partial string Key { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Value { get; set; } = string.Empty;
    }
}
