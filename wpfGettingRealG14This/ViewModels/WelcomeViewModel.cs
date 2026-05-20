using GetReal;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{
    // WelcomeViewModel styrer velkomstskærmen.
    // Den modtager et repository så den kan vise dagens aftaler
    // i højre side af vinduet med det samme ved login.
    public class WelcomeViewModel : INotifyPropertyChanged
    {
        // Bruges til at indlæse dagens aftaler fra JSON
        private readonly IRepository<Client> _clientRepository;

        // Danske måneds- og dagnavne til datooverskriften
        private static readonly string[] MonthNames =
        {
            "januar", "februar", "marts", "april", "maj", "juni",
            "juli", "august", "september", "oktober", "november", "december"
        };
        private static readonly string[] DayNames =
        {
            "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag"
        };

        // =====================
        // PROPERTIES
        // =====================

        // Brugernavnet vises i menubjælken som "Velkommen, admin"
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        // Overskrift over dagens aftaler, fx "Fredag d. 16. maj"
        private string _todaysDateLabel;
        public string TodaysDateLabel
        {
            get { return _todaysDateLabel; }
            set { _todaysDateLabel = value; OnPropertyChanged(); }
        }

        // Listen over aftaler der er booket til i dag
        // AppointmentInfo er defineret i CalendarViewModel.cs i samme namespace
        private ObservableCollection<AppointmentInfo> _todaysAppointments;
        public ObservableCollection<AppointmentInfo> TodaysAppointments
        {
            get { return _todaysAppointments; }
            set { _todaysAppointments = value; OnPropertyChanged(); }
        }

        // =====================
        // COMMANDS
        // =====================
        public ICommand OpenClientsCommand { get; }
        public ICommand OpenAppointmentsCommand { get; }
        public ICommand OpenExercisesCommand { get; }
        public ICommand OpenCalendarCommand { get; }
        // Logger brugeren ud og returnerer til login-skærmen
        public ICommand LogOutCommand { get; }

        // =====================
        // EVENTS
        // =====================
        public event Action NavigateToClients;
        public event Action NavigateToAppointments;
        public event Action NavigateToExercises;
        public event Action NavigateToCalendar;
        public event Action NavigateToLogOut;

        // =====================
        // KONSTRUKTØR
        // =====================

        // Modtager brugernavn fra login og repository til at slå aftaler op
        public WelcomeViewModel(string username, IRepository<Client> clientRepository)
        {
            Username = username;
            _clientRepository = clientRepository;

            OpenClientsCommand      = new RelayCommand(() => NavigateToClients?.Invoke());
            OpenAppointmentsCommand = new RelayCommand(() => NavigateToAppointments?.Invoke());
            OpenExercisesCommand    = new RelayCommand(() => NavigateToExercises?.Invoke());
            OpenCalendarCommand     = new RelayCommand(() => NavigateToCalendar?.Invoke());
            LogOutCommand           = new RelayCommand(() => NavigateToLogOut?.Invoke());

            // Indlæs dagens aftaler med det samme ved opstart
            LoadTodaysAppointments();
        }

        // =====================
        // PRIVATE METODER
        // =====================

        // Finder alle aftaler der er booket til dags dato
        // og bygger datooverskriften i dansk format
        private void LoadTodaysAppointments()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Byg datooverskriften: "Fredag d. 16. maj"
            int dayIndex = ((int)today.DayOfWeek == 0)
                ? 6
                : (int)today.DayOfWeek - 1;
            TodaysDateLabel = $"{DayNames[dayIndex]} d. {today.Day}. " +
                              $"{MonthNames[today.Month - 1]}";

            // Saml alle aftaler på tværs af alle klienter der matcher dags dato
            var list = new List<AppointmentInfo>();
            foreach (var client in _clientRepository.GetAll())
            {
                foreach (var course in client.TreatmentCourses)
                {
                    foreach (var appt in course.Appointments)
                    {
                        if (appt.Date == today)
                        {
                            list.Add(new AppointmentInfo
                            {
                                TimeSlot   = $"{appt.StartTime:HH\\:mm} - {appt.EndTime:HH\\:mm}",
                                ClientName = $"{client.FirstName} {client.LastName}"
                            });
                        }
                    }
                }
            }

            // Sorter efter starttidspunkt og gem i property
            TodaysAppointments = new ObservableCollection<AppointmentInfo>(
                list.OrderBy(a => a.TimeSlot));
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
