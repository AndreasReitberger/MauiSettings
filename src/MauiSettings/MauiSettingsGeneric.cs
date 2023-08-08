using AndreasReitberger.Maui.Attributes;
#if IOS
using AndreasReitberger.Maui.Cloud;
#endif
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Utilities;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        public static async Task LoadSettingAsync<T>(Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await LoadObjectSettingAsync(SettingsObject, value);
            });
        }
        public void LoadObjectSettings()
        {
            LoadSettings(this);
        }
        public static void LoadObjectSetting<T>(object settingsObject, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings: settingsObject, value, MauiSettingsActions.Load);
        }
        public static async Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await GetExpressionMetaAsync(settings: settingsObject, value, MauiSettingsActions.Load);
            });
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
        public static async Task LoadSecureSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await LoadSecureSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task LoadSecureSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, secureOnly: true);
            });
        }
        public static async Task LoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, bool save = true)
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, save: save);
            });
        }
        public static async Task LoadSettingsAsync(string key, Tuple<object, Type> data, bool save = true)
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: new() { { key, data} }, save: save);
            });
        }
        public static async Task LoadSettingsAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, bool save = true)
        {
            await Task.Run(async delegate
            {
                await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: MauiSettingsActions.Load, secureOnly: false);
                // Save the restored settings right away
                if (save) await SaveSettingsAsync(settings: settings);
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
        public static async Task SaveSecureSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await SaveSecureSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task SaveSecureSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save, secureOnly: true);
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

        #region Conversion

        public static async Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync()
        {
            return await ToDictionaryAsync(settings: SettingsObject);
        }
        public static async Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync(object settings)
        {
            if (true)
            {
                Dictionary<string, Tuple<object, Type>> setting = new();
                //List<MemberInfo> members = GetClassMetaAsList(settings);

                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    MauiSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo);
                    if (settingsPair != null)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                    }
                }
                /*
                members?.ForEach(member =>
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = member;
                    var settingsPair = ProcessSettingsInfoAsKeyValuePair(settingsObjectInfo, settingsInfo);
                    if (settingsPair != null)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                    }
                });
                */
            return setting;
            }
        }

        public static async Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync()
        {
            return await ToConcurrentDictionaryAsync(settings: SettingsObject);
        }
        public static async Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync(object settings)
        {
            ConcurrentDictionary<string, Tuple<object, Type>> setting = new();
            List<MemberInfo> members = GetClassMetaAsList(settings);

            MauiSettingsMemberInfo settingsObjectInfo = new();
            MauiSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in members)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo);
                if (settingsPair != null)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                }
            }
            /*
            members?.ForEach(member =>
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = member;
                var settingsPair = ProcessSettingsInfoAsKeyValuePair(settingsObjectInfo, settingsInfo);
                if(settingsPair != null)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                }
            });
            */
            return setting;
        }

        public static async Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value)
        {
            return await ToSettingsTupleAsync(settings: SettingsObject, value: value);
        }

        public static async Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(object settings, Expression<Func<SO, T>> value)
        {
            MauiSettingsInfo info = await GetExpressionMetaAsKeyValuePairAsync(settings: settings, value: value);
            return new(info.Name, new(info.Value, info.SettingsType));
        }
        #endregion

        #region Private
        static List<MemberInfo> GetClassMetaAsList(object settings)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();
                return declaredMembers?.ToList();
            }
        }
        static void GetClassMeta(object settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

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
        static async Task GetClassMetaAsync(object settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local, bool secureOnly = false)
        {
            //lock (lockObject)
            if (true)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly);
                }
            }
        }
        static async Task GetMetaFromDictionaryAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local, bool secureOnly = false)
        {
            //lock (lockObject)
            if (true)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    bool useValueFromSettingsInfo = false;
                    // Try to find the matching key
                    KeyValuePair<string, Tuple<object, Type>>? keyPair = dictionary?.FirstOrDefault(keypair => 
                        keypair.Key.EndsWith(mInfo.Name
                        //?.Replace("get_", string.Empty)
                        ));
                    if (keyPair?.Key != null)
                    {
                        useValueFromSettingsInfo = true;
                        // If a matching key was found, prepare the settingsInfo with the loaded data
                        settingsInfo = new()
                        {
                            Name = mInfo.Name?.Replace("get_", string.Empty),
                            Value = keyPair.Value.Value.Item1,
                            SettingsType = keyPair.Value.Value.Item2,
                        };
                    }
                    else
                        useValueFromSettingsInfo = false;
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, useValueFromSettingsInfo: useValueFromSettingsInfo);
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

        static async Task GetExpressionMetaAsync<T>(object settings, Expression<Func<SO, T>> value, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {

            if (value.Body is MemberExpression memberExpression)
            {
                _ = await ProcessSettingsInfoAsync(new MauiSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiSettingsInfo(), mode, target);
            }        
        }

        static async Task<MauiSettingsInfo> GetExpressionMetaAsKeyValuePairAsync<T>(object settings, Expression<Func<SO, T>> value)
        {
            if (value.Body is MemberExpression memberExpression)
            {
                return await ProcessSettingsInfoAsKeyValuePairAsync(new MauiSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiSettingsInfo());
            }
            return new();
        }

        static bool ProcessSettingsInfo(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target, bool throwOnError = false)
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
                            if (throwOnError) throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            else break;
                        case MauiSettingsTarget.Local:
                        default:
                            if (throwOnError) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
                            else break;

                    }
#else
                    if (throwOnError) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
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

        static async Task<bool> ProcessSettingsInfoAsync(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target, bool secureOnly = false, bool useValueFromSettingsInfo = false)
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
            }
            bool secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly) 
                        return true;
                    // If the value is not used from the passed settingsInfo, load it

                    switch (target)
                    {
#if IOS
                    case MauiSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                        case MauiSettingsTarget.Local:
                        default:
                            if (!useValueFromSettingsInfo)
                                settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                            else
                                settingsInfo.Value = MauiSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
                            break;
                    }
                    
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
                            if (!useValueFromSettingsInfo)
                                settingsInfo.Value = await MauiSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                            else
                                settingsInfo.Value = MauiSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
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
                    /* => Calling SaveSettingsAsync() after loading
                    if(useValueFromSettingsInfo)
                    {
                        // If the value is used from the dictionary, save it.
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
                    }*/
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

        static async Task<MauiSettingsInfo> ProcessSettingsInfoAsKeyValuePairAsync(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, bool secureOnly = false)
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
                    return null;
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
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                bool secure = settingAttribute.Secure;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly)
                        return null;
                    settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    settingsInfo.Value = await MauiSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                }
                else
                {
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                }
            }
            // Sets the loaded value back to the settingsObject
            //MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
            return settingsInfo;
        }

        #endregion

        #endregion
    }
}
