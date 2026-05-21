using CommunityToolkit.Mvvm.ComponentModel;

namespace AndreasReitberger.Maui.Helper
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */
#if MauiAppSettings
    public partial class MauiAppSettingsInfo : ObservableObject
#else
    public partial class MauiSettingsInfo : ObservableObject
#endif
    {
        #region Properties
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;
        [ObservableProperty]
        public partial object? Value { get; set; }
        [ObservableProperty]
        public partial Type? SettingsType { get; set; }
        [ObservableProperty]
        public partial object? Default { get; set; }
        [ObservableProperty]
        public partial bool IsSecure { get; set; } = false;
        [ObservableProperty]
        public partial bool Encrypt { get; set; } = false;
        [ObservableProperty]
        public partial bool SkipForExport { get; set; } = false;
        #endregion
    }
}
