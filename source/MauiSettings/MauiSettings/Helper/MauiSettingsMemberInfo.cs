using System.Reflection;

namespace AndreasReitberger.Maui.Helper
{
    internal class MauiSettingsMemberInfo
    {
        #region Properties
        public object OrignalSettingsObject { get; set; }
        public MemberInfo Info { get; set; }
        public Type SettingsType { get; set; }
        #endregion
    }
}
