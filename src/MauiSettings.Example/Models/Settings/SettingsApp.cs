using AndreasReitberger.Maui;
using AndreasReitberger.Maui.Attributes;
using System.Runtime.CompilerServices;

namespace MauiSettings.Example.Models.Settings
{
    /// <summary>
    /// Holds the whole local application settings
    /// </summary>
    public partial class SettingsApp : MauiSettings<SettingsApp>
    {
        #region Properties
        static bool _settingsChanged = false;
        public static bool SettingsChanged
        {
            get => _settingsChanged;
            set
            {
                if (_settingsChanged == value) return;
                _settingsChanged = value;
            }
        }
        #endregion

        #region DeviceSettings
        /// <summary>
        /// Opens the settings UI on the current device
        /// </summary>
        public static void OpenDeviceSettings()
        {
            AppInfo.ShowSettingsUI();
        }
        #endregion

        #region Settings

        #region Version
        [MauiSetting(Name = nameof(App_SettingsVersion), DefaultValue = "1.0.0", SkipForExport = true)]
        public static Version? App_SettingsVersion { get; set; }

        #endregion

        #region Default

        [MauiSetting(Name = nameof(License_AcceptEndUserLicenseAgreement), DefaultValue = false)]
        public static bool License_AcceptEndUserLicenseAgreement { get; set; }

        [MauiSetting(Name = nameof(License_ProUpgradePurchased), DefaultValue = false)]
        public static bool License_ProUpgradePurchased { get; set; }

        [MauiSetting(Name = nameof(LicenseInfo), DefaultValue = "", Secure = true, Encrypt = true)]
        public static string LicenseInfo { get; set; } = string.Empty;
        #endregion

        #region Privacy
        [MauiSetting(Name = nameof(Privacy_AcceptPrivacyPolicy), DefaultValue = false, SkipForExport = true)]
        public static bool Privacy_AcceptPrivacyPolicy { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of the current <c>SettingsApp</c>
        /// </summary>
        /// <returns>Copy of the current <c>SettingsApp</c></returns>
        public static SettingsApp Copy()
        {
            SettingsApp app = new();
            app.LoadObjectSettings();
            return app;
        }

        #endregion
    }
}
