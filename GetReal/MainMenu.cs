
using GetReal;
using SofusGettingReal;
using System;

namespace GetReal
{
	public class MainMenu
	{
		private AppointmentService _appointmentService;
		private ClientService _clientService;
		private ExerciseTemplateService _exerciseTemplateService;
        private TherapistService _therapistService;

        public MainMenu(AppointmentService appointmentService, ClientService clientService, ExerciseTemplateService exerciseService, TherapistService therapistService)
		{
			_appointmentService = appointmentService;
			_clientService = clientService;
			_exerciseTemplateService = exerciseService;
            _therapistService = therapistService;
        }

		public void Show()
		{
			bool running = true;
			while (running)
			{
				Console.Clear();
				Console.WriteLine("=== Hovedmenu ===");
				Console.WriteLine("1. Behandlere");
				Console.WriteLine("2. Aftaler");
				Console.WriteLine("3. Klienter");
				Console.WriteLine("4. Øvelser");
				Console.WriteLine("0. Afslut");
				Console.Write("\nVælg et punkt: ");

				string input = Console.ReadLine();

				switch (input)
				{
					case "1":
                        TherapistMenu therapistMenu = new TherapistMenu(_therapistService);
                        therapistMenu.Show();
                        break;
					case "2":
						AppointmentMenu appointmentMenu = new AppointmentMenu(_clientService, _appointmentService, _therapistService);
						appointmentMenu.Show();
						break;
					case "3":
						ClientMenu clientMenu = new ClientMenu(_clientService, _appointmentService, _exerciseTemplateService, _therapistService);
						clientMenu.Show();
						break;
					case "4":
						ExerciseMenu exerciseMenu = new ExerciseMenu(_exerciseTemplateService, _clientService, _appointmentService, _therapistService);
						exerciseMenu.ShowMenu();
                        break;
					case "0":
                        running = false;
                        /* LogInMenu logInMenu = new LogInMenu();
                        logInMenu.StartLogin(); */
                        break;
					default:
						Console.WriteLine("Ugyldigt valg, prøv igen.");
						Console.ReadKey();
						break;
				}
			}
		}
	}
}