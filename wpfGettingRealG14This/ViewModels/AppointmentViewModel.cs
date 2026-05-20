using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GetReal;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{
    // HJÆLPEKLASSE: AppointmentDisplay
    // Pakker én aftale sammen med klientnavnet, så vi nemt kan vise fx
    // "Jens Hansen — 10:00 - 11:00" i listen i vinduet.
    public class AppointmentDisplay
    {
        public string ClientName { get; set; }
        public string TimeSlot { get; set; }   // fx "10:00 - 11:00"
        public DateOnly Date { get; set; }
        public Appointment Appointment { get; set; }
    }

    // APPOINTMENTVIEWMODEL
    // Styrer logikken bag aftale-vinduet:
    //   - Henter klienter og behandlingsforløb til dropdowns
    //   - Validerer input (tidspunkter, valg)
    //   - Opretter aftaler og gemmer til JSON
    //   - Viser alle eksisterende aftaler sorteret efter dato og tid
    public class AppointmentViewModel : INotifyPropertyChanged
    {
        private readonly AppointmentService _appointmentService;

        // Vi bruger repository direkte til GetAll() og Save()
        // fordi AppointmentService ikke har en save-metode
        private readonly IRepository<Client> _clientRepository;

        // =====================
        // PROPPERTIES
        // =====================

        // Listen af alle klienter — vises i dropdown øverst i formularen
        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set { _clients = value; OnPropertyChanged(); }
        }

		// Den klient brugeren har valgt i dropdown
		// Når den ændres via metoden OnPropertyChanged(),
        // kaldes LoadCourses() automatisk, så forløbs-dropdown opdateres
        // til at vise forløbene for den valgte klient.
		private Client _selectedClient;
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
                LoadCourses();
            }
        }

        // Listen af behandlingsforløb for den valgte klient
        private ObservableCollection<TreatmentCourse> _courses;
        public ObservableCollection<TreatmentCourse> Courses
        {
            get { return _courses; }
            set { _courses = value; OnPropertyChanged(); }
        }

        // Det behandlingsforløb aftalen skal tilknyttes
        private TreatmentCourse _selectedCourse;
        public TreatmentCourse SelectedCourse
        {
            get { return _selectedCourse; }
            set { _selectedCourse = value; OnPropertyChanged(); }
        }

        // Dato for den nye aftale — binder til DatePicker i XAML
        // Starter på dags dato som standard
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        // Starttid som tekst — brugeren skriver fx "10:00"
        // Vi parser det til TimeOnly i CreateAppointment()
        private string _startTime;
        public string StartTime
        {
            get { return _startTime; }
            set { _startTime = value; OnPropertyChanged(); }
        }

        // Sluttid som tekst — fx "11:00"
        private string _endTime;
        public string EndTime
        {
            get { return _endTime; }
            set { _endTime = value; OnPropertyChanged(); } 
        }

        // Rød fejlbesked — vises hvis noget gik galt
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // Grøn bekræftelse — vises når aftalen er gemt
        private string _confirmation;
        public string Confirmation
        {
            get { return _confirmation; }
            set { _confirmation = value; OnPropertyChanged(); }
        }

        // Alle aftaler på tværs af alle klienter, sorteret efter dato og tid
        // Vises i listen til højre i vinduet
        private ObservableCollection<AppointmentDisplay> _allAppointments;
        public ObservableCollection<AppointmentDisplay> AllAppointments
        {
            get { return _allAppointments; }
            set { _allAppointments = value; OnPropertyChanged(); }
        }

        // =====================
        // COMMANDS
        // =====================

        // Aktiv kun når alle felter er udfyldt — CanCreateAppointment() bestemmer det
        public ICommand CreateAppointmentCommand { get; }

        // =====================
        // KONSTRUKTØR
        // =====================
        public AppointmentViewModel(
            AppointmentService appointmentService,
            IRepository<Client> clientRepository)
        {
            _appointmentService = appointmentService;
            _clientRepository = clientRepository;

            // Anden parameter til RelayCommand er CanExecute-tjekket
            CreateAppointmentCommand = new RelayCommand(CreateAppointment, CanCreateAppointment);

            LoadClients();
            LoadAllAppointments();
        }

        // =====================
        // PRIVATE METODER
        // =====================

        // Henter alle klienter fra JSON og fylder dropdown-listen
        private void LoadClients()
        {
            var list = _clientRepository.GetAll();
            Clients = new ObservableCollection<Client>(list);
        }

        // Opdaterer forløbs-dropdown når en ny klient vælges
        private void LoadCourses()
        {
            if (SelectedClient == null)
            {
                Courses = new ObservableCollection<TreatmentCourse>();
                return;
            }

            // Hent forløbene fra den valgte klient og vælg det første automatisk
            Courses = new ObservableCollection<TreatmentCourse>(SelectedClient.TreatmentCourses);
            SelectedCourse = Courses.FirstOrDefault();
        }

        // Bygger listen over alle aftaler på tværs af alle klienter
        // Kaldes igen efter en ny aftale oprettes, så listen opdateres
        public void LoadAllAppointments()
        {
            var allAppts = new ObservableCollection<AppointmentDisplay>();

            foreach (var client in _clientRepository.GetAll())
            {
                foreach (var course in client.TreatmentCourses)
                {
                    foreach (var appt in course.Appointments)
                    {
                        allAppts.Add(new AppointmentDisplay
                        {
                            ClientName  = $"{client.FirstName} {client.LastName}",
                            TimeSlot    = $"{appt.StartTime:HH\\:mm} - {appt.EndTime:HH\\:mm}",
                            Date        = appt.Date,
                            Appointment = appt
                        });
                    }
                }
            }

            // Sorter: tidligste dato øverst, og inden for samme dato tidligste tid øverst
            AllAppointments = new ObservableCollection<AppointmentDisplay>(
                allAppts
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Appointment.StartTime));
        }

        // Knappen er kun aktiv når klient, forløb, start- og sluttid er udfyldt
        private bool CanCreateAppointment()
        {
            return SelectedClient != null
                && SelectedCourse != null
                && !string.IsNullOrWhiteSpace(StartTime)
                && !string.IsNullOrWhiteSpace(EndTime);
        }

		// Opretter aftalen og gemmer til JSON via repository der opdaterer hele klienten
        // (inklusive det nye forløb og aftale) — det er sådan vores data er struktureret,
        // så vi må opdatere hele klienten for at gemme en ny aftale.
		private void CreateAppointment()
        {
            // Valider at tidspunkterne er i korrekt format (HH:mm)
            if (!TimeOnly.TryParse(StartTime, out TimeOnly start))
            {
                ErrorMessage = "Ugyldigt starttidspunkt — brug formatet HH:mm";
                return;
            }
            if (!TimeOnly.TryParse(EndTime, out TimeOnly end))
            {
                ErrorMessage = "Ugyldigt sluttidspunkt — brug formatet HH:mm";
                return;
            }

			// ID bruges kun til at identificere aftaler internt i JSON-filen — det vises ikke i UI og har ingen betydning for brugeren.
			// det er godt at have id på aftalerne, så vi kan skelne dem fra hinanden i JSON-filen, og så vi kan udvide funktionaliteten
			// senere (fx redigere eller slette aftaler).
			// Find næste ledige ID manuelt — AppointmentService.GenerateAppointmentId()
			// crasher hvis der ingen aftaler er i forvejen (Max() på tom liste).
			// Vi beregner det selv ved at hente alle eksisterende aftaler og finde det højeste ID, og så lægge 1 til for den nye aftale.
			var existingAppointments = _clientRepository.GetAll()
                .SelectMany(k => k.TreatmentCourses)
                .SelectMany(f => f.Appointments)
                .ToList();
            int nextId = existingAppointments.Any()
                ? existingAppointments.Max(a => a.AppointmentId) + 1
                : 1;

            var appt = new Appointment 
            {
                AppointmentId = nextId,
                Date          = DateOnly.FromDateTime(SelectedDate),
                StartTime     = start,
                EndTime       = end
            };

            // Tilføj aftalen direkte til behandlingsforløbet og gem
            // (SelectedCourse er en reference til det faktiske objekt i repository)
            SelectedCourse.Appointments.Add(appt);
            _clientRepository.Update(SelectedClient);
            _clientRepository.Save();

            LoadAllAppointments();
            ErrorMessage = "";
            Confirmation = "Aftale oprettet!";
        }

		// =====================
		// INOTIFYPROPERTYCHANGED 
		// =====================
		// Dette interface og event er nødvendigt for at WPF kan opdatere UI når properties ændres.fx
		// når vi sætter ErrorMessage = "Ugyldigt starttidspunkt", så skal det vises i UI — det sker via dette event.
		// Interfacet er ikke noget vi selv laver men en del af .NET og WPF, og det er sådan WPF ved hvornår det skal opdatere UI'et.
		public event PropertyChangedEventHandler? PropertyChanged;  
		protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
