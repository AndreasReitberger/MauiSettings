using AndreasReitberger.Maui.Attributes;
using System.Reflection;

namespace AndreasReitberger.Maui.Helper
{
    internal class MauiSettingsObjectHelper
    {
        public static object GetSettingValue(MemberInfo mi, object o)
        {
            if (mi is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(o);
            }
            else if (mi is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(o);
            }
            return (mi as PropertyInfo)?.GetMethod.Invoke(o, new object[0]);
        }

        public static void SetSettingValue(MemberInfo memberInfo, object settings, object settingValue, Type settingType)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(settings, settingValue);
                return;
            }

            if (memberInfo is PropertyInfo propertyInfo)
            {
                MethodInfo setMethod = propertyInfo.SetMethod;
                if (setMethod is null)
                {
                    throw new NullReferenceException($"MauiSettings: Cannot set {memberInfo.Name} property! (Read only)");
                }
                // If the settings value type doesn't match the target type of the field.
                // Maui saves the settings as string, so this conversion is needed.
                if (settingValue.GetType() != settingType)
                {
                    settingValue = GetConvertedTypeValue(settingValue, settingType);
                }

                setMethod.Invoke(settings, new object[1] { settingValue });
                return;
            }
            throw new NotSupportedException($"MauiSettings: The type '{memberInfo.GetType()}' is not supported for the field: {memberInfo.Name}");
        }

        public static Type GetSettingType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }
            throw new NotSupportedException($"MauiSettings: The type '{memberInfo.GetType()}' is not supported for the field: {memberInfo.Name}");
        }

        public static object GetTypeDefaultValue(Type type)
        {
            if (type is not null && type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static object GetDefaultValue(MauiSettingBaseAttribute attr, Type settingType)
        {
            try
            {
                if (attr != null && attr.DefaultValueInUse)
                {
                    object obj = attr.DefaultValue;
                    if (obj?.GetType() != settingType)
                    {
                        if(obj.GetType() == typeof(string))
                        {
                            // Try to pass the string object for the constructor
                            //return Activator.CreateInstance(settingType, new string[] { obj as string });
                            return GetConvertedTypeValue(obj, settingType);
                        }
                    }
                    return obj;
                }

                return GetTypeDefaultValue(settingType);
            }
            catch (Exception exc)
            {
                return GetTypeDefaultValue(settingType);
            }
        }

        public static object GetConvertedTypeValue(object setting, Type settingsType)
        {
            if (settingsType is not null)
            {
                try
                {
                    if (setting?.GetType() == settingsType)
                        return setting;
                    //
                    return setting.GetType() == typeof(string)
                        ? Activator.CreateInstance(settingsType, new string[] { setting as string })
                        : Convert.ChangeType(setting, settingsType);
                }
                catch(Exception)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
