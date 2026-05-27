#if MauiAppSettings
using AndreasReitberger.Maui;
using AndreasReitberger.Maui.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiSettings.Example.Interfaces;
using System.Text.Json.Serialization;

namespace MauiSettings.Example.Services
{
    public partial class AppSettingsService : MauiAppSettings<AppSettingsService>, IAppSettingsService
    {
        #region Settings

        #region Version
        [ObservableProperty, MauiAppSetting(Name = nameof(App_SettingsVersion), DefaultValue = "1.0.0", SkipForExport = true)]
        public partial Version? App_SettingsVersion { get; set; }

        [ObservableProperty, MauiAppSetting(Name = nameof(ResourcesCurrentVersionAvailable))]
        public partial string ResourcesCurrentVersionAvailable { get; set; } = string.Empty;

        #endregion

        #region Default

        [ObservableProperty, MauiAppSetting(Name = nameof(License_AcceptEndUserLicenseAgreement), DefaultValue = false)]
        public partial bool License_AcceptEndUserLicenseAgreement { get; set; }

        [ObservableProperty, MauiAppSetting(Name = nameof(License_ProUpgradePurchased), DefaultValue = false)]
        public partial bool License_ProUpgradePurchased { get; set; }

        [ObservableProperty, MauiAppSetting(Name = nameof(LicenseInfo), DefaultValue = "", Secure = true, Encrypt = true)]
        public partial string LicenseInfo { get; set; } = string.Empty;
        #endregion

        #region Privacy
        [ObservableProperty, MauiAppSetting(Name = nameof(Privacy_AcceptPrivacyPolicy), DefaultValue = false, SkipForExport = true)]
        public partial bool Privacy_AcceptPrivacyPolicy { get; set; }

        #endregion

        #region Misc

        [ObservableProperty, MauiAppSetting(Name = nameof(Misc_Boolean), DefaultValue = false)]
        public partial bool Misc_Boolean { get; set; }

        [ObservableProperty, MauiAppSetting(Name = nameof(Misc_String))]
        public partial string Misc_String { get; set; } = string.Empty;

        [ObservableProperty, MauiAppSetting(Name = nameof(Misc_Numeric), DefaultValue = 0)]
        public partial double Misc_Numeric { get; set; } = 0;

        [ObservableProperty, MauiAppSetting(Name = nameof(Misc_Counter))]
        public partial int Misc_Counter { get; set; } = 0;
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of the current <c>AppSettingsService</c>
        /// </summary>
        /// <returns>Copy of the current <c>AppSettingsService</c></returns>
        public static AppSettingsService Copy(JsonSerializerContext context)
        {
            AppSettingsService app = new();
            app.LoadObjectSettings(context);
            return app;
        }

        #endregion
    }
}
#endif