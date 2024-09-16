using MauiSettings.Example.ViewModels;

namespace MauiSettings.Example
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsPageViewModel();
        }
    }

}
