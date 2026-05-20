using GetReal;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using wpfGettingRealG14This.Helpers;

namespace wpfGettingRealG14This.ViewModels
{
    // En enkelt aftale til listen i klient-detalje vinduet
    public class ClientAppointmentDisplay
    {
        public string DateLabel { get; set; }   // fx "17/05/2026"
        public string TimeSlot { get; set; }    // fx "10:00 - 11:00"
        public string CourseName { get; set; }  // behandlingsforløbets navn/issue
        public DateOnly SortDate { get; set; }  // bruges kun til sortering, vises ikke
    }

    // =====================================================================
    // CLIENTDETAILVIEWMODEL
    // Styrer klient-detalje vinduet.
    // Viser kontaktinfo, alle aftaler og en journalfunktion for én klient.
    // =====================================================================
    public class ClientDetailViewModel : INotifyPropertyChanged
    {
        private readonly Client _client;

		// Stien til filen vi gemmer noter i — samme mappe som clients.json men en anden fil
		// da jeg ikke ville rode i den fil der indeholder klientdata, men det havde self været det bedste at gøre.
		private const string NotesFilePath = "clientnotes.json";

        // =====================
        // PROPERTIES
        // =====================

        // Klientens fulde navn — vises som overskrift øverst i vinduet
        // Vi bruger => her fordi det er bare en simpel read-only beregning
        public string FullName => $"{_client.FirstName} {_client.LastName}";
        public string Phone    => _client.Phone;
        public string Email    => _client.Email;

        // Liste over alle aftaler for denne klient, på tværs af alle forløb
        private ObservableCollection<ClientAppointmentDisplay> _appointments;
        public ObservableCollection<ClientAppointmentDisplay> Appointments
        {
            get { return _appointments; }
            set { _appointments = value; OnPropertyChanged(); }
        }

        // Den samlede notattekst for denne klient (hele historikken)
        // Vises i en read-only scrollbar TextBox i vinduet
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; OnPropertyChanged(); }
        }

        // Det brugeren skriver i inputfeltet — den nye note der skal tilføjes
        private string _newNoteText;
        public string NewNoteText
        {
            get { return _newNoteText; }
            set { _newNoteText = value; OnPropertyChanged(); }
        }

        // =====================
        // COMMANDS
        // =====================

        // Gem-knappen er kun aktiv når der er tekst i inputfeltet
        public ICommand SaveNoteCommand { get; }

        // =====================
        // KONSTRUKTØR
        // =====================
        public ClientDetailViewModel(Client client)
        {
            _client = client;
            SaveNoteCommand = new RelayCommand(SaveNote, CanSaveNote);
            LoadAppointments();
            LoadNotes();
        }

        // =====================
        // PRIVATE METODER
        // =====================

        // Samler alle aftaler fra alle behandlingsforløb for klienten
        private void LoadAppointments()
        {
            var list = new List<ClientAppointmentDisplay>();

            foreach (var course in _client.TreatmentCourses)
            {
                foreach (var appt in course.Appointments)
                {
                    list.Add(new ClientAppointmentDisplay
                    {
                        DateLabel  = appt.Date.ToString("dd/MM/yyyy"),
                        TimeSlot   = $"{appt.StartTime:HH\\:mm} - {appt.EndTime:HH\\:mm}",
                        CourseName = course.Issue ?? "Ukendt forløb",
                        SortDate   = appt.Date
                    });
                }
            }

            // Nyeste aftaler øverst — vi sorterer faldende på dato
            Appointments = new ObservableCollection<ClientAppointmentDisplay>(
                list.OrderByDescending(a => a.SortDate));
        }

        // Læser eksisterende noter for denne klient fra JSON-filen
        private void LoadNotes()
        {
            var dict = ReadNotesFile();
            // TryGetValue returnerer false hvis klienten ikke har nogen noter endnu
            Notes = dict.TryGetValue(_client.Id, out string existing)
                ? existing
                : string.Empty;
        }

        // Tilføjer en ny note med tidsstempel øverst i historikken og gemmer
        private void SaveNote()
        {
            if (string.IsNullOrWhiteSpace(NewNoteText)) return;

            // Lav et tidsstempel i dansk datoformat
            string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string entry = $"[{timestamp}]\n{NewNoteText.Trim()}\n\n";

            // Sæt den nye note øverst — så er den nyeste altid øverst i historikken
            Notes = entry + Notes;
            NewNoteText = string.Empty;

            // Gem den opdaterede notattekst til filen
            var dict = ReadNotesFile();
            dict[_client.Id] = Notes;
            SaveNotesFile(dict);
        }

        private bool CanSaveNote()
        {
            return !string.IsNullOrWhiteSpace(NewNoteText);
        }

        // Læser notes-filen og returnerer et dictionary: clientId → notetekst
        // Hvis filen ikke eksisterer endnu, returneres et tomt dictionary
        private Dictionary<int, string> ReadNotesFile()
        {
            if (!File.Exists(NotesFilePath))
                return new Dictionary<int, string>();

            string json = File.ReadAllText(NotesFilePath);
            return JsonSerializer.Deserialize<Dictionary<int, string>>(json)
                   ?? new Dictionary<int, string>();
        }

        // Gemmer dictionary tilbage til JSON-filen
        private void SaveNotesFile(Dictionary<int, string> dict)
        {
            string json = JsonSerializer.Serialize(dict,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(NotesFilePath, json);
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
