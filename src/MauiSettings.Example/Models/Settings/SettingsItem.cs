using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiSettings.Example.Models.Settings
{
    public partial class SettingsItem : ObservableObject
    {
        [ObservableProperty]
        string key = string.Empty;

        [ObservableProperty]
        string value = string.Empty;
    }
}
