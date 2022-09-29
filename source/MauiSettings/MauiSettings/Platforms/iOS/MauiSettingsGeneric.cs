using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Cloud;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on iOS.
    public partial class MauiSettingsGeneric<SO> where SO : new()
    {
        #region Methods
        public async Task SyncSettingsToICloudAsync()
        {
            await Task.Run(async delegate
            {
                await SyncSettingsToICloudAsync(settings: SettingsObject);
            });
        }

        public async Task SyncSettingsToICloudAsync<T>(Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await SyncSettingsToICloudAsync(settings: SettingsObject, value: value);
            });
        }

        public async Task SyncSettingsToICloudAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save);
            });
        }
        public async Task SyncSettingsToICloudAsync(object settings, Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, value: value, mode: MauiSettingsActions.Save);
            });
        }

        public async Task SyncSettingsFromICloudAsync()
        {

        }
        #endregion

        #region Private

        #region Private

        static new bool ProcessSettingsInfo(MauiSettingsMemberInfo settingsObjectInfo, MauiSettingsInfo settingsInfo, MauiSettingsActions mode)
        {
            // Save settings also locally
            base.ProcessSettingsInfo(settingsObjectInfo, settingsInfo, mode, target: MauiSettingsTarget.Local);
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

                // Load the value from the cloud, if null, use Default
                settingsInfo.Value = ICloudStoreManager.GetValue(key) ?? settingsInfo.Default;
            }
            bool? secure = false;
            if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (secure ?? false)
                {
#if DEBUG

#else
                    throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
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
                    ICloudStoreManager.SetValue<string>(settingsInfo.Name, settingsInfo.Value);
                    break;
                case MauiSettingsActions.Delete:
                    object fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    if(settingsInfo.Value != null)
                    {
                        // If there is a default value, do not delete the key. Instead write the default value
                        ICloudStoreManager.SetValue<string>(settingsInfo.Name, settingsInfo.Value);
                    }
                    else
                    {
                        // Otherwise delete the key from the cloud storage
                        ICloudStoreManager.DeleteValue<string>(settingsInfo.Name);
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
                    ICloudStoreManager.SetValue<string>(settingsInfo.Name, settingsInfo.Value);
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