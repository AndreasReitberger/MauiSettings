namespace AndreasReitberger.Maui.Attributes
{
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
