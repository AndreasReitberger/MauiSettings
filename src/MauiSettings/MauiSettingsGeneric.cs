using AndreasReitberger.Maui.Attributes;
#if IOS
using AndreasReitberger.Maui.Cloud;
#endif
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Utilities;
using System.Linq.Expressions;
using System.Reflection;

namespace AndreasReitberger.Maui
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    public partial class MauiSettingsGeneric<SO> where SO : new()
    {
        #region Settings Object

        static object _settingsObject;
        public static SO SettingsObject
        {
            get
            {
                if (_settingsObject == null)
                {
                    _settingsObject = new SO();
                }
                return (SO)_settingsObject;
            }
        }
        #endregion

        #region Variables
        static readonly object lockObject = new();
        #endregion

        #region Constructor
        public MauiSettingsGeneric() { }
        public MauiSettingsGeneric(SO settingsObject)
        {
            _settingsObject = settingsObject;
        }
        #endregion

        #region Methods

        #region LoadSettings
        public static void LoadSettings()
        {
            LoadSettings(settings: SettingsObject);
        }
        public static void LoadSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectSetting(SettingsObject, value);
        }
        public void LoadObjectSettings()
        {
            LoadSettings(this);
        }
        public static void LoadObjectSetting<T>(object settingsObject, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings: settingsObject, value, MauiSettingsActions.Load);
        }
        public static void LoadSettings(object settings)
        {
            GetClassMeta(settings: settings, mode: MauiSettingsActions.Load);
        }
        public static async Task LoadSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task LoadSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load);
            });
        }
        #endregion

        #region SaveSettings
        public static void SaveSettings()
        {
            SaveSettings(SettingsObject);
        }
        public static void SaveSetting<T>(Expression<Func<SO, T>> value)
        {
            SaveObjectSetting(SettingsObject, value);
        }
        public static void SaveObjectSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, MauiSettingsActions.Save);
        }
        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value)
        {
            SaveObjectSetting(this, value);
        }
        public void SaveObjectSettings()
        {
            SaveSettings(this);
        }
        public static void SaveSettings(object settings)
        {
            GetClassMeta(settings, MauiSettingsActions.Save);
        }
        public static async Task SaveSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await SaveSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task SaveSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save);
            });
        }
        #endregion

        #region DeleteSettings
        public static void DeleteSettings()
        {
            DeleteSettings(SettingsObject);
        }
        public static void DeleteSetting<T>(Expression<Func<SO, T>> value)
        {
            DeleteObjectSetting(SettingsObject, value);
        }
        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value)
        {
            DeleteObjectSetting(this, value);
        }
        public static void DeleteObjectSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, MauiSettingsActions.Delete);
        }
        public void DeleteObjectSettings()
        {
            DeleteSettings(this);
        }
        public static void DeleteSettings(object settings)
        {
            GetClassMeta(settings, MauiSettingsActions.Delete);
        }
        #endregion

        #region LoadDefaults
        public static void LoadDefaultSettings()
        {
            LoadDefaultSettings(SettingsObject);
        }
        public static void LoadDefaultSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectDefaultSetting(SettingsObject, value);
        }

        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectDefaultSetting(this, value);
        }

        public static void LoadObjectDefaultSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, MauiSettingsActions.LoadDefaults);
        }
        public void LoadObjectDefaultSettings()
        {
            LoadDefaultSettings(this);
        }
        public static void LoadDefaultSettings(object settings)
        {
            GetClassMeta(settings, MauiSettingsActions.LoadDefaults);
        }
        #endregion

        #region Private
        static void GetClassMeta(object settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;
                MauiSettingsMemberInfo memberInfo = new();

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = ProcessSettingsInfo(settingsObjectInfo, settingsInfo, mode, target);
                }
            }
        }
        static async Task GetClassMetaAsync(object settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            //lock (lockObject)
            if (true)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;
                MauiSettingsMemberInfo memberInfo = new();

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target);
                }
            }
        }

        static void GetExpressionMeta<T>(object settings, Expression<Func<SO, T>> value, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            lock (lockObject)
            {
                if (value.Body is MemberExpression memberExpression)
                {
                    _ = ProcessSettingsInfo(new MauiSettingsMemberInfo()
                    {
                        OrignalSettingsObject = settings,
                        Info = memberExpression.Member,

                    }, new MauiSettingsInfo(), mode, target);
                }
            }
        }

        static bool ProcessSettingsInfo(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target)
        {
            settingsInfo ??= new();
            MauiSettingBaseAttribute settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiSettingAttribute> settingBaseAttributes
                    = settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)
                    .ToList();
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return false;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null)
            {
                settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                //Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                switch (target)
                {
#if IOS
                    case MauiSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                    case MauiSettingsTarget.Local:
                    default:
                        settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                        break;

                }
                //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
            }
            bool? secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (secure ?? false)
                {
#if IOS
                    switch (target)
                    {
                        case MauiSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                        case MauiSettingsTarget.Local:
                        default:
                            throw new NotSupportedException("SecureStorage is only available in the Async methods!");

                    }
#else
                    throw new NotSupportedException("SecureStorage is only available in the Async methods!");
#endif
                }
            }

            switch (mode)
            {
                case MauiSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                    }
                    // Sets the loaded value back to the settingsObject
                    MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case MauiSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                case MauiSettingsActions.Delete:
                    object fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            if (settingsInfo.Value != null)
                            {
                                // If there is a default value, do not delete the key. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the key from the cloud storage
                                ICloudStoreManager.DeleteValue(settingsInfo.Name);
                            }
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                case MauiSettingsActions.LoadDefaults:
                    object defaulSettingtValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        static async Task<bool> ProcessSettingsInfoAsync(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target)
        {
            settingsInfo ??= new();
            MauiSettingBaseAttribute settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiSettingAttribute> settingBaseAttributes
                    = settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)
                    .ToList();
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return false;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null)
            {
                settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                //Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
            }
            bool secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (!secure)
                {
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                            break;
                    }
                    //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            //break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            settingsInfo.Value = await MauiSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                            break;
                    }
                }
                else
                {
#if IOS
                    switch (target)
                    {
                        case MauiSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                        case MauiSettingsTarget.Local:
                        default:
                            throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");

                    }
#else
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
#endif
                }
            }

            switch (mode)
            {
                case MauiSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                    }
                    // Sets the loaded value back to the settingsObject
                    MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case MauiSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                case MauiSettingsActions.Delete:
                    object fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            if (settingsInfo.Value != null)
                            {
                                // If there is a default value, do not delete the key. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the key from the cloud storage
                                ICloudStoreManager.DeleteValue(settingsInfo.Name);
                            }
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                case MauiSettingsActions.LoadDefaults:
                    object defaulSettingtValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
        #endregion

        #endregion
    }
}
