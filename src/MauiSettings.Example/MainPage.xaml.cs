using MauiSettings.Example.ViewModels;

namespace MauiSettings.Example
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
            Loaded += ((MainPageViewModel)BindingContext).Pages_Loaded;
        }

        ~MainPage()
        {
            Loaded -= ((MainPageViewModel)BindingContext).Pages_Loaded;
        }
    }

}
