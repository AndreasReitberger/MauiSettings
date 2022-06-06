namespace AndreasReitberger.Maui.Helper
{
    internal class MauiSettingsInfo
    {
        #region Properties
        public string Name { get; set; }
        public object Value { get; set; }
        public Type SettingsType { get; set; }
        public object Default { get; set; }
        #endregion
    }
}
