using AndreasReitberger.Maui.Attributes;
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Utilities;
using AndreasReitberger.Shared.Core.Utilities;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AndreasReitberger.Maui.Interfaces
{
    public interface IMauiAppSettingsService<SO> where SO : class
    {
        #region Settings Object

        public SO SettingsObject { get; }

        #endregion

        #region Dispatcher

        public IDispatcher? Dispatcher { get; set; }

        #endregion

        #region Serializer

        public JsonSerializerContext? Context { get; set; }
        #endregion

        #region Properties

        public bool ThrowIfSettingsObjectIsNull { get; set; }
        #endregion

        #region Methods

        #region LoadSettings
        public void LoadSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task LoadSecureSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null);
        public void LoadObjectSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadObjectSetting<T>(object? settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task LoadSecureObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null);
        public void LoadSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null);
        public Task LoadSettingsAsync(JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task LoadSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task<bool> TryLoadSettingsAsync(JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null);
        public Task<bool> TryLoadSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null);
        public Task LoadSecureSettingsAsync(JsonSerializerContext? context = null, string? key = null);
        public Task LoadSecureSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null);
        public Task LoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null);
        public Task<bool> TryLoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null);
        public Task LoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null);
        public Task<bool> TryLoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null);
        public Task LoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, bool save = true, string? key = null, string? sharedName = null);
        public Task<bool> TryLoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, JsonSerializerContext? context = null, string? key = null, bool justTryLoading = true, string? sharedName = null);

        #endregion

        #region SaveSettings
        public void SaveSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void SaveSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void SaveObjectSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void SaveObjectSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void SaveSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null);
        public Task SaveSettingAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task SaveObjectSettingAsync<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task SaveSettingsAsync(JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task SaveSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task SaveSecureSettingsAsync(JsonSerializerContext? context = null, string? key = null);
        public Task SaveSecureSettingsAsync(object? settings, JsonSerializerContext? context = null, string? key = null);

        #endregion

        #region DeleteSettings
        public void DeleteSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void DeleteSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void DeleteObjectSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void DeleteObjectSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void DeleteSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null);

        #endregion

        #region LoadDefaults
        public void LoadDefaultSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadDefaultSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadObjectDefaultSetting<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadObjectDefaultSettings(JsonSerializerContext? context = null, string? sharedName = null);
        public void LoadDefaultSettings(object? settings, JsonSerializerContext? context = null, string? sharedName = null);

        #endregion

        #region Conversion

        public Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(JsonSerializerContext context);
        public Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null);
        public Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(object? settings, JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null);
        public Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(JsonSerializerContext context);
        public Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null);
        public Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(object? settings, JsonSerializerContext? context = null, bool secureOnly = false, string? key = null, string? sharedName = null);
        public Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        public Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(object? settings, Expression<Func<SO, T>> value, JsonSerializerContext? context = null, string? key = null, string? sharedName = null);
        #endregion

        #region Encryption

        public Task ExhangeKeyAsync(string newKey, JsonSerializerContext? context = null, string? oldKey = null, bool reloadSettings = false);

        #endregion

#if IOS
        #region Methods

        #region Save
        public Task SyncSettingsToICloudAsync(JsonSerializerContext context, string? sharedName = null);
        public void SyncSettingsToICloud<T>(Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null);
        public Task SyncSettingsToICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null);
        public void SyncSettingsToICloud<T>(object settings, Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null);
        #endregion

        #region Load

        public Task SyncSettingsFromICloudAsync(JsonSerializerContext context, string? sharedName = null);
        public void SyncSettingsFromICloud<T>(Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null);
        public Task SyncSettingsFromICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null);
        public void SyncSettingsFromICloud<T>(object settings, Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null);
        #endregion

        #endregion
#endif

        #region Private
        /*
        List<MemberInfo>? GetClassMetaAsList(object? settings);
        void GetClassMeta(
            object? settings,
            MauiSettingsActions mode, JsonSerializerContext? context = null, MauiSettingsTarget target = MauiSettingsTarget.Local,
            string? sharedName = null
            );

        Task<bool> GetClassMetaAsync(
            object? settings,
            MauiSettingsActions mode, JsonSerializerContext? context = null, MauiSettingsTarget target = MauiSettingsTarget.Local,
            bool secureOnly = false, string? key = null, bool justTryLoading = false, string? sharedName = null);

        Task<bool> GetMetaFromDictionaryAsync(
            object? settings,
            Dictionary<string, Tuple<object?, Type>> dictionary,
            MauiSettingsActions mode, JsonSerializerContext? context = null, MauiSettingsTarget target = MauiSettingsTarget.Local,
            bool secureOnly = false, string? key = null, bool justTryLoading = false, string? sharedName = null
            );

        void GetExpressionMeta<T>(
            object? settings, Expression<Func<SO, T>> value,
            MauiSettingsActions mode, JsonSerializerContext? context = null, MauiSettingsTarget target = MauiSettingsTarget.Local,
            string? sharedName = null
            );

        Task GetExpressionMetaAsync<T>(
            object? settings, Expression<Func<SO, T>> value,
            MauiSettingsActions mode, JsonSerializerContext? context = null, MauiSettingsTarget target = MauiSettingsTarget.Local,
            bool secureOnly = false, string? key = null, string? sharedName = null
            );

        Task<MauiSettingsInfo?> GetExpressionMetaAsKeyValuePairAsync<T>(
            object? settings, Expression<Func<SO, T>> value,
            JsonSerializerContext? context = null,
            string? key = null, string? sharedName = null
            );

        bool ProcessSettingsInfo(
            MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo,
            MauiSettingsActions mode, MauiSettingsTarget target,
            JsonSerializerContext? context = null,
            bool throwOnError = false, bool justTryLoading = false, string? sharedName = null
            );

        Task<MauiSettingsResults> ProcessSettingsInfoAsync(
            MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode, MauiSettingsTarget target,
            JsonSerializerContext? context = null,
            bool secureOnly = false, bool useValueFromSettingsInfo = false, string? key = null, bool keepEncrypted = false, bool justTryLoading = false,
            string? sharedName = null
            );

        Task<MauiSettingsInfo?> ProcessSettingsInfoAsKeyValuePairAsync(
            MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo,
            JsonSerializerContext? context = null,
            bool secureOnly = false, string? key = null, bool keeyEncrypted = false, string? sharedName = null
            );
        */
        #endregion

        #endregion
    }
}
