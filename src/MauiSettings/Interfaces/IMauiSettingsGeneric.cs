using System.Linq.Expressions;

namespace AndreasReitberger.Maui.Interfaces
{
    public interface IMauiSettingsGeneric<SO>
    {
        public static SO? SettingsObject { get; }
        public static bool ThrowIfSettingsObjectIsNull { get; set; }

        #region Methods
        public static abstract void LoadSettings();
        public static abstract void LoadSettings<T>(Expression<Func<SO, T>> value);
        public static abstract Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null);
        #endregion
    }
}
