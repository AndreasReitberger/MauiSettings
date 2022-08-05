using AndreasReitberger.Maui.Enums;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on iOS.
    public partial class MauiSettingsGeneric<SO> where SO : new()
    {
        #region Methods
        public async Task SyncSettingsToICloud()
        {
            await Task.Run(async delegate
            {
                await SyncSettingsToICloud(settings: SettingsObject);
            });
        }

        public async Task SyncSettingsToICloud(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsyn(settings: settings, mode: MauiSettingsActions.Load);
            });
        }

        public async Task SyncSettingsFromICloud()
        {

        }
        #endregion
    }
}