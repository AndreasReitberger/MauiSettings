using AndreasReitberger.Shared.Core.Utilities;

namespace MauiSettings.XTests
{
    public class UnitTest
    {
        [Fact]
        public async Task ExportSettingsTest()
        {
            try
            {
                // Does not work at the moment, cannot access the Maui.Preference...
                // Ref: https://github.com/dotnet/maui/discussions/14847
                string hash = EncryptionManager.GenerateBase64Key();
                SettingsApp.LoadDefaultSettings();

                SettingsApp.LicenseInfo = "My secretd license key";
                await SettingsApp.SaveSettingsAsync(key: hash);
                SettingsApp.LicenseInfo = "";
                await SettingsApp.LoadSettingsAsync(key: hash);
                Assert.True(!string.IsNullOrEmpty(SettingsApp.LicenseInfo));

                var settingsDictionary = await SettingsApp.ToDictionaryAsync();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}