using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{
	public class LoginViewModel : INotifyPropertyChanged
	{
		// De to hardcodede værdier der godkendes som "korrekt login"
		// I en rigtig app ville disse aldrig ligge direkte i koden sådan her
		private const string CorrectUsername = "admin";
		private const string CorrectPassword = "1234";

		// =====================
		// PROPERTIES
		// =====================

		// Binder til brugernavnsfeltet i XAML
		private string _username;
		public string Username
		{
			get { return _username; }
			set { _username = value; OnPropertyChanged(); }
		}

		// Koden kan ikke bindes direkte fra PasswordBox
		// via normal binding — se forklaring i XAML-filen
		// Vi sætter den manuelt fra code-behind i stedet
		public string Password { get; set; }

		// Fejlbesked der vises hvis login fejler
		private string _errorMessage;
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; OnPropertyChanged(); }
		}

		// =====================
		// COMMANDS
		// =====================
		public ICommand LoginCommand { get; }

		// =====================
		// EVENT — fortæller App.xaml.cs at login lykkedes
		// =====================

		// Action er en simpel delegate — en reference til en metode
		// App.xaml.cs tilmelder sig dette event og åbner WelcomeView
		// når det fyres af
		public event Action LoginSucceeded;

		// =====================
		// KONSTRUKTØR
		// =====================
		public LoginViewModel()
		{
			LoginCommand = new RelayCommand(CheckLogin);
		}

		// =====================
		// PRIVATE METODER
		// =====================
		private void CheckLogin()
		{
			// Sammenlign det indtastede med de hardcodede værdier
			// Trim() fjerner mellemrum før og efter teksten
			if (Username?.Trim() == CorrectUsername &&
				Password == CorrectPassword)
			{
				// Login OK — fyr eventet så App.xaml.cs kan reagere
				LoginSucceeded?.Invoke();
			}
			else
			{
				// Login fejlede — vis fejlbesked i vinduet
				ErrorMessage = "Forkert brugernavn eller adgangskode.";
			}
		}

		// =====================
		// INOTIFYPROPERTYCHANGED
		// =====================
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
