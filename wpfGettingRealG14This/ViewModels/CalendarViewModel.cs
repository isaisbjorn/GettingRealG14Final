using GetReal;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{

	// Denne klasse er en hjælpeklasse der kun bruges til at vise kalenderen i CalendarViewModel.
	// Repræsenterer ét felt i kalendergitteret — én dag.
	// Vi laver 42 af dem (6 rækker × 7 kolonner) og binder dem til XAML.
	public class DayDisplay
    {
		// Hvilken dato feltet repræsenterer. DateOnly er en type der kun
        // indeholder dato (år, måned, dag) uden klokkeslæt.
		public DateOnly Date { get; set; }

        // True hvis der ligger mindst én aftale på denne dato
        // — bruges til at vise eller skjule den røde prik i XAML
        public bool HasAppointments { get; set; }

		// False hvis feltet tilhører forrige eller næste måned
		// — bruges til at tone feltet i XAML så det er tydeligt at det ikke er en del af den aktuelle måned
		// Fx hvis vi viser maj, så vil de første 2 felter i gitteret være 29. og 30. april, og de sidste
        // 3 felter være 1., 2. og 3. juni.
		public bool IsInCurrentMonth { get; set; }
    }

	// AppointmentInfo er endnu en simpel hjælpeklasse der bruges til at vise aftaler
    // i aftalelisten i højre side af CalendarView.
	// En simpel visningsklasse til aftalelisten i højre side.
	// Pakker klientnavn og tidspunkt ind i ét objekt.
	// Bruges også i WelcomeViewModel til at vise dagens aftaler.
	public class AppointmentInfo
    {
        public string TimeSlot { get; set; }   // fx "10:00 - 11:00"
        public string ClientName { get; set; } // fx "Jens Hansen"
    }

	// Styrer logik for kalendervinduet:
	//   - Hvilken måned der vises
	//   - Bygger de 42 dag-felter
	//   - Hvilken dag brugeren har valgt
	//   - Listen over aftaler for den valgte dag
	public class CalendarViewModel : INotifyPropertyChanged
    {
        // Repository bruges til at hente alle klienter og deres aftaler
        private readonly IRepository<Client> _clientRepository;

        // Hvilken måned og år der vises lige nu
        private int _year;
        private int _month;

        // Danske månedsnavne — bruges i overskriften fx "Maj 2026"
        private static readonly string[] MonthNames =
        {
            "Januar", "Februar", "Marts", "April", "Maj", "Juni",
            "Juli", "August", "September", "Oktober", "November", "December"
        };

        // Danske dagnavne startende fra mandag (=indeks 0)
        // DayOfWeek i C# starter fra søndag — så vi justerer i koden nedenfor
        private static readonly string[] DayNames =
        {
            "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag"
        };


		// PROPERTIES

		// De 42 dag-felter der vises i gitteret
		// ObservableCollection opdaterer automatisk UI når listen ændres fx ved
        // at trykke videre til en ny måned
		private ObservableCollection<DayDisplay> _days;
        public ObservableCollection<DayDisplay> Days
        {
            get { return _days; }
            set { _days = value; OnPropertyChanged(); }
        }

        // Den dag brugeren har klikket på
        // Når denne ændres, indlæses aftalerne for den dag automatisk
        private DayDisplay _selectedDay;
        public DayDisplay SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                _selectedDay = value;
                OnPropertyChanged();

                // Opdater overskrift og aftaleliste baseret på valget
                if (value != null)
                {
                    // Beregn ugedagsindeks med mandag som 0
                    // C# giver: søndag=0, mandag=1... — vi retter til mandag=0
                    int dayIndex = ((int)value.Date.DayOfWeek == 0)
                        ? 6
                        : (int)value.Date.DayOfWeek - 1;

                    // Byg en pæn datostreng, fx "Torsdag d. 15. maj"
                    SelectedDateLabel = $"{DayNames[dayIndex]} d. {value.Date.Day}. " +
                                       $"{MonthNames[value.Date.Month - 1].ToLower()}";

                    LoadAppointments(value.Date);
                }
                else
                {
                    // Ingen dag valgt — nulstil tekst og liste
                    SelectedDateLabel = "Vælg en dag i kalenderen";
                    Appointments = new ObservableCollection<AppointmentInfo>();
                }
            }
        }

        // Overskrift over aftalelisten, fx "Torsdag d. 15. maj"
        private string _selectedDateLabel = "Vælg en dag i kalenderen";
        public string SelectedDateLabel
        {
            get { return _selectedDateLabel; }
            set { _selectedDateLabel = value; OnPropertyChanged(); }
        }

		// Overskrift over navigationen, fx "Maj 2026"
		// Vi bruger => (expression body) — det er bare en kort get-property der beregner en streng
        // baseret på _month og _year fra ovenfor. den har ingen setter fordi den ikke skal sættes udefra,
        // den opdateres automatisk når vi ændrer måned eller år.
		public string MonthLabel => $"{MonthNames[_month - 1]} {_year}";

        // Listen af aftaler for den valgte dag, sorteret efter starttidspunkt
        private ObservableCollection<AppointmentInfo> _appointments;
        public ObservableCollection<AppointmentInfo> Appointments
        {
            get { return _appointments; }
            set { _appointments = value; OnPropertyChanged(); }
        }

        // =====================
        // COMMANDS
        // =====================

        // Knapperne til at bladre frem og tilbage i månederne
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }

        // =====================
        // KONSTRUKTØR
        // =====================
        public CalendarViewModel(IRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;

            // Start med den aktuelle måned
            _year  = DateTime.Today.Year;
            _month = DateTime.Today.Month;

            PreviousMonthCommand = new RelayCommand(PreviousMonth);
            NextMonthCommand     = new RelayCommand(NextMonth);

            // Nulstil aftalelisten og byg den første kalender
            Appointments = new ObservableCollection<AppointmentInfo>();
            BuildCalendar();
        }

        // =====================
        // PRIVATE METODER
        // =====================

        // Bladrer én måned tilbage og genopbygger kalenderen
        private void PreviousMonth()
        {
            _month--;
            // Hvis vi går forbi januar, hopper vi til december året før
            if (_month < 1) { _month = 12; _year--; }

            // Vi skal fortælle UI at MonthLabel (som ikke har en setter) er ændret
            OnPropertyChanged(nameof(MonthLabel));
            BuildCalendar();
        }

        // Bladrer én måned frem og genopbygger kalenderen
        private void NextMonth()
        {
            _month++;
            // Hvis vi går forbi december, hopper vi til januar året efter
            if (_month > 12) { _month = 1; _year++; }

            OnPropertyChanged(nameof(MonthLabel));
            BuildCalendar();
        }

        // Bygger listen af 42 DayDisplay-objekter til gitteret
        // 6 rækker × 7 kolonner = 42 felter i alt
        private void BuildCalendar()
        {
            var days = new ObservableCollection<DayDisplay>();

            // Find den første dag i den aktuelle måned
            var firstDay = new DateOnly(_year, _month, 1);

            // Find ud af hvilken ugedag måneden starter på
            // DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
            // Vi vil have mandag som første kolonne (indeks 0)
            int weekday = (int)firstDay.DayOfWeek;
            int startOffset = (weekday == 0) ? 6 : weekday - 1;

            // Gå startOffset dage tilbage for at finde gitterets første felt
            // Eks: hvis måneden starter på onsdag (offset=2), starter gitteret mandag
            var startDate = firstDay.AddDays(-startOffset);

            // Tilføj 42 felter — kan indeholde dage fra forrige/næste måned
            for (int i = 0; i < 42; i++)
            {
                var date = startDate.AddDays(i);
                days.Add(new DayDisplay
                {
                    Date = date,
                    // Sæt til false hvis datoen er udenfor den aktuelle måned
                    IsInCurrentMonth = date.Month == _month && date.Year == _year,
                    // Tjek om der er mindst én aftale på denne dato
                    HasAppointments = DayHasAppointments(date)
                });
            }

            Days = days;

            // Nulstil valg og aftaleliste når vi skifter måned
            SelectedDay = null;
        }

        // Returnerer true hvis der ligger mindst én aftale på den givne dato
        // SelectMany "flader" listerne ud — vi går fra klienter → forløb → aftaler
        private bool DayHasAppointments(DateOnly date)
        {
            return _clientRepository.GetAll()
                .SelectMany(k => k.TreatmentCourses)
                .SelectMany(f => f.Appointments)
                .Any(a => a.Date == date);
        }

        // Henter alle aftaler på den valgte dato på tværs af alle klienter
        // og sorterer dem efter starttidspunkt
        private void LoadAppointments(DateOnly date)
        {
            var list = new List<AppointmentInfo>();

            // Gå igennem alle klienter, alle forløb, alle aftaler
            foreach (var client in _clientRepository.GetAll())
            {
                foreach (var course in client.TreatmentCourses)
                {
                    foreach (var appt in course.Appointments)
                    {
                        if (appt.Date == date)
                        {
                            list.Add(new AppointmentInfo
                            {
                                // Formater tidspunktet som "10:00 - 11:00"
                                TimeSlot   = $"{appt.StartTime:HH\\:mm} - {appt.EndTime:HH\\:mm}",
                                ClientName = $"{client.FirstName} {client.LastName}"
                            });
                        }
                    }
                }
            }

            // Sorter listen så de tidligste aftaler vises øverst
            Appointments = new ObservableCollection<AppointmentInfo>(
                list.OrderBy(a => a.TimeSlot));
        }

        // =====================
        // INOTIFYPROPERTYCHANGED
        // Fortæller UI at en property er ændret så den kan opdatere sig selv
        // =====================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
