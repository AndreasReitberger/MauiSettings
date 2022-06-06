using AndreasReitberger.Maui.Attributes;
using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Helper;
using AndreasReitberger.Maui.Utilities;
using System.Reflection;

namespace AndreasReitberger.Maui
{
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
        public void LoadObjectSettings()
        {
            LoadSettings(this);
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
                await GetClassMetaAsyn(settings: settings, mode: MauiSettingsActions.Load);
            });
        }
        #endregion

        #region SaveSettings
        public static void SaveSettings()
        {
            SaveSettings(SettingsObject);
        }
        public void SaveObjectSettings()
        {
            SaveSettings(this);
        }
        public static void SaveSettings(object settings)
        {
            GetClassMeta(settings, MauiSettingsActions.Save);
        }
        #endregion

        #region DeleteSettings
        public static void DeleteSettings()
        {
            DeleteSettings(SettingsObject);
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
        static void GetClassMeta(object settings, MauiSettingsActions mode)
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

                    MauiSettingBaseAttribute settingBaseAttribute = null;
                    if (settingsObjectInfo.Info is not null)
                    {
                        List<MauiSettingAttribute> settingBaseAttributes
                            = settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)
                            .ToList();
                        if (settingBaseAttributes?.Count == 0)
                        {
                            // If the member has not the needed MauiSettingsAttribute, continue with the search
                            continue;
                        }
                        settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
                    }
                    // Get the formated settings name
                    if (settingsObjectInfo.Info is not null)
                    {
                        settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                        settingsInfo.SettingsType = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                        settingsInfo.Default = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                        
                        //Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                        //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                        settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                    }
                    bool? secure = false;
                    if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
                    {
                        secure = settingAttribute.Secure;
                        if (secure ?? false)
                        {
#if DEBUG

#else
                            throw new NotSupportedException("SecureStorage is only available in the Async methods!");
#endif
                        }
                    }

                    switch (mode)
                    {
                        case MauiSettingsActions.Load:
                            if(settingBaseAttribute?.DefaultValueInUse ?? false)
                            {
                                object defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                            
                            }
                            // Sets the loaded value back to the settingsObject
                            MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                            break;
                        case MauiSettingsActions.Save:
                            // Get the value from the settingsObject
                            settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                            MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                        case MauiSettingsActions.Delete:
                            object fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                            settingsInfo.Value = fallbackValue;
                            settingsInfo.Default = fallbackValue;
                            if (settingsObjectInfo.Info is not null)
                            {
                                MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
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
                            break;
                        default:
                            break;
                    }
                }

                // Save settingsObject to MauiSettings

            }
        }
        static async Task GetClassMetaAsyn(object settings, MauiSettingsActions mode)
        {
            //lock (lockObject)
            if(true)
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

                    MauiSettingBaseAttribute settingBaseAttribute = null;
                    if (settingsObjectInfo.Info is not null)
                    {
                        List<MauiSettingAttribute> settingBaseAttributes
                            = settingsObjectInfo.Info.GetCustomAttributes<MauiSettingAttribute>(inherit: false)
                            .ToList();
                        if (settingBaseAttributes?.Count == 0)
                        {
                            // If the member has not the needed MauiSettingsAttribute, continue with the search
                            continue;
                        }
                        settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
                    }
                    bool? secure = false;
                    if (settingBaseAttribute is MauiSettingAttribute settingAttribute)
                    {
                        secure = settingAttribute.Secure;
                    }

                    // Get the formated settings name
                    if (settingsObjectInfo.Info is not null)
                    {
                        settingsInfo.Name = MauiSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                        Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                        //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                        if (!secure ?? true)
                        {
                            settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                        }
                        else if(settingsInfo.SettingsType == typeof(string))
                        {
                            settingsInfo.Value = await MauiSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                        }
                        else
                        {
                            throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                        }
                    }

                    switch (mode)
                    {
                        case MauiSettingsActions.Load:
                            if(settingBaseAttribute?.DefaultValueInUse ?? false)
                            {
                                object defaultValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                            
                            }
                            // Sets the loaded value back to the settingsObject

                            MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                            break;
                        case MauiSettingsActions.Save:
                            // Get the value from the settingsObject
                            settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                            MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                        case MauiSettingsActions.Delete:
                            object fallbackValue = MauiSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                            settingsInfo.Value = fallbackValue;
                            settingsInfo.Default = fallbackValue;
                            if (settingsObjectInfo.Info is not null)
                            {
                                MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
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
                            break;
                        default:
                            break;
                    }
                }

                // Save settingsObject to MauiSettings

            }
        }
#endregion

        #endregion
    }
}
