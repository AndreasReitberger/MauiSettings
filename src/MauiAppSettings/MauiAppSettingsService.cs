using AndreasReitberger.Maui.Attributes;
#if IOS
using AndreasReitberger.Maui.Cloud;
#endif
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Interfaces;
using AndreasReitberger.Maui.Utilities;
using AndreasReitberger.Shared.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AndreasReitberger.Maui
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    public partial class MauiAppSettingsService<SO> : ObservableObject, IMauiAppSettingsService<SO> where SO : class, new()
    {
        #region Settings Object

        [ObservableProperty]
        public partial SO SettingsObject { get; private set; } = new();
        #endregion

        #region Dispatcher
        [ObservableProperty]
        public partial IDispatcher? Dispatcher { get; set; }
        #endregion

        #region Serializer

        [ObservableProperty]
        public partial JsonSerializerContext? Context { get; set; } 
        #endregion

        #region Variables

        readonly Lock lockObject = new();
        #endregion

        #region Properties

        [ObservableProperty]
        public partial bool ThrowIfSettingsObjectIsNull { get; set; } = false;
        #endregion

        #region Constructor
        public MauiAppSettingsService() : base()
        {
            Dispatcher ??= DispatcherProvider.Current.GetForCurrentThread();
        }
        public MauiAppSettingsService(IDispatcher? dispatcher) : base()
        {
            Dispatcher = dispatcher;
        }
        public MauiAppSettingsService(SO settingsObject, IDispatcher? dispatcher) : base()
        {
            SettingsObject = settingsObject;
            Dispatcher = dispatcher;
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
        public void LoadSettings(JsonSerializerContext? context = null, string? sharedName = null) => LoadSettings(settings: SettingsObject, context, sharedName);

        public void LoadSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => LoadObjectSetting(SettingsObject, value, context, sharedName);

        public Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null) => Task.Run(async delegate
        {
            if (SettingsObject is null) return;
            await LoadObjectSettingAsync(SettingsObject, value, context, key: key, sharedName: sharedName);
        });

        public Task LoadSecureSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null) => Task.Run(async delegate
        {
            if (SettingsObject is null) return;
            await LoadSecureObjectSettingAsync(SettingsObject, value, context, key: key);
        });

        public void LoadObjectSettings(JsonSerializerContext? context = null, string? sharedName = null) => LoadSettings(this, context, sharedName);

        public void LoadObjectSetting<T>(object? settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null)
            => GetExpressionMeta(settings: settingsObject, value, MauiAppSettingsActions.Load, context, sharedName: sharedName);

        public Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, MauiAppSettingsActions.Load, context, key: key, sharedName: sharedName);
        });
        public Task LoadSecureObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, MauiAppSettingsActions.Load, context, secureOnly: true, key: key);
        });

        public void LoadSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null) => GetClassMeta(settings: settings, mode: MauiAppSettingsActions.Load, context, sharedName: sharedName);

        public Task LoadSettingsAsync(JsonSerializerContext? context = null, string? key = null, string? sharedName = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, context, key: key, sharedName: sharedName);
            });

        public Task LoadSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, string? sharedName = null)
            => Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Load, context, key: key, sharedName: sharedName);
            });

        public Task<bool> TryLoadSettingsAsync(JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, context, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
            });

        public Task<bool> TryLoadSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null)
            => Task.Run(async delegate
            {
                return await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Load, context, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
            });

        public Task LoadSecureSettingsAsync(JsonSerializerContext? context = null, string? key = null)
            => Task.Run(async delegate
            {
                await LoadSecureSettingsAsync(settings: SettingsObject, context, key: key);
            });

        public Task LoadSecureSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null)
            => Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Load, context, secureOnly: true, key: key);
            });

        public Task LoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, context, save: save, key: key, sharedName: sharedName);
            });

        public Task<bool> TryLoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, context, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
            });

        public Task LoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null)
            => Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, context, save: save, key: key, sharedName: sharedName);
            });

        public Task<bool> TryLoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, context, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
            });

        public Task LoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null)
            => Task.Run(async delegate
            {
                await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: MauiAppSettingsActions.Load, context, secureOnly: false, key: key, sharedName: sharedName);
                // Save the restored settings right away
                if (save) await SaveSettingsAsync(settings: settings, context, key: key, sharedName: sharedName);
            });
        public Task<bool> TryLoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null)
            => Task.Run(async delegate
            {
                return await GetMetaFromDictionaryAsync(
                    settings: settings, dictionary: dictionary, mode: MauiAppSettingsActions.Load, context, secureOnly: false, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
            });

        #endregion

        #region SaveSettings
        public void SaveSettings(JsonSerializerContext? context = null, string? sharedName = null) => SaveSettings(SettingsObject, context, sharedName);

        public void SaveSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => SaveObjectSetting(SettingsObject, value, context, sharedName);

        public void SaveObjectSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null)
            => GetExpressionMeta(settings, value, MauiAppSettingsActions.Save, context, sharedName: sharedName);

        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => SaveObjectSetting(this, value, context, sharedName);

        public void SaveObjectSettings(JsonSerializerContext? context = null, string? sharedName = null) => SaveSettings(this, context, sharedName);

        public void SaveSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null) 
            => GetClassMeta(settings, MauiAppSettingsActions.Save, context, sharedName: sharedName);

        public Task SaveSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null)
            => SaveObjectSettingAsync(SettingsObject, value, context, key, sharedName);

        public Task SaveObjectSettingAsync<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings, value, MauiAppSettingsActions.Save, context, key: key, sharedName: sharedName);
        });

        public Task SaveSettingsAsync(JsonSerializerContext? context = null, string? key = null, string? sharedName = null) => Task.Run(async delegate
        {
            await SaveSettingsAsync(settings: SettingsObject, context, key: key, sharedName);
        });

        public Task SaveSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, string? sharedName = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Save, context, key: key, sharedName: sharedName);
        });

        public Task SaveSecureSettingsAsync(JsonSerializerContext? context = null, string? key = null) => Task.Run(async delegate
        {
            await SaveSecureSettingsAsync(settings: SettingsObject, context, key: key);
        });

        public Task SaveSecureSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Save, context, secureOnly: true, key: key);
        });

        #endregion

        #region DeleteSettings
        public void DeleteSettings(JsonSerializerContext? context = null, string? sharedName = null) 
            => DeleteSettings(SettingsObject, context, sharedName);

        public void DeleteSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => DeleteObjectSetting(SettingsObject, value, context, sharedName);

        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => DeleteObjectSetting(this, value, context, sharedName);

        public void DeleteObjectSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => GetExpressionMeta(settings, value, MauiAppSettingsActions.Delete, context, sharedName: sharedName);

        public void DeleteObjectSettings(JsonSerializerContext? context = null, string? sharedName = null) 
            => DeleteSettings(this, context, sharedName);

        public void DeleteSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null) 
            => GetClassMeta(settings, MauiAppSettingsActions.Delete, context, sharedName: sharedName);

        #endregion

        #region LoadDefaults
        public void LoadDefaultSettings(JsonSerializerContext? context = null, string? sharedName = null) 
            => LoadDefaultSettings(SettingsObject, context, sharedName);

        public void LoadDefaultSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => LoadObjectDefaultSetting(SettingsObject, value, context, sharedName);

        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null) 
            => LoadObjectDefaultSetting(this, value, context, sharedName);

        public void LoadObjectDefaultSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null)
            => GetExpressionMeta(settings, value, MauiAppSettingsActions.LoadDefaults, context, sharedName: sharedName);

        public void LoadObjectDefaultSettings(JsonSerializerContext? context = null, string? sharedName = null) 
            => LoadDefaultSettings(this, context, sharedName);

        public void LoadDefaultSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null)
            => GetClassMeta(settings, MauiAppSettingsActions.LoadDefaults, context, sharedName: sharedName);

        #endregion

        #region Conversion

        public Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(JsonSerializerContext context)
            => ToDictionaryAsync(settings: SettingsObject, context);

        public Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null)
            => ToDictionaryAsync(settings: SettingsObject, context: context, secureOnly: secureOnly, key: key, sharedName: sharedName);

        public async Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(object? settings, JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null)
        {
            if (true)
            {
                Dictionary<string, Tuple<object?, Type>> setting = [];
                IEnumerable<MemberInfo>? declaredMembers = settings?.GetType().GetTypeInfo().DeclaredMembers;

                MauiAppSettingsMemberInfo settingsObjectInfo = new();
                MauiAppSettingsInfo settingsInfo = new();
                if (declaredMembers is null) return setting;

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    MauiAppSettingsInfo? settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(
                        settingsObjectInfo, settingsInfo, context: context, secureOnly: secureOnly, key: key, keeyEncrypted: true, sharedName: sharedName
                        );
                    if (settingsPair is not null && !settingsPair.SkipForExport)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object?, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType ?? typeof(object)));
                    }
                }
                return setting;
            }
        }

        public Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(JsonSerializerContext context)
            => ToConcurrentDictionaryAsync(settings: SettingsObject, context: context);
        public Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null)
            => ToConcurrentDictionaryAsync(settings: SettingsObject, context: context, secureOnly: secureOnly, key: key, sharedName: sharedName);

        public async Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(object? settings, JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null)
        {
            ConcurrentDictionary<string, Tuple<object?, Type>> setting = new();
            List<MemberInfo>? members = GetClassMetaAsList(settings);

            MauiAppSettingsMemberInfo settingsObjectInfo = new();
            MauiAppSettingsInfo settingsInfo = new();
            if (members is null) return setting;

            foreach (MemberInfo mInfo in members)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiAppSettingsInfo? settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(
                    settingsObjectInfo, settingsInfo, context: context, secureOnly: secureOnly, key: key, keeyEncrypted: true, sharedName: sharedName);
                if (settingsPair != null && !settingsPair.SkipForExport)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object?, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType ?? typeof(object)));
                }
            }
            return setting;
        }

        public Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null)
            => ToSettingsTupleAsync(settings: SettingsObject, value: value, context, key: key, sharedName: sharedName);

        public async Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null)
        {
            MauiAppSettingsInfo? info = await GetExpressionMetaAsKeyValuePairAsync(settings: settings, value: value, context, key: key, sharedName: sharedName);
            if (info is not null)
                return new(info.Name, new(info.Value, info.SettingsType ?? typeof(object)));
            else
                return null;
        }
        #endregion

        #region Encryption

        public Task ExhangeKeyAsync(string newKey, JsonSerializerContext? context = null, string? oldKey = null, bool reloadSettings = false)
            => Task.Run(async delegate
            {
                if (reloadSettings) await LoadSecureSettingsAsync(context, key: oldKey);
                await SaveSettingsAsync(context, key: newKey);
            });

        #endregion

        #region Private
        List<MemberInfo>? GetClassMetaAsList(object? settings)
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
        void GetClassMeta(
            object? settings,
            MauiAppSettingsActions mode, JsonSerializerContext? context = null, MauiAppSettingsTarget target = MauiAppSettingsTarget.Local,            
            string? sharedName = null
            )
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;
            Debug.WriteLine($"MauiSettings: Called '{nameof(GetClassMeta)}' => Mode = '{mode}' / Target = '{target}' (SharedName: ' {sharedName} ')");
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                MauiAppSettingsMemberInfo settingsObjectInfo = new();
                MauiAppSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = ProcessSettingsInfo(settingsObjectInfo, settingsInfo, mode, target, context, sharedName: sharedName);
                }
            }
        }
        
        async Task<bool> GetClassMetaAsync(
            object? settings, 
            MauiAppSettingsActions mode, JsonSerializerContext? context = null, MauiAppSettingsTarget target = MauiAppSettingsTarget.Local, 
            bool secureOnly = false, string? key = null, bool justTryLoading = false, string? sharedName = null)
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return false;
            Debug.WriteLine($"MauiSettings: Called '{nameof(GetClassMetaAsync)}' => Mode = '{mode}' / Target = '{target}' (SharedName: ' {sharedName} ')");
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            MauiAppSettingsMemberInfo settingsObjectInfo = new();
            MauiAppSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiAppSettingsResults result = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, context, secureOnly: secureOnly, key: key, justTryLoading: justTryLoading, sharedName: sharedName);
                if (result == MauiAppSettingsResults.EncryptionError || result == MauiAppSettingsResults.Failed) { return false; }
            }
            return true;
        }
        
        async Task<bool> GetMetaFromDictionaryAsync(
            object? settings, 
            Dictionary<string, Tuple<object?, Type>> dictionary, 
            MauiAppSettingsActions mode, JsonSerializerContext? context = null, MauiAppSettingsTarget target = MauiAppSettingsTarget.Local, 
            bool secureOnly = false, string? key = null, bool justTryLoading = false, string? sharedName = null
            )
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return false;
            Debug.WriteLine($"MauiSettings: Called '{nameof(GetMetaFromDictionaryAsync)}' => Mode = '{mode}' / Target = '{target}' (SharedName: '{sharedName}')");
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            MauiAppSettingsMemberInfo settingsObjectInfo = new();
            MauiAppSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                bool useValueFromSettingsInfo = false;
                // Try to find the matching settingsKey
                KeyValuePair<string, Tuple<object?, Type>>? keyPair = dictionary?.FirstOrDefault(keypair =>
                    mInfo.Name is not null && keypair.Key.EndsWith(mInfo.Name
                    //?.Replace("get_", string.Empty)
                    ));
                if (keyPair?.Key is not null && mInfo.Name is not null)
                {
                    useValueFromSettingsInfo = true;
                    // If a matching settingsKey was found, prepare the settingsInfo with the loaded data
                    settingsInfo = new()
                    {
                        Name = mInfo.Name.Replace("get_", string.Empty),
                        Value = keyPair.Value.Value.Item1,
                        SettingsType = keyPair.Value.Value.Item2,
                    };
                }
                else
                    useValueFromSettingsInfo = false;
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                MauiAppSettingsResults result = await ProcessSettingsInfoAsync(
                    settingsObjectInfo, settingsInfo, mode, target, context,
                    secureOnly: secureOnly, 
                    useValueFromSettingsInfo: useValueFromSettingsInfo, 
                    key: key, 
                    justTryLoading: justTryLoading,
                    sharedName: sharedName);
                if (result == MauiAppSettingsResults.EncryptionError || result == MauiAppSettingsResults.Failed)
                {
                    return false;
                }
            }
            return true;
        }

        void GetExpressionMeta<T>(
            object? settings, Expression<Func<SO, T>> value, 
            MauiAppSettingsActions mode, JsonSerializerContext? context = null, MauiAppSettingsTarget target = MauiAppSettingsTarget.Local, 
            string? sharedName = null
            )
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;
            Debug.WriteLine($"MauiSettings: Called '{nameof(GetExpressionMeta)}' => Mode = '{mode}' / Target = '{target}' / Value = '{value}' (SharedName: '{sharedName}')");
            lock (lockObject)
            {
                if (value.Body is MemberExpression memberExpression)
                {
                    _ = ProcessSettingsInfo(new MauiAppSettingsMemberInfo()
                    {
                        OrignalSettingsObject = settings,
                        Info = memberExpression.Member,

                    }, new MauiAppSettingsInfo(), mode, target, context, sharedName: sharedName);
                }
            }
        }

        async Task GetExpressionMetaAsync<T>(
            object? settings, Expression<Func<SO, T>> value, 
            MauiAppSettingsActions mode, JsonSerializerContext? context = null, MauiAppSettingsTarget target = MauiAppSettingsTarget.Local, 
            bool secureOnly = false, string? key = null, string? sharedName = null
            )
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return;

            Debug.WriteLine($"MauiSettings: Called '{nameof(GetExpressionMeta)}' => Mode = '{mode}' / Target = '{target}' / Value = '{value}' (SharedName: '{sharedName}')");
            if (value.Body is MemberExpression memberExpression)
            {
                _ = await ProcessSettingsInfoAsync(new MauiAppSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiAppSettingsInfo(), mode, target, context, secureOnly: secureOnly, key: key, sharedName: sharedName);
            }
        }

        async Task<MauiAppSettingsInfo?> GetExpressionMetaAsKeyValuePairAsync<T>(
            object? settings, Expression<Func<SO, T>> value, 
            JsonSerializerContext? context = null,
            string? key = null, string? sharedName = null
            )
        {
            if (ThrowIfSettingsObjectIsNull)
                ArgumentNullException.ThrowIfNull(settings);
            else if (settings is null)
                return null;
            Debug.WriteLine($"MauiSettings: Called '{nameof(GetExpressionMetaAsKeyValuePairAsync)}' => Type = '{typeof(T)}' / Key = '{key}' (SharedName: '{sharedName}')");
            if (value.Body is MemberExpression memberExpression)
            {
                return await ProcessSettingsInfoAsKeyValuePairAsync(new MauiAppSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new MauiAppSettingsInfo(), context, key: key, keeyEncrypted: true, sharedName: sharedName);
            }
            return new();
        }

        bool ProcessSettingsInfo(
            MauiAppSettingsMemberInfo settingsObjectInfo, MauiAppSettingsInfo settingsInfo,
            MauiAppSettingsActions mode, MauiAppSettingsTarget target,
            JsonSerializerContext? context = null,
            bool throwOnError = false, bool justTryLoading = false, string? sharedName = null
            )
        {
            // If the passed context is null, try to use the global context
            context ??= Context;
            settingsInfo ??= new();
            MauiAppSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiAppSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiAppSettingAttribute>(inherit: false)];
                if (settingBaseAttributes.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return false;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null && settingBaseAttribute is not null)
            {
                settingsInfo.Name = MauiAppSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiAppSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                switch (target)
                {
#if IOS
                    case MauiAppSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                    case MauiAppSettingsTarget.Local:
                    default:
                        if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                        {
                            //Debug.WriteLine($"MauiSettings: Dispatched");
                            Dispatcher.Dispatch(() =>
                            {
                                settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName);
                            });
                        }
                        else
                        {
                            settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName);
                        }
                        break;
                }
            }
            else
            {
                return false;
            }
            bool secure = false;
            if (settingBaseAttribute is MauiAppSettingAttribute settingAttribute)
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
                        case MauiAppSettingsTarget.ICloud:
                            if (throwOnError) throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            else break;
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (throwOnError) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
                            else break;

                    }
