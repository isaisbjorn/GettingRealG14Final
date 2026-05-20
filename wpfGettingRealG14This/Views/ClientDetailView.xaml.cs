using System.Windows;
using wpfGettingRealG14This.ViewModels;

namespace wpfGettingRealG14This.Views
{
    public partial class ClientDetailView : Window
    {
        public ClientDetailView(ClientDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Lukker dette vindue og returnerer til ClientView
        private void TilbageKnap_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
