namespace AndreasReitberger.Maui.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MauiSettingAttribute : MauiSettingBaseAttribute
    {
        #region Properties
        public bool Secure { get; set; } = false;
        #endregion
    }
}
