using MauiSettings.Example.ViewModels;

namespace MauiSettings.Example
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            Loaded += ((MainPageViewModel)BindingContext).Pages_Loaded;
        }

        ~MainPage()
        {
            Loaded -= ((MainPageViewModel)BindingContext).Pages_Loaded;
        }
    }

}
