namespace AndreasReitberger.Maui.Events
{
    public partial class EncryptionErrorEventArgs : EventArgs
    {
        public Exception? Exception { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}
