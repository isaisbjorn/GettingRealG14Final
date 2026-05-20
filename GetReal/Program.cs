using System.Net;
using GetReal;
using SofusGettingReal;

namespace GetReal
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IRepository<Client> clientRepository = new RepositoryJson<Client>("clients.json");
			IRepository<Therapist> therapistRepository = new RepositoryJson<Therapist>("therapists.json");
			IRepository<ExerciseTemplate> exerciseTemplateRepository = new RepositoryJson<ExerciseTemplate>("exercisetemplates.json");

			ClientService clientService = new ClientService(clientRepository);
			AppointmentService appointmentService = new AppointmentService(clientRepository);
			ExerciseTemplateService exerciseService = new ExerciseTemplateService(exerciseTemplateRepository);
            TherapistService therapistService = new TherapistService(therapistRepository);

            LogInMenu logInMenu = new LogInMenu();
			logInMenu.StartLogin();

			MainMenu mainMenu = new MainMenu(appointmentService, clientService, exerciseService, therapistService);
			mainMenu.Show();

			clientRepository.Save();
			therapistRepository.Save();
			exerciseTemplateRepository.Save();

            
		}
	}
}