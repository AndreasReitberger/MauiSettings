using Foundation;

namespace AndreasReitberger.Maui.Cloud
{
    // Docs: https://learn.microsoft.com/en-us/xamarin/ios/data-cloud/introduction-to-icloud
    public partial class ICloudStoreManager
    {
        #region Properties

        const long LimitKey = 64;           // Maximum key size - Key names cannot be longer than 64 bytes.
        const long LimitValue = 64000;      // Maximum value size - You cannot store more than 64 kilobytes in a single value.

        public static NSUbiquitousKeyValueStore Store => NSUbiquitousKeyValueStore.DefaultStore;

        #endregion

        #region Methods

        public static object? GetValue(string key) => Store?.GetString(key);
        
        public static void SetValue<T>(string key, T value, bool synchronize = true)
        {
            // Maximum key size - Key names cannot be longer than 64 bytes.
            // Maximum value size - You cannot store more than 64 kilobytes in a single value.
            long size = key?.Length ?? 0 * sizeof(char);
            if (size > LimitKey)
            {
                throw new ArgumentOutOfRangeException($"The size of the key '{key}' exceeds the limit of '{LimitKey}' (current size is '{size}')!");
            }
            if (value is string valueString)
            {
                size = valueString.Length * sizeof(char);
                if (size > LimitKey)
                {
                    throw new ArgumentOutOfRangeException($"The size of the value '{valueString}' exceeds the limit of '{LimitValue}' (current size is '{size}')!");
                }

                Store?.SetString(key, valueString);  // key and value
                if (synchronize)
                    Store?.Synchronize();
            }
        }

        public static void SeString(string key, string value, bool synchronize = true)
        {
            SetValue(key, value, synchronize);
        }

        public static void DeleteValue(string key, bool synchronize = true)
        {
            Store?.Remove(key);
            if (synchronize)
                Store?.Synchronize();
        }
        #endregion
    }
}
