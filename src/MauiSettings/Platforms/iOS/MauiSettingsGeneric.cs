using AndreasReitberger.Maui.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on iOS.
    public partial class MauiSettingsGeneric<SO> : ObservableObject where SO : new()
    {
        #region Methods

        #region Save
        public async Task SyncSettingsToICloudAsync(JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            await Task.Run(async delegate
            {
                await SyncSettingsToICloudAsync(settings: SettingsObject, context, sharedName);
            });
        }

        public void SyncSettingsToICloud<T>(Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            SyncSettingsToICloud(settings: SettingsObject, value: value, context, sharedName);
        }

        public async Task SyncSettingsToICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Save, context, target: MauiSettingsTarget.ICloud, sharedName: sharedName);
            });
        }
        public void SyncSettingsToICloud<T>(object settings, Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiSettingsActions.Save, context, target: MauiSettingsTarget.ICloud, sharedName);
        }

        #endregion

        #region Load

        public async Task SyncSettingsFromICloudAsync(JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            await Task.Run(async delegate
            {
                await SyncSettingsFromICloudAsync(settings: SettingsObject, context, sharedName);
            });
        }

        public void SyncSettingsFromICloud<T>(Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(SettingsObject);
            SyncSettingsFromICloud(settings: SettingsObject, value: value, context, sharedName);
        }

        public async Task SyncSettingsFromICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiSettingsActions.Load, context, target: MauiSettingsTarget.ICloud, sharedName: sharedName);
            });
        }
        public void SyncSettingsFromICloud<T>(object settings, Expression<Func<SO, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiSettingsActions.Load, context, target: MauiSettingsTarget.ICloud, sharedName: sharedName);
        }
        #endregion

        #endregion
    }
}