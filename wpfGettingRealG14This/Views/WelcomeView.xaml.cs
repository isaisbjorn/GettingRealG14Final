using System.Windows;
using wpfGettingRealG14This.ViewModels;

namespace wpfGettingRealG14This.Views
{
    // Code-behind til WelcomeView.
    // Den er meget simpel fordi al logik ligger i WelcomeViewModel.
    // Vi behøver kun at sætte DataContext — resten klarer binding i XAML.
    public partial class WelcomeView : Window
    {
        // Konstruktøren modtager WelcomeViewModel fra App.xaml.cs
        public WelcomeView(WelcomeViewModel viewModel)
        {
            InitializeComponent();

            // DataContext fortæller vinduet hvilken ViewModel
            // alle {Binding ...} i XAML skal hente data fra
            DataContext = viewModel;
        }
    }
}
