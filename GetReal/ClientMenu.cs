using System;
using System.Collections.Generic;
using System.Text;
using GetReal;

namespace GetReal
{
    public class ClientMenu
    {
        private readonly ClientService _clientService;
        private readonly TherapistService _therapistService;
        private readonly AppointmentService _appointmentService;
        private ExerciseTemplateService _exerciseService;
        private readonly EntitySelector _entitySelector;

        public ClientMenu(ClientService clientService, AppointmentService appointmentService, ExerciseTemplateService exerciseService, TherapistService therapistService)
        {
            _clientService = clientService;
            _appointmentService = appointmentService;
            _exerciseService = exerciseService;
            _therapistService = therapistService;
            _entitySelector = new EntitySelector(clientService, appointmentService, therapistService);
        }

        public void Show() 
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== Klientmenu ===");
                Console.WriteLine("1. Opret klient");
                Console.WriteLine("2. Se klient / rediger behandlingsforløb");
                Console.WriteLine("3. Opret nyt behandlingsforløb");
                Console.WriteLine("4. Rediger klientdata");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg et punkt: ");

                string? userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("=== Opret klient ===\n");
                        CreateClient();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("=== Søg klient ===\n");
                        Client? selectedClient = _entitySelector.SelectClient();
                        
                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }
                        
                        Console.Clear();
                        Console.WriteLine(selectedClient.Print());
                        TreatmentCourse selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];
                        Console.Clear();
                        Console.WriteLine(selectedClient.Print());
                        Console.WriteLine("\nViser behandlingsforløb:");
                        Console.WriteLine($"\nProblemstilling: {selectedTreatment.Issue}");
                        Console.WriteLine($"\nUdvikling: {selectedTreatment.Development}");
                        Console.WriteLine("\nAftaler:");
                        if (selectedTreatment.Appointments.Count() > 0)
                        {
                            foreach (var appt in selectedTreatment.Appointments)
                            {
                                Therapist therapist = _therapistService.GetTherapistById(appt.TherapistId);
                                Console.WriteLine($"Dato: {appt.Date}, Starttid: {appt.StartTime}, Sluttid: {appt.EndTime}, Behandler: {therapist.FirstName} {therapist.LastName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Klienten har ingen aftaler");
                        }
                        Console.WriteLine("\nTildelte øvelser:");
                        if (selectedTreatment.AssignedExercises.Count() > 0)
                        {
                            foreach (var exc in selectedTreatment.AssignedExercises)
                            {
                                ExerciseTemplate template = _exerciseService.GetExerciseTemplateById(exc.ExerciseTemplateId);
                                Console.WriteLine($"Navn: {template.Name}, Beskrivelse: {template.Description}, Kropsdel: {template.BodyPart}, Sæt: {exc.Sets}, Repetitioner: {exc.Repetitions}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Klienten er ikke tildelt nogle øvelser");
                        }
                        Console.WriteLine("\nTryk r hvis du vil redigere problemstilling og/eller udvikling.\nEllers tryk på mellemrumstasten for at returnere til klientmenuen");
                        while (true)
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.R)
                            {
                                selectedTreatment.Issue = UIHelper.AskStringEdit("Problemstilling:", selectedTreatment.Issue);
                                selectedTreatment.Development = UIHelper.AskStringEdit("Udvikling:", selectedTreatment.Development);
                                break;
                            }
                            if (key.Key == ConsoleKey.Spacebar) { break; }
                        }
                        
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("=== Søg klient ===\n");
                        selectedClient = _entitySelector.SelectClient();
                        Console.WriteLine(selectedClient.Print());
                        _clientService.CreateTreatmentCourse(selectedClient);
                        Console.WriteLine("\nNyt behandlingsforløb oprettet. Gå til Se klient/rediger behandlingsforløb for at redigere");
                        UIHelper.Wait();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("=== Rediger klient ===\n");
                        selectedClient = _entitySelector.SelectClient();
                        if (selectedClient == null)
                        {
                            Console.WriteLine("\nIngen klienter fundet");
                            UIHelper.Wait();
                            break;
                        }
                        Console.WriteLine(selectedClient.Print());
                        Console.WriteLine("\nIndtast nye klientdata");
                        selectedClient.FirstName = UIHelper.AskStringEdit("Fornavn?", selectedClient.FirstName);
                        selectedClient.LastName = UIHelper.AskStringEdit("Efternavn?", selectedClient.LastName);
                        selectedClient.Phone = UIHelper.AskStringEdit("Telefonnummer?", selectedClient.Phone);
                        selectedClient.Email = UIHelper.AskStringEdit("E-mail?", selectedClient.Email);
                        selectedClient.Birthday = UIHelper.AskDateEdit("Fødselsdag (ddMMååå)", selectedClient.Birthday);

                        _clientService.UpdateClient(selectedClient);
                        UIHelper.Wait();
                        break;

                    case "0":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nUgyldig indtastning");
                        UIHelper.Wait();
                        break;
                }

            }
        }
        public void CreateClient() 
        {
            string firstName = UIHelper.AskString("Fornavn?");
            string lastName = UIHelper.AskString("Efternavn?");
            string phoneNumber = UIHelper.AskString("Telefonnummer?");
            string email = UIHelper.AskString("E-mail?");
            DateOnly birthday = UIHelper.AskDate("Fødselsdag? (DD-MM-YYYY)");
            _clientService.CreateClient(firstName, lastName, phoneNumber, email, birthday);
        }
    }
}
