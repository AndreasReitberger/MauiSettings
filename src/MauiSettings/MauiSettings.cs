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
    public class MauiSettings<T> : MauiSettingsGeneric<T> where T : new()
    {
        public MauiSettings() { }
        //public MauiSettings(string key) : base(key) { }
        //public MauiSettings(T settingsObject, string key) : base(settingsObject, key) { }
    }
}