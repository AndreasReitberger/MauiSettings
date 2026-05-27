using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

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
    public partial class MauiAppSettingsMemberInfo : ObservableObject
#else
    public partial class MauiSettingsMemberInfo : ObservableObject
#endif
    {
        #region Properties
        [ObservableProperty]
        public partial object? OrignalSettingsObject { get; set; }
        [ObservableProperty]
        public partial MemberInfo? Info { get; set; }
        [ObservableProperty]
        public partial Type? SettingsType { get; set; }
        #endregion
    }
}
