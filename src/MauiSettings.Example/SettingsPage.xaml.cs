using MauiSettings.Example.ViewModels;

namespace MauiSettings.Example
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel; 
            //Loaded += ((SettingsPageViewModel)BindingContext).Pages_Loaded;
        }

        ~SettingsPage()
        {
            //Loaded -= ((SettingsPageViewModel)BindingContext).Pages_Loaded;
        }
    }

}
