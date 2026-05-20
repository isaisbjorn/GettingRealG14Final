using System.Windows;
using wpfGettingRealG14This.ViewModels;

namespace wpfGettingRealG14This.Views
{
	public partial class LoginView : Window
	{
		// Vi gemmer en reference til ViewModel
		// så vi kan sende koden til den fra code-behind
		private readonly LoginViewModel _viewModel;

		public LoginView(LoginViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = viewModel;
		}

		// Kaldes når brugeren klikker Sign in
		// Vi sender kodeordet fra PasswordBox til ViewModel
		// INDEN ViewModel tjekker login
		// Dette er nødvendigt fordi PasswordBox ikke
		// understøtter normal data binding af sikkerhedsgrunde 
		// på vores buværende niveau, er det med sikkerhed ikke så relevant, mwn det
		// er en god øvelse at prøve at håndtere det korrekt.
		private void LoginKnap_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Password = KodeFelt.Password;
		}
	}
}
