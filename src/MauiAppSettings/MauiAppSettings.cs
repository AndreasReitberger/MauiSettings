using AndreasReitberger.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AndreasReitberger.Maui
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    // All the code in this file is included in all platforms.
    //public partial class MauiAppSettings<T> : MauiAppSettingsService<T>, IMauiAppSettings<T> where T : new()
    public partial class MauiAppSettings<T> : MauiAppSettingsService, IMauiAppSettings<T>
    {
        #region Properties

        [ObservableProperty]
        public partial bool SettingsChanged { get; set; } = false;
        #endregion

        #region Ctors
        public MauiAppSettings() : base() { }
        public MauiAppSettings(IDispatcher dispatcher) : base(dispatcher) { }
        #endregion

        #region DeviceSettings
        /// <summary>
        /// Opens the settings UI on the current device
        /// </summary>
        public void OpenDeviceSettings() => AppInfo.ShowSettingsUI();
        #endregion
    }
}