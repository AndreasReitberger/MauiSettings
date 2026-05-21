using AndreasReitberger.Maui.Interfaces;

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
    public partial class MauiAppSettings<T> : MauiAppSettingsService<T>, IMauiAppSettings<T> where T : class, new()
    {
        public MauiAppSettings() : base() { }
        public MauiAppSettings(IDispatcher dispatcher) : base(dispatcher) { }
    }
}