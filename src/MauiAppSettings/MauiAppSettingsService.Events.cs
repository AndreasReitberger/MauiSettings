using AndreasReitberger.Maui.Events;
using AndreasReitberger.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AndreasReitberger.Maui
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    public partial class MauiAppSettingsService<SO> : ObservableObject, IMauiAppSettingsService<SO> where SO : new()
    {
        #region Events

        public event EventHandler? ErrorEvent;
        protected void OnErrorEvent(ErrorEventArgs e)
        {
            ErrorEvent?.Invoke(typeof(MauiAppSettingsService<SO>), e);
        }

        public event EventHandler? EncryptionErrorEvent;
        protected void OnEncryptionErrorEvent(EncryptionErrorEventArgs e)
        {
            EncryptionErrorEvent?.Invoke(typeof(MauiAppSettingsService<SO>), e);
        }
        #endregion
    }
}
