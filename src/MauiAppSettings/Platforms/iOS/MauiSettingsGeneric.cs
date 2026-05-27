using AndreasReitberger.Maui.Enums;
using AndreasReitberger.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on iOS.
    public partial class MauiAppSettingsService : ObservableObject, IMauiAppSettingsService
    //public partial class MauiAppSettingsGeneric<SO> : ObservableObject where SO : new()
    {
        #region Methods

        #region Save
        public async Task SyncSettingsToICloudAsync(JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(this);
            await Task.Run(async delegate
            {
                await SyncSettingsToICloudAsync(settings: this, context, sharedName);
            });
        }

        public void SyncSettingsToICloud<T>(Expression<Func<IMauiAppSettingsService, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(this);
            SyncSettingsToICloud(settings: this, value: value, context, sharedName);
        }

        public async Task SyncSettingsToICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Save, context, target: MauiAppSettingsTarget.ICloud, sharedName: sharedName);
            });
        }
        public void SyncSettingsToICloud<T>(object settings, Expression<Func<IMauiAppSettingsService, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiAppSettingsActions.Save, context, target: MauiAppSettingsTarget.ICloud, sharedName);
        }

        #endregion

        #region Load

        public async Task SyncSettingsFromICloudAsync(JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(this);
            await Task.Run(async delegate
            {
                await SyncSettingsFromICloudAsync(settings: this, context, sharedName);
            });
        }

        public void SyncSettingsFromICloud<T>(Expression<Func<IMauiAppSettingsService, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(this);
            SyncSettingsFromICloud(settings: this, value: value, context, sharedName);
        }

        public async Task SyncSettingsFromICloudAsync(object settings, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: MauiAppSettingsActions.Load, context, target: MauiAppSettingsTarget.ICloud, sharedName: sharedName);
            });
        }
        public void SyncSettingsFromICloud<T>(object settings, Expression<Func<IMauiAppSettingsService, T>> value, JsonSerializerContext context, string? sharedName = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            GetExpressionMeta(settings: settings, value: value, mode: MauiAppSettingsActions.Load, context, target: MauiAppSettingsTarget.ICloud, sharedName: sharedName);
        }
        #endregion

        #endregion
    }
}