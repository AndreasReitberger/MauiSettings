﻿using AndreasReitberger.Maui.Attributes;
#if IOS
using AndreasReitberger.Maui.Cloud;
#endif
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Events;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Utilities;
using AndreasReitberger.Shared.Core.Utilities;
using System.Collections.Concurrent;
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

        static object? _settingsObject;
        public static SO SettingsObject
        {
            get
            {
                _settingsObject ??= new SO();
                return (SO)_settingsObject;
            }
        }
        #endregion

        #region Variables

        static readonly Lock lockObject = new();
        #endregion

        #region Properties
        public static bool ThrowIfSettingsObjectIsNull { get; set; } = false;
        #endregion

        #region Constructor
        public MauiSettingsGeneric() { }
        public MauiSettingsGeneric(SO settingsObject)
        {
            _settingsObject = settingsObject;
        }
        /*
        public MauiSettingsGeneric(string settingsKey)
        {
            _passPhrase = settingsKey;
        }
        public MauiSettingsGeneric(SO settingsObject, string settingsKey)
        {
            _settingsObject = settingsObject;
            _passPhrase = settingsKey;
        }
        */
        #endregion

        #region Methods

        #region LoadSettings
        public static void LoadSettings() => LoadSettings(settings: SettingsObject);

        public static void LoadSetting<T>(Expression<Func<SO, T>> value) => LoadObjectSetting(SettingsObject, value);

        public static Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            if (SettingsObject is null) return;
            await LoadObjectSettingAsync(SettingsObject, value, key: key);  
        });

        public static Task LoadSecureSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            if (SettingsObject is null) return;
            await LoadSecureObjectSettingAsync(SettingsObject, value, key: key);
        });

        public void LoadObjectSettings() => LoadSettings(this);

        public static void LoadObjectSetting<T>(object? settingsObject, Expression<Func<SO, T>> value)
            => GetExpressionMeta(settings: settingsObject, value, MauiSettingsActions.Load);

        public static Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, MauiSettingsActions.Load, key: key);
        });
        public static Task LoadSecureObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, MauiSettingsActions.Load, secureOnly: true, key: key);
        });

        public static void LoadSettings(object? settings) => GetClassMeta(settings: settings, mode: MauiSettingsActions.Load);

        public static Task LoadSettingsAsync(string? key = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, key: key);
            });

        public static Task LoadSettingsAsync(object? settings, string? key = null)
            => Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, key: key);
            });

        public static Task<bool> TryLoadSettingsAsync(string? key = null, bool justTryLoading = true)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, key: key, justTryLoading: justTryLoading);
            });

        public static Task<bool> TryLoadSettingsAsync(object? settings, string? key = null, bool justTryLoading = true)
            => Task.Run(async delegate
            {
                return await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, key: key, justTryLoading: justTryLoading);
            });

        public static Task LoadSecureSettingsAsync(string? key = null)
            => Task.Run(async delegate
            {
                await LoadSecureSettingsAsync(settings: SettingsObject, key: key);
            });

        public static Task LoadSecureSettingsAsync(object? settings, string? key = null)
            => Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, secureOnly: true, key: key);
            });

        public static Task LoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, bool save = true, string? key = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, save: save, key: key);
            });

        public static Task<bool> TryLoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, string? key = null, bool justTryLoading = true)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, key: key, justTryLoading: justTryLoading);
            });

        public static Task LoadSettingsAsync(string settingsKey, Tuple<object, Type> data, bool save = true, string? key = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, save: save, key: key);
            });

        public static Task<bool> TryLoadSettingsAsync(string settingsKey, Tuple<object, Type> data, string? key = null, bool justTryLoading = true)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, key: key, justTryLoading: justTryLoading);
            });

        public static Task LoadSettingsAsync(object? settings, Dictionary<string, Tuple<object, Type>> dictionary, bool save = true, string? key = null)
            => Task.Run(async delegate
            {
                await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: MauiSettingsActions.Load, secureOnly: false, key: key);
                // Save the restored settings right away
                if (save) await SaveSettingsAsync(settings: settings, key: key);
            });
        public static Task<bool> TryLoadSettingsAsync(object? settings, Dictionary<string, Tuple<object, Type>> dictionary, string? key = null, bool justTryLoading = true)
            => Task.Run(async delegate
            {
                return await GetMetaFromDictionaryAsync(
                    settings: settings, dictionary: dictionary, mode: MauiSettingsActions.Load, secureOnly: false, key: key, justTryLoading: justTryLoading);
            });

        #endregion

        #region SaveSettings
        public static void SaveSettings() => SaveSettings(SettingsObject);

        public static void SaveSetting<T>(Expression<Func<SO, T>> value) => SaveObjectSetting(SettingsObject, value);

        public static void SaveObjectSetting<T>(object? settings, Expression<Func<SO, T>> value)
            => GetExpressionMeta(settings, value, MauiSettingsActions.Save);

        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value) => SaveObjectSetting(this, value);

        public void SaveObjectSettings() => SaveSettings(this);

        public static void SaveSettings(object? settings) => GetClassMeta(settings, MauiSettingsActions.Save);

        public static Task SaveSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await SaveSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task SaveSettingsAsync(object? settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save, key: key);
        });

        public static Task SaveSecureSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await SaveSecureSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task SaveSecureSettingsAsync(object? settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save, secureOnly: true, key: key);
        });

        #endregion

        #region DeleteSettings
        public static void DeleteSettings() => DeleteSettings(SettingsObject);

        public static void DeleteSetting<T>(Expression<Func<SO, T>> value) => DeleteObjectSetting(SettingsObject, value);

        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value) => DeleteObjectSetting(this, value);

        public static void DeleteObjectSetting<T>(object? settings, Expression<Func<SO, T>> value) => GetExpressionMeta(settings, value, MauiSettingsActions.Delete);

        public void DeleteObjectSettings() => DeleteSettings(this);

        public static void DeleteSettings(object? settings) => GetClassMeta(settings, MauiSettingsActions.Delete);

        #endregion

        #region LoadDefaults
        public static void LoadDefaultSettings() => LoadDefaultSettings(SettingsObject);

        public static void LoadDefaultSetting<T>(Expression<Func<SO, T>> value) => LoadObjectDefaultSetting(SettingsObject, value);


        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value) => LoadObjectDefaultSetting(this, value);


        public static void LoadObjectDefaultSetting<T>(object? settings, Expression<Func<SO, T>> value)
            => GetExpressionMeta(settings, value, MauiSettingsActions.LoadDefaults);

        public void LoadObjectDefaultSettings() => LoadDefaultSettings(this);

        public static void LoadDefaultSettings(object? settings) => GetClassMeta(settings, MauiSettingsActions.LoadDefaults);

        #endregion

        #region Conversion

        public static Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync()
            => ToDictionaryAsync(settings: SettingsObject);

        public static Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(bool secureOnly = false, string? key = null)
            => ToDictionaryAsync(settings: SettingsObject, secureOnly: secureOnly, key: key);

        public static async Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(object? settings, bool secureOnly = false, string? key = null)
        {
            if (true)
            {
                Dictionary<string, Tuple<object?, Type>> setting = [];
                IEnumerable<MemberInfo>? declaredMembers = settings?.GetType().GetTypeInfo().DeclaredMembers;

                MauiSettingsMemberInfo settingsObjectInfo = new();
                MauiSettingsInfo settingsInfo = new();
                if (declaredMembers is null) return setting;

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    MauiSettingsInfo? settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo, secureOnly: secureOnly, key: key, keeyEncrypted: true);
                    if (settingsPair is not null && !settingsPair.SkipForExport)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object?, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType ?? typeof(object)));
                    }
                }
                return setting;
            }
        }

        public static Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync()
            => ToConcurrentDictionaryAsync(settings: SettingsObject);
        public static Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(bool secureOnly = false, string? key = null)
            => ToConcurrentDictionaryAsync(settings: SettingsObject, secureOnly: secureOnly, key: key);

        public static async Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(object? settings, bool secureOnly = false, string? key = null)
        {
            ConcurrentDictionary<string, Tuple<object?, Type>> setting = new();
            List<MemberInfo>? members = GetClassMetaAsList(settings);

            MauiSettingsMemberInfo settingsObjectInfo = new();
            MauiSettingsInfo settingsInfo = new();
            if (members is null) return setting;

            foreach (MemberInfo mInfo in members)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiSettingsInfo? settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo, secureOnly: secureOnly, key: key, keeyEncrypted: true);
                if (settingsPair != null && !settingsPair.SkipForExport)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object?, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType ?? typeof(object)));
                }
            }
            return setting;
        }

        public static Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value, string? key = null)
            => ToSettingsTupleAsync(settings: SettingsObject, value: value, key: key);

        public static async Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(object? settings, Expression<Func<SO, T>> value, string? key = null)
        {
            MauiSettingsInfo? info = await GetExpressionMetaAsKeyValuePairAsync(settings: settings, value: value, key: key);
            if (info is not null)
                return new(info.Name, new(info.Value, info.SettingsType ?? typeof(object)));
            else
                return null;
        }
        #endregion

        #region Encryption

        public static Task ExhangeKeyAsync(string newKey, string? oldKey = null, bool reloadSettings = false)
            => Task.Run(async delegate
            {
                if (reloadSettings) await LoadSecureSettingsAsync(key: oldKey);
                await SaveSettingsAsync(key: newKey);
            });

        #endregion

        #region Private
        static List<MemberInfo>? GetClassMetaAsList(object? settings)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return null;
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo>? declaredMembers = settings?.GetType().GetTypeInfo().DeclaredMembers;
                //MauiSettingsMemberInfo settingsObjectInfo = new();
                //MauiSettingsInfo settingsInfo = new();
                return declaredMembers?.ToList();
            }
        }
        static void GetClassMeta(object? settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;
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
        static async Task<bool> GetClassMetaAsync(object? settings, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local, bool secureOnly = false, string? key = null, bool justTryLoading = false)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return false;
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            MauiSettingsMemberInfo settingsObjectInfo = new();
            MauiSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiSettingsResults result = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, key: key, justTryLoading: justTryLoading);
                if (result == MauiSettingsResults.EncryptionError || result == MauiSettingsResults.Failed) { return false; }
            }
            return true;
        }
        static async Task<bool> GetMetaFromDictionaryAsync(object? settings, Dictionary<string, Tuple<object, Type>> dictionary, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local, bool secureOnly = false, string? key = null, bool justTryLoading = false)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return false;
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            MauiSettingsMemberInfo settingsObjectInfo = new();
            MauiSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                bool useValueFromSettingsInfo = false;
                // Try to find the matching settingsKey
                KeyValuePair<string, Tuple<object, Type>>? keyPair = dictionary?.FirstOrDefault(keypair =>
                    mInfo.Name is not null && keypair.Key.EndsWith(mInfo.Name
                    //?.Replace("get_", string.Empty)
                    ));
                if (keyPair?.Key is not null && mInfo.Name is not null)
                {
                    useValueFromSettingsInfo = true;
                    // If a matching settingsKey was found, prepare the settingsInfo with the loaded data
                    settingsInfo = new()
                    {
                        Name = mInfo.Name.Replace("get_", string.Empty) ,
                        Value = keyPair.Value.Value.Item1,
                        SettingsType = keyPair.Value.Value.Item2,
                    };
                }
                else
                    useValueFromSettingsInfo = false;
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiSettingsResults result = await ProcessSettingsInfoAsync(
                    settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, useValueFromSettingsInfo: useValueFromSettingsInfo, key: key, justTryLoading: justTryLoading);
                if (result == MauiSettingsResults.EncryptionError || result == MauiSettingsResults.Failed)
                {
                    return false; 
                }
            }
            return true;
        }

        static void GetExpressionMeta<T>(object? settings, Expression<Func<SO, T>> value, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;
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

        static async Task GetExpressionMetaAsync<T>(object? settings, Expression<Func<SO, T>> value, MauiSettingsActions mode, MauiSettingsTarget target = MauiSettingsTarget.Local, bool secureOnly = false, string? key = null)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;
            if (value.Body is MemberExpression memberExpression)
            {
                _ = await ProcessSettingsInfoAsync(new MauiSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiSettingsInfo(), mode, target, secureOnly: secureOnly, key: key);
            }
        }

        static async Task<MauiSettingsInfo?> GetExpressionMetaAsKeyValuePairAsync<T>(object? settings, Expression<Func<SO, T>> value, string? key = null)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return null;
            if (value.Body is MemberExpression memberExpression)
            {
                return await ProcessSettingsInfoAsKeyValuePairAsync(new MauiSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiSettingsInfo(), key: key, keeyEncrypted: true);
            }
            return new();
        }

        static bool ProcessSettingsInfo(
            MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target,
            bool throwOnError = false, bool justTryLoading = false
            )
        {
            settingsInfo ??= new();
            MauiSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)];
                if (settingBaseAttributes.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return false;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null && settingBaseAttribute is not null)
            {
                settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                switch (target)
                {
#if IOS
                    case MauiSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                    case MauiSettingsTarget.Local:
                    default:
                        settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default);
                        break;

                }
            }
            else
            {
                return false;
            }
            bool secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                // Save the states
                settingsInfo.IsSecure = secure;
                settingsInfo.Encrypt = settingAttribute.Encrypt;
                settingsInfo.SkipForExport = settingAttribute.SkipForExport;
                if (secure)
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
                    if (throwOnError && mode != MauiSettingsActions.LoadDefaults) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
#endif
                }
                else if (settingsInfo.Encrypt)
                {
                    if (throwOnError) throw new NotSupportedException("Encryption is only available on the Async methods and if the property is marked as 'Secure'");
                }
            }
            switch (mode)
            {
                case MauiSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object? defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
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
                    object? fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
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
                                // If there is a default value, do not delete the settingsKey. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the settingsKey from the cloud storage
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
                    object? defaulSettingtValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
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

        static async Task<MauiSettingsResults> ProcessSettingsInfoAsync(
            MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target, 
            bool secureOnly = false, bool useValueFromSettingsInfo = false, string? key = null, bool keepEncrypted = false, bool justTryLoading = false
            )
        {
            settingsInfo ??= new();
            MauiSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)];
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return MauiSettingsResults.Skipped;
                }
                settingBaseAttribute = settingBaseAttributes?.FirstOrDefault();
                settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            else
            {
                return MauiSettingsResults.Skipped;
            }
            bool secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                // Save the states
                settingsInfo.IsSecure = secure;
                settingsInfo.Encrypt = settingAttribute.Encrypt;
                settingsInfo.SkipForExport = settingAttribute.SkipForExport;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly)
                        return MauiSettingsResults.Skipped;
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
                                //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                                settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default);
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
                        object? defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    }
                    if (secure && settingsInfo.Encrypt && !keepEncrypted)
                    {
                        if (string.IsNullOrEmpty(key))
                            throw new ArgumentNullException(nameof(key));
                        if (settingsInfo.Value is string secureString)
                        {
                            // Decrypt string
                            try
                            {
                                string decryptedString = EncryptionManager.DecryptStringFromBase64String(secureString, key);
                                // Throw on key missmatch
                                /*
                                if (string.IsNullOrEmpty(decryptedString) && !string.IsNullOrEmpty(secureString))
                                    throw new Exception($"The secure string is not empty, but the decrypted string is. This indicates a key missmatch!");
                                */
                                if (!justTryLoading)
                                    MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, decryptedString, settingsInfo.SettingsType);
                            }
                            catch(Exception ex)
                            {
                                OnEncryptionErrorEvent(new()
                                {
                                    Exception = ex,
                                    Key = key,
                                });
                                return MauiSettingsResults.EncryptionError;
                            }
                            break;
                        }
                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
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
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
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
                    object? fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
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
                                // If there is a default value, do not delete the settingsKey. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the settingsKey from the cloud storage
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
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
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
                    object? defaulSettingtValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
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
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
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
            return MauiSettingsResults.Success;
        }

        static async Task<MauiSettingsInfo?> ProcessSettingsInfoAsKeyValuePairAsync(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, bool secureOnly = false, string? key = null, bool keeyEncrypted = false)
        {
            settingsInfo ??= new();
            MauiSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)];
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return null;
                }
                settingBaseAttribute = settingBaseAttributes?.FirstOrDefault();
                settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                bool secure = settingAttribute.Secure;
                // Save the states
                settingsInfo.IsSecure = secure;
                settingsInfo.Encrypt = settingAttribute.Encrypt;
                settingsInfo.SkipForExport = settingAttribute.SkipForExport;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly)
                        return null;
                    //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                    settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default);
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    settingsInfo.Value = await MauiSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                    if (settingsInfo.Encrypt && !keeyEncrypted)
                    {
                        if (string.IsNullOrEmpty(key))
                            throw new ArgumentNullException(nameof(key));
                        // Decrypt string
                        if (settingsInfo.Value is string secureString)
                        {
                            try
                            {
                                string decryptedString = EncryptionManager.DecryptStringFromBase64String(secureString, key);
                                // Throw on key missmatch
                                /*
                                if (string.IsNullOrEmpty(decryptedString) && !string.IsNullOrEmpty(secureString))
                                    throw new Exception($"The secure string is not empty, but the decrypted string is. This indicates a key missmatch!");
                                */
                                settingsInfo.Value = decryptedString;
                            }
                            catch (Exception ex)
                            {
                                OnEncryptionErrorEvent(new()
                                {
                                    Exception = ex,
                                    Key = key,
                                });
                                return null;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                }
            }
            return settingsInfo;
        }

        #endregion

        #endregion

        #region Events

        public static event EventHandler? ErrorEvent;
        protected static void OnErrorEvent(ErrorEventArgs e)
        {
            ErrorEvent?.Invoke(typeof(MauiSettingsGeneric<SO>), e);
        }

        public static event EventHandler? EncryptionErrorEvent;
        protected static void OnEncryptionErrorEvent(EncryptionErrorEventArgs e)
        {
            EncryptionErrorEvent?.Invoke(typeof(MauiSettingsGeneric<SO>), e);
        }
        #endregion
    }
}
