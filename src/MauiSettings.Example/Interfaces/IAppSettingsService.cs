#if MauiAppSettings
using AndreasReitberger.Maui.Interfaces;
using MauiSettings.Example.Services;

namespace MauiSettings.Example.Interfaces
{
    public interface IAppSettingsService : IMauiAppSettings<AppSettingsService>
    {
        #region Settings

        #region Version
        public Version? App_SettingsVersion { get; set; }
        public string ResourcesCurrentVersionAvailable { get; set; }

        #endregion
        #region Default

        public bool License_AcceptEndUserLicenseAgreement { get; set; }

        public bool License_ProUpgradePurchased { get; set; }

        public string LicenseInfo { get; set; }
        #endregion

        #region Privacy
        public bool Privacy_AcceptPrivacyPolicy { get; set; }

        #endregion

        #region Misc

        public bool Misc_Boolean { get; set; }

        public string Misc_String { get; set; }

        public double Misc_Numeric { get; set; }

        public int Misc_Counter { get; set; }
        #endregion

        #endregion
    }
}
#endif