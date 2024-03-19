namespace AndreasReitberger.Maui.Helper
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */
    internal class MauiSettingsInfo
    {
        #region Properties
        public string Name { get; set; }
        public object Value { get; set; }
        public Type SettingsType { get; set; }
        public object Default { get; set; }
        public bool IsSecure { get; set; } = false;
        public bool Encrypt { get; set; } = false;
        public bool SkipForExport { get; set; } = false;
        #endregion
    }
}
