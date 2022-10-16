namespace AndreasReitberger.Maui.Attributes
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MauiSettingBaseAttribute : Attribute
    {
        #region Properties
        public string? Name { get; set; }

        private object _default;
        public object DefaultValue
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
                DefaultValueInUse = true;
            }
        }

        internal bool DefaultValueInUse;
        #endregion
    }
}
