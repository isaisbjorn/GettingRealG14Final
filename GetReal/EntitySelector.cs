using GetReal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class EntitySelector
    {
        
        private readonly ClientService _clientService;
        private readonly AppointmentService _appointmentService;
        private readonly ExerciseTemplateService _exerciseService;
        private readonly TherapistService _therapistService;
        public EntitySelector(ClientService clientService, AppointmentService appointmentService, TherapistService therapistService)
        {
            _clientService = clientService;
            _appointmentService = appointmentService;
            _therapistService = therapistService;
        }

        public Client? SelectClient()
        {
        string searchString = UIHelper.AskString("Indtast hele eller en del af klientens fornavn, efternavn, telefonnummer eller e-mail?");
        IEnumerable<Client> clientSearch = new List<Client>();
        clientSearch = _clientService.SearchClient(searchString);

            if (clientSearch.Count() == 0)
            {
                return null;
            }
            else
            {
                Console.WriteLine("Følgende klienter blev fundet:");
                int counter = 0;
                foreach (Client client in clientSearch)
                {
                    counter++;
                    Console.WriteLine($"{counter}) {client.FirstName} {client.LastName}");
                }
                int selection = UIHelper.AskIntInRange($"Vælg klient (1-{clientSearch.Count()})", 1, clientSearch.Count());
                Client selectedClient = clientSearch.ElementAt(selection - 1);
                return selectedClient;
            }
        }
        public Therapist? SelectTherapist()
        {
            string searchString = UIHelper.AskString("Indtast hele eller en del af behandlerens fornavn eller efternavn");
            IEnumerable<Therapist> therapistSearch = new List<Therapist>();
            therapistSearch = _therapistService.SearchTherapist(searchString);

            if (therapistSearch.Count() == 0)
            {
                return null;
            }
            else
            {
                Console.WriteLine("Følgende behandlere blev fundet:");
                int counter = 0;
                foreach (Therapist therapist in therapistSearch)
                {
                    counter++;
                    Console.WriteLine($"{counter}) {therapist.FirstName} {therapist.LastName}");
                }
                int selection = UIHelper.AskIntInRange($"Vælg behandler (1-{therapistSearch.Count()})", 1, therapistSearch.Count());
                Therapist selectedTherapist = therapistSearch.ElementAt(selection - 1);
                return selectedTherapist;
            }
        }
        public TreatmentCourse SelectTreatment(Client client)
        {
            Console.WriteLine($"\nKlienten har følgende behandlingsforløb:\n");
            int counter = 0;
            foreach (TreatmentCourse treatment in client.TreatmentCourses)
            {
                counter++;
                Console.WriteLine($"{counter}) oprettet {treatment.Created}");
            }
            int selection = UIHelper.AskIntInRange($"Vælg behandlingsforløb (1-{client.TreatmentCourses.Count()})", 1, client.TreatmentCourses.Count());
            TreatmentCourse selectedTreatment = client.TreatmentCourses.ElementAt(selection - 1);
            return selectedTreatment;
        }
    }
}
