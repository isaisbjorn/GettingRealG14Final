using System.Windows;
using wpfGettingRealG14This.ViewModels;

namespace wpfGettingRealG14This.Views
{

    public partial class ClientView : Window
    {
        public ClientView(ClientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Lukker dette vindue — WelcomeView er stadig åben bag ved
        private void TilbageKnap_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
