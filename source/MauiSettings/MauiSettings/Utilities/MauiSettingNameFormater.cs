using AndreasReitberger.Maui.Attributes;
using System.Reflection;

namespace AndreasReitberger.Maui.Utilities
{
    internal class MauiSettingNameFormater
    {
        #region Variables
        public const string Version = "v1";
        public const string NamePrefix = "AR";
        #endregion
        public static string GetFullSettingName(Type settingsType, MemberInfo mi, MauiSettingBaseAttribute baseSettingAttr)
        {
            string name;
            if (string.IsNullOrEmpty(baseSettingAttr?.Name))
            {
                name = mi.Name;
            }
            else
            {
                name = baseSettingAttr.Name;
                settingsType = null;
            }
            return $"{NamePrefix}_{Version}_{name}{(settingsType == null ? "" : $"_{settingsType}")}";
        }
    }
}
