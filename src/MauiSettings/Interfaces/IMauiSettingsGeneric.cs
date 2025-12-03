using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace AndreasReitberger.Maui.Interfaces
{
    public interface IMauiSettingsGeneric<SO>
    {
        public static SO? SettingsObject { get; }
        public static bool ThrowIfSettingsObjectIsNull { get; set; }

        #region Properties
        public static IDispatcher? Dispatcher { get; set; }
        public static string? Hash { get; set; }
        public static string? PassPhrase { get; set; }
        #endregion

        #region Methods

        #region LoadSettings
        public static abstract void LoadSettings();
        public static abstract void LoadSetting<T>(Expression<Func<SO, T>> value);
        public static abstract Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null);
        public static abstract Task LoadSecureSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null);
        public abstract void LoadObjectSettings();
        public static abstract void LoadObjectSetting<T>(object? settingsObject, Expression<Func<SO, T>> value);
        public static abstract Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null);
        public static abstract Task LoadSecureObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null);
        public static abstract void LoadSettings(object? settings);
        public static abstract Task LoadSettingsAsync(string? key = null);
        public static abstract Task LoadSettingsAsync(object? settings, string? key = null);
        public static abstract Task<bool> TryLoadSettingsAsync(string? key = null, bool justTryLoading = true);
        public static abstract Task<bool> TryLoadSettingsAsync(object? settings, string? key = null, bool justTryLoading = true);
        public static abstract Task LoadSecureSettingsAsync(string? key = null);
        public static abstract Task LoadSecureSettingsAsync(object? settings, string? key = null);
        public static abstract Task LoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, bool save = true, string? key = null);
        public static abstract Task<bool> TryLoadSettingsAsync(Dictionary<string, Tuple<object?, Type>> dictionary, string? key = null, bool justTryLoading = true);
        public static abstract Task LoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, bool save = true, string? key = null);
        public static abstract Task<bool> TryLoadSettingsAsync(string settingsKey, Tuple<object?, Type> data, string? key = null, bool justTryLoading = true);
        public static abstract Task LoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, bool save = true, string? key = null);
        public static abstract Task<bool> TryLoadSettingsAsync(object? settings, Dictionary<string, Tuple<object?, Type>> dictionary, string? key = null, bool justTryLoading = true);
        #endregion

        #region SaveSettings
        public static abstract void SaveSettings();
        public static abstract void SaveSetting<T>(Expression<Func<SO, T>> value);
        public static abstract void SaveObjectSetting<T>(object? settings, Expression<Func<SO, T>> value);
        public abstract void SaveObjectSetting<T>(Expression<Func<SO, T>> value);
        public abstract void SaveObjectSettings();
        public static abstract void SaveSettings(object? settings);
        public static abstract Task SaveSettingsAsync(string? key = null);
        public static abstract Task SaveSettingsAsync(object? settings, string? key = null);
        public static abstract Task SaveSecureSettingsAsync(string? key = null);
        public static abstract Task SaveSecureSettingsAsync(object? settings, string? key = null);

        #endregion

        #region DeleteSettings
        public static abstract void DeleteSettings();
        public static abstract void DeleteSetting<T>(Expression<Func<SO, T>> value);
        public abstract void DeleteObjectSetting<T>(Expression<Func<SO, T>> value);
        public static abstract void DeleteObjectSetting<T>(object? settings, Expression<Func<SO, T>> value);
        public abstract void DeleteObjectSettings();
        public static abstract void DeleteSettings(object? settings);

        #endregion

        #region LoadDefaults
        public static abstract void LoadDefaultSettings();
        public static abstract void LoadDefaultSetting<T>(Expression<Func<SO, T>> value);
        public abstract void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value);
        public static abstract void LoadObjectDefaultSetting<T>(object? settings, Expression<Func<SO, T>> value);
        public abstract void LoadObjectDefaultSettings();
        public static abstract void LoadDefaultSettings(object? settings);

        #endregion

        #region Conversion

        public static abstract Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync();
        public static abstract Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(bool secureOnly = false, string? key = null);
        public static abstract Task<Dictionary<string, Tuple<object?, Type>>> ToDictionaryAsync(object? settings, bool secureOnly = false, string? key = null);
        public static abstract Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync();
        public static abstract Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(bool secureOnly = false, string? key = null);
        public static abstract Task<ConcurrentDictionary<string, Tuple<object?, Type>>> ToConcurrentDictionaryAsync(object? settings, bool secureOnly = false, string? key = null);
        public static abstract Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value, string? key = null);
        public static abstract Task<Tuple<string, Tuple<object?, Type>>?> ToSettingsTupleAsync<T>(object? settings, Expression<Func<SO, T>> value, string? key = null);
        #endregion

        #region Encryption

        public static abstract Task ExhangeKeyAsync(string newKey, string? oldKey = null, bool reloadSettings = false);

        #endregion

        #endregion
    }
}
