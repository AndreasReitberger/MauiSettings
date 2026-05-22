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
    public partial class MauiSettings<T> : MauiSettingsGeneric<T> where T : new()
    {
        #region Ctors
        public MauiSettings() : base() { }
        public MauiSettings(IDispatcher dispatcher) : base(dispatcher) { }
        #endregion

        #region DeviceSettings
        /// <summary>
        /// Opens the settings UI on the current device
        /// </summary>
        public static void OpenDeviceSettings() => AppInfo.ShowSettingsUI();
        #endregion
    }
}