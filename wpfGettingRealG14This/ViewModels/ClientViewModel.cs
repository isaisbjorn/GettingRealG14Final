using GetReal;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{
	public class ClientViewModel : INotifyPropertyChanged
	{
		private readonly ClientService _clientService;
		private readonly IRepository<Client> _clientRepository;

		// =====================
		// PROPERTIES TIL VISNING
		// =====================
		private ObservableCollection<Client> _clients; // ObservableCollection er en liste der automatisk opdaterer UI når den ændres
		public ObservableCollection<Client> Clients
		{
			get { return _clients; }
			set { _clients = value; OnPropertyChanged(); }
		}

		private Client _selectedClient;
		public Client SelectedClient
		{
			get { return _selectedClient; }
			set { _selectedClient = value; OnPropertyChanged(); }
		}

		// =====================
		// PROPERTIES TIL INPUTFELTER
		// =====================
		private string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; OnPropertyChanged(); }
		}

		private string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; OnPropertyChanged(); }
		}

		private string _phone;
		public string Phone
		{
			get { return _phone; }
			set { _phone = value; OnPropertyChanged(); }
		}

		private string _email;
		public string Email
		{
			get { return _email; }
			set { _email = value; OnPropertyChanged(); }
		}

		private string _searchString;
		public string SearchString
		{
			get { return _searchString; }
			set
			{
				_searchString = value;
				OnPropertyChanged();
				// Søg automatisk mens brugeren skriver
				SearchClients();
			}
		}

		// =====================
		// COMMANDS
		// =====================
		public ICommand CreateClientCommand { get; }
		public ICommand DeleteClientCommand { get; }
		// Åbner detaljevinduet for den valgte klient
		public ICommand OpenDetailCommand { get; }

		// =====================
		// EVENT
		// =====================
		// App.xaml.cs lytter på dette og åbner ClientDetailView med den valgte klient
		public event Action<Client> NavigateToDetail;

		// =====================
		// KONSTRUKTØR
		// =====================

		// Vi modtager både service og repository
		// fordi service mangler GetAll() og Save() — dem kalder vi på repository
		public ClientViewModel(ClientService clientService, IRepository<Client> clientRepository)
		{
			_clientService = clientService;
			_clientRepository = clientRepository;

			CreateClientCommand = new RelayCommand(CreateClient, CanCreateClient);
			DeleteClientCommand = new RelayCommand(DeleteClient, () => SelectedClient != null);
			// OpenDetailCommand — altid aktiv, men OpenDetail() tjekker selv om noget er valgt
			OpenDetailCommand = new RelayCommand(OpenDetail);

			LoadClients();
		}

		// =====================
		// PRIVATE METODER
		// =====================
		private void LoadClients()
		{
			// GetAll() henter alle klienter fra JSON-filen
			var list = _clientRepository.GetAll();
			Clients = new ObservableCollection<Client>(list);
		}

		private void CreateClient()
		{
			_clientService.CreateClient(
				FirstName,
				LastName,
				Phone,
				Email,
				DateOnly.FromDateTime(DateTime.Today)
			);

			// Save() gemmer de opdaterede data til JSON
			_clientRepository.Save();
			LoadClients();
			ClearForm();
		}

		private bool CanCreateClient()
		{
			// Alle fire felter skal være udfyldt inden knappen bliver aktiv
			return !string.IsNullOrWhiteSpace(FirstName)
				&& !string.IsNullOrWhiteSpace(LastName)
				&& !string.IsNullOrWhiteSpace(Phone)
				&& !string.IsNullOrWhiteSpace(Email);
		}

		private void DeleteClient()
		{
			if (SelectedClient == null) return;

			// Metoden hedder RemoveClient i service-laget
			_clientService.RemoveClient(SelectedClient.Id);
			_clientRepository.Save();
			LoadClients();
		}

		private void SearchClients()
		{
			if (string.IsNullOrWhiteSpace(SearchString))
			{
				// Tomt søgefelt = vis alle klienter igen
				LoadClients();
				return;
			}
			var results = _clientService.SearchClient(SearchString);
			Clients = new ObservableCollection<Client>(results);
		}

		private void ClearForm()
		{
			FirstName = string.Empty;
			LastName  = string.Empty;
			Phone     = string.Empty;
			Email     = string.Empty;
		}

		// Fyrer NavigateToDetail-eventet med den valgte klient
		// App.xaml.cs opfanger det og åbner ClientDetailView
		private void OpenDetail()
		{
			if (SelectedClient != null)
				NavigateToDetail?.Invoke(SelectedClient);
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
