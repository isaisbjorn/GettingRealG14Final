using GetReal;
using System.Windows;
using wpfGettingRealG14This.ViewModels;
using wpfGettingRealG14This.Views;

namespace wpfGettingRealG14This
{
    // App.xaml.cs er applikationens "dirigent".
    // Den starter det første vindue og kobler alle de andre vinduer sammen.
    // ViewModels ved ikke noget om hinanden — det er her de forbindes.
    public partial class App : Application
    {
        // OnStartup kører én gang når programmet starter
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartLogin();
        }

        // Opretter og viser en ny login-skærm.
        // Kalde ved opstart og når brugeren logger ud.
        private void StartLogin()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            LoginView loginView = new LoginView(loginViewModel);

            // Tilmeld os LoginSucceeded-eventet.
            // Koden herinde kører kun når brugeren logger ind korrekt.
            loginViewModel.LoginSucceeded += () =>
            {
                // =====================
                // VELKOMSTSKÆRM
                // Vi opretter et repository her og sender det med,
                // så WelcomeViewModel kan vise dagens aftaler
                // =====================
                IRepository<Client> welcomeRepository =
                    new RepositoryJson<Client>("clients.json");

                WelcomeViewModel welcomeViewModel =
                    new WelcomeViewModel(loginViewModel.Username, welcomeRepository);

                WelcomeView welcomeView = new WelcomeView(welcomeViewModel);

                // =====================
                // NAVIGATION FRA VELKOMST
                // =====================

                // Klienter-knappen åbner ClientView
                welcomeViewModel.NavigateToClients += () =>
                {
                    IRepository<Client> clientRepository =
                        new RepositoryJson<Client>("clients.json");
                    ClientService clientService = new ClientService(clientRepository);
                    ClientViewModel clientViewModel =
                        new ClientViewModel(clientService, clientRepository);

                    // Detaljeknap i ClientView åbner ClientDetailView med den valgte klient
                    clientViewModel.NavigateToDetail += (client) =>
                    {
                        ClientDetailViewModel detailViewModel =
                            new ClientDetailViewModel(client);
                        ClientDetailView detailView = new ClientDetailView(detailViewModel);
                        detailView.Show();
                    };

                    ClientView clientView = new ClientView(clientViewModel);
                    clientView.Show();
                };

                // Aftaler-knappen åbner AppointmentView
                welcomeViewModel.NavigateToAppointments += () =>
                {
                    IRepository<Client> appointmentRepository =
                        new RepositoryJson<Client>("clients.json");
                    AppointmentService appointmentService =
                        new AppointmentService(appointmentRepository);
                    AppointmentViewModel appointmentViewModel =
                        new AppointmentViewModel(appointmentService, appointmentRepository);
                    AppointmentView appointmentView = new AppointmentView(appointmentViewModel);
                    appointmentView.Show();
                };

                // Kalender-knappen åbner CalendarView
                welcomeViewModel.NavigateToCalendar += () =>
                {
                    IRepository<Client> calendarRepository =
                        new RepositoryJson<Client>("clients.json");
                    CalendarViewModel calendarViewModel =
                        new CalendarViewModel(calendarRepository);
                    CalendarView calendarView = new CalendarView(calendarViewModel);
                    calendarView.Show();
                };

                // Øvelser er ikke implementeret endnu
                welcomeViewModel.NavigateToExercises += () =>
                {
                    MessageBox.Show("Øvelser er ikke implementeret endnu.");
                };

                // Log ud — luk velkomstvinduet og vis en ny login-skærm
                welcomeViewModel.NavigateToLogOut += () =>
                {
                    welcomeView.Close();
                    StartLogin();
                };

                welcomeView.Show();
                loginView.Close();
            };

            loginView.Show();
        }
    }
}
