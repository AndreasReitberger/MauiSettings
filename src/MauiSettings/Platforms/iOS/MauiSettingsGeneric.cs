using AndreasReitberger.Maui.Enums;
using System.Linq.Expressions;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on iOS.
    public partial class MauiSettingsGeneric<SO> where SO : new()
    {
        #region Methods

        #region Save
        public async Task SyncSettingsToICloudAsync()
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            await Task.Run(async delegate
            {
                await SyncSettingsToICloudAsync(settings: SettingsObject);
            });
        }

        public void SyncSettingsToICloud<T>(Expression<Func<SO, T>> value)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            SyncSettingsToICloud(settings: SettingsObject, value: value);
        }

        public async Task SyncSettingsToICloudAsync(object settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save, target: MauiSettingsTarget.ICloud);
            });
        }
        public void SyncSettingsToICloud<T>(object settings, Expression<Func<SO, T>> value)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiSettingsActions.Save, target: MauiSettingsTarget.ICloud);
        }

        #endregion

        #region Load

        public async Task SyncSettingsFromICloudAsync()
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            await Task.Run(async delegate
            {
                await SyncSettingsFromICloudAsync(settings: SettingsObject);
            });
        }

        public void SyncSettingsFromICloud<T>(Expression<Func<SO, T>> value)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            SyncSettingsFromICloud(settings: SettingsObject, value: value);
        }

        public async Task SyncSettingsFromICloudAsync(object settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, target: MauiSettingsTarget.ICloud);
            });
        }
        public void SyncSettingsFromICloud<T>(object settings, Expression<Func<SO, T>> value)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiSettingsActions.Load, target: MauiSettingsTarget.ICloud);
        }
        #endregion

        #endregion
    }
}