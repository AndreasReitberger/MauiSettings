namespace AndreasReitberger.Maui.Interfaces
{
    public interface IMauiAppSettings<T> : IMauiAppSettingsService//<T> where T : new()
    {
        #region Properties
        public bool SettingsChanged { get; set; }
        #endregion

        #region Methods
        public void OpenDeviceSettings();
        #endregion
    }
}