#else
                    if (throwOnError)
                    {
                        if (mode != MauiAppSettingsActions.LoadDefaults)
                            throw new NotSupportedException("SecureStorage is only available in the Async methods!");
                    }
#endif
                }
                else if (settingsInfo.Encrypt)
                {
                    if (throwOnError) throw new NotSupportedException("Encryption is only available on the Async methods and if the property is marked as 'Secure'");
                }
            }
            switch (mode)
            {
                case MauiAppSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object? defaultValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case MauiAppSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = MauiAppSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                            {
                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                Dispatcher.Dispatch(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                            }
                            else
                                MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                            break;
                    }
                    break;
                case MauiAppSettingsActions.Delete:
                    object? fallbackValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
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
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                            {
                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                Dispatcher.Dispatch(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                            }
                            else 
                                MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                            break;
                    }
                    break;
                case MauiAppSettingsActions.LoadDefaults:
                    object? defaulSettingtValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                            {
                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                Dispatcher.Dispatch(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                            }
                            else
                                MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                            break;
                    }
                    break;
                default:
                    break;
            }
            Debug.WriteLine($"MauiSettings: Called '{nameof(ProcessSettingsInfo)}' => Mode = '{mode}' / Target = '{target}' / Name = '{settingsInfo.Name}' / Value = '{settingsInfo.Value}' (SharedName: '{sharedName}')");
            return true;
        }

        async Task<MauiAppSettingsResults> ProcessSettingsInfoAsync(
            MauiAppSettingsMemberInfo settingsObjectInfo, MauiAppSettingsInfo settingsInfo, MauiAppSettingsActions mode, MauiAppSettingsTarget target,
            JsonSerializerContext? context = null,
            bool secureOnly = false, bool useValueFromSettingsInfo = false, string? key = null, bool keepEncrypted = false, bool justTryLoading = false, 
            string? sharedName = null
            )
        {
            // If the passed context is null, try to use the global context
            context ??= Context;
            settingsInfo ??= new();
            MauiAppSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiAppSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiAppSettingAttribute>(inherit: false)];
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return MauiAppSettingsResults.Skipped;
                }
                settingBaseAttribute = settingBaseAttributes?.FirstOrDefault();
                settingsInfo.Name = MauiAppSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiAppSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            else
            {
                return MauiAppSettingsResults.Skipped;
            }
            bool secure = false;
            if (settingBaseAttribute is MauiAppSettingAttribute settingAttribute)
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
                        return MauiAppSettingsResults.Skipped;
                    // If the value is not used from the passed settingsInfo, load it

                    switch (target)
                    {
#if IOS
                    case MauiAppSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (!useValueFromSettingsInfo)
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName));
                                }
                                else
                                    settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName);
                                Debug.WriteLine($"MauiSettings: Loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            else
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => settingsInfo.Value = MauiAppSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default));
                                }
                                else
                                    settingsInfo.Value = MauiAppSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
                                Debug.WriteLine($"MauiSettings: Loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            break;
                    }
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            //break;
#endif
                        case MauiAppSettingsTarget.Local:
                        default:
                            if (!useValueFromSettingsInfo)
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(async () => settingsInfo.Value = await MauiAppSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string));
                                }
                                else
                                {
                                    settingsInfo.Value = await MauiAppSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                                }
                                Debug.WriteLine($"MauiSettings: Loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            else
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => settingsInfo.Value = MauiAppSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default));
                                }
                                else
                                {
                                    settingsInfo.Value = MauiAppSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
                                }
                                Debug.WriteLine($"MauiSettings: Loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            break;
                    }
                }
                else
                {
#if IOS
                    switch (target)
                    {
                        case MauiAppSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                        case MauiAppSettingsTarget.Local:
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
                case MauiAppSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object? defaultValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
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
                                    MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, decryptedString, settingsInfo.SettingsType);
                            }
                            catch (Exception ex)
                            {
                                OnEncryptionErrorEvent(new()
                                {
                                    Exception = ex,
                                    Key = key,
                                });
                                return MauiAppSettingsResults.EncryptionError;
                            }
                            break;
                        }
                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case MauiAppSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = MauiAppSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiAppSettingsTarget.Local:
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
                                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                            {
                                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                                await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString));
                                            }
                                            else
                                                await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);

                                            Debug.WriteLine($"MauiSettings: Saved '{settingsInfo.Name}' => '{encryptedString}'");
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiAppSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                    {
                                        if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                        {
                                            //Debug.WriteLine($"MauiSettings: Dispatched");
                                            await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString));
                                        }
                                        else
                                            await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);

                                        Debug.WriteLine($"MauiSettings: Saved '{settingsInfo.Name}' => '{secureString}'");
                                    }
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                                }
                                else
                                    MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                                Debug.WriteLine($"MauiSettings: Saved '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            break;
                    }
                    break;
                case MauiAppSettingsActions.Delete:
                    object? fallbackValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
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
                        case MauiAppSettingsTarget.Local:
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
                                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                            {
                                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                                await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString));
                                            }
                                            else
                                                await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                            Debug.WriteLine($"MauiSettings: Deleted '{settingsInfo.Name}' => '{encryptedString}'");
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiAppSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                    {
                                        if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                        {
                                            //Debug.WriteLine($"MauiSettings: Dispatched");
                                            await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString));
                                        }
                                        else
                                            await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                        Debug.WriteLine($"MauiSettings: Deleted '{settingsInfo.Name}' => '{secureString}'");
                                    }
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                                }
                                else
                                    MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                                Debug.WriteLine($"MauiSettings: Deleted '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            break;
                    }
                    break;
                case MauiAppSettingsActions.LoadDefaults:
                    object? defaulSettingtValue = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiAppSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiAppSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case MauiAppSettingsTarget.Local:
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
                                            if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                            {
                                                //Debug.WriteLine($"MauiSettings: Dispatched");
                                                await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString));
                                            }
                                            else
                                                await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                            Debug.WriteLine($"MauiSettings: Default loaded '{settingsInfo.Name}' => '{encryptedString}'");
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return MauiAppSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                    {
                                        if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                        {
                                            //Debug.WriteLine($"MauiSettings: Dispatched");
                                            await Dispatcher.DispatchAsync(async () => await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString));
                                        }
                                        else
                                            await MauiAppSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                        Debug.WriteLine($"MauiSettings: Default loaded '{settingsInfo.Name}' => '{secureString}'");
                                    }
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                                {
                                    //Debug.WriteLine($"MauiSettings: Dispatched");
                                    await Dispatcher.DispatchAsync(() => MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName));
                                }
                                else
                                    MauiAppSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value, context, sharedName);
                                Debug.WriteLine($"MauiSettings: Default loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
            Debug.WriteLine($"MauiSettings: Called '{nameof(ProcessSettingsInfoAsync)}' => Mode = '{mode}' / Target = '{target}' / Name = '{settingsInfo.Name}' / Value = '{settingsInfo.Value}' (SharedName: '{sharedName}')");
            return MauiAppSettingsResults.Success;
        }

        async Task<MauiAppSettingsInfo?> ProcessSettingsInfoAsKeyValuePairAsync(
            MauiAppSettingsMemberInfo settingsObjectInfo, MauiAppSettingsInfo settingsInfo,
            JsonSerializerContext? context = null,
            bool secureOnly = false, string? key = null, bool keeyEncrypted = false, string? sharedName = null
            )
        {
            settingsInfo ??= new();
            MauiAppSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<MauiAppSettingAttribute> settingBaseAttributes
                    = [.. settingsObjectInfo.Info.GetCustomAttributes<MauiAppSettingAttribute>(inherit: false)];
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return null;
                }
                settingBaseAttribute = settingBaseAttributes?.FirstOrDefault();
                settingsInfo.Name = MauiAppSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject?.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiAppSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = MauiAppSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            if (settingBaseAttribute is MauiAppSettingAttribute settingAttribute)
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

                    if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                    {
                        //Debug.WriteLine($"MauiSettings: Dispatched");
                        await Dispatcher.DispatchAsync(() => settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName));
                    }
                    else
                        settingsInfo.Value = MauiAppSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.SettingsType, settingsInfo.Default, context, sharedName);
                    Debug.WriteLine($"MauiSettings: Loaded '{settingsInfo.Name}' => '{settingsInfo.Value}'");
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    if (Dispatcher is not null && Dispatcher.IsDispatchRequired)
                    {
                        //Debug.WriteLine($"MauiSettings: Dispatched");
                        await Dispatcher.DispatchAsync(async () => settingsInfo.Value = await MauiAppSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string));
                    }
                    else
                        settingsInfo.Value = await MauiAppSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                    Debug.WriteLine($"MauiSettings: Saved '{settingsInfo.Name}' => '{settingsInfo.Value}'");

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
            Debug.WriteLine($"MauiSettings: Called '{nameof(ProcessSettingsInfoAsKeyValuePairAsync)}' => Name = '{settingsInfo.Name}' / Value = '{settingsInfo.Value}' (SharedName: '{sharedName}')");
            return settingsInfo;
        }

        #endregion

        #endregion
    }
}
