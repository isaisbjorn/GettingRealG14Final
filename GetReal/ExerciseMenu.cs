using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class ExerciseMenu
    {
        private ExerciseTemplateService _exerciseService;
        private ClientService _clientService;
        private TherapistService _therapistService;
        private readonly EntitySelector _entitySelector;

        public ExerciseMenu(ExerciseTemplateService exerciseService, ClientService clientService, AppointmentService appointmentService, TherapistService therapistService)
        {
            _exerciseService = exerciseService;
            _clientService = clientService;
            _entitySelector = new EntitySelector(clientService, appointmentService, therapistService);
        }

        public void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("=== Øvelse Menu ===");
                Console.WriteLine("1. Opret øvelse");
                Console.WriteLine("2. Vis øvelser");
                Console.WriteLine("3. Opdater øvelser");
                Console.WriteLine("4. Slet øvelser");
                Console.WriteLine("5. Tildel øvelse til klient");
                Console.WriteLine("0. Tilbage");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateExercise();
                        break;

                    case "2":
                        ViewExercises();
                        break;

                    case "3":
                        UpdateExercise();
                        break;

                    case "4":
                        DeleteExercise();
                        break;

                    case "5":
                        AssignExerciseToClient();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("\nUgyldig indtastning");
                        UIHelper.Wait();
                        break;
                }
            }
        }

        private void CreateExercise()
        {
            Console.Clear();

            Console.WriteLine("=== Opret øvelse ===");

            string name = UIHelper.AskString("Navn: ");
            string description = UIHelper.AskString("Beskrivelse: ");

            Console.WriteLine("Vælg kropsdel:");

            foreach (BodyPart bodyPart in Enum.GetValues(typeof(BodyPart)))
            {
                Console.WriteLine($"{(int)bodyPart} - {bodyPart}");
            }

            int choice = UIHelper.AskIntInRange("", 1, Enum.GetNames(typeof(BodyPart)).Length);
            BodyPart selectedBodyPart = (BodyPart)choice;

            ExerciseTemplate exercise = new ExerciseTemplate
            (
                0,
                name,
                description,
                selectedBodyPart
            );

            _exerciseService.AddExercise(exercise);

            Console.WriteLine("Øvelse oprettet!");
            UIHelper.Wait();
        }

        private void ViewExercises()
        {
            Console.Clear();

            Console.WriteLine("=== Øvelser ===");

            List<ExerciseTemplate> exercises =
                _exerciseService.GetAllExercises();

            foreach (ExerciseTemplate exercise in exercises)
            {
                Console.WriteLine(
                    $"{exercise.Id}: {exercise.Name} - {exercise.BodyPart}");
            }

            UIHelper.Wait();
        }

        private void UpdateExercise()
        {
            Console.Clear();
            Console.WriteLine("=== Opdater øvelse ===\n");

            List<ExerciseTemplate> exercises =
                _exerciseService.GetAllExercises();

            foreach (ExerciseTemplate exercise in exercises)
            {
                Console.WriteLine(
                    $"Id {exercise.Id}: {exercise.Name}");
            }

            int id = UIHelper.AskInt("Indtast Id på øvelse: ");

            ExerciseTemplate exerciseToUpdate =
                exercises.FirstOrDefault(e => e.Id == id);

            if (exerciseToUpdate == null)
            {
                Console.WriteLine("Øvelse ikke fundet.");
                UIHelper.Wait();
                return;
            }

            exerciseToUpdate.Name = UIHelper.AskString("Nyt navn: ");
            exerciseToUpdate.Description = UIHelper.AskString("Ny beskrivelse: ");

            _exerciseService.UpdateExercise(exerciseToUpdate);

            Console.WriteLine("Øvelse opdateret!");
            UIHelper.Wait();
        }

        private void DeleteExercise()
        {
            Console.Clear();
            Console.WriteLine("=== Slet øvelse ===\n");
            List<ExerciseTemplate> exercises =
                _exerciseService.GetAllExercises();

            foreach (ExerciseTemplate exercise in exercises)
            {
                Console.WriteLine(
                    $"Id {exercise.Id}: {exercise.Name}");
            }

            int id = UIHelper.AskInt("Indtast Id på øvelse der skal slettes: ");

            _exerciseService.DeleteExercise(id);

            Console.WriteLine("Øvelse slettet!");
            UIHelper.Wait();
        }
        private void AssignExerciseToClient()
        {
            Console.Clear();

            Console.WriteLine("=== Tildel øvelse til klient ===\n");

            Client? selectedClient = _entitySelector.SelectClient();

            if (selectedClient == null)
            {
                Console.WriteLine("\nIngen klienter fundet");
                UIHelper.Wait();
                return;
            }

            TreatmentCourse selectedTreatment = (selectedClient.TreatmentCourses.Count() > 1) ? _entitySelector.SelectTreatment(selectedClient) : selectedClient.TreatmentCourses[0];

            List<ExerciseTemplate> exercises =
                _exerciseService.GetAllExercises();

            foreach (ExerciseTemplate exercise in exercises)
            {
                Console.WriteLine($"Id: {exercise.Id}, Øvelse navn: {exercise.Name}");
            }

            int exerciseId = UIHelper.AskInt("Vælg øvelse Id: ");

            ExerciseTemplate selectedExercise =
                exercises.FirstOrDefault(e => e.Id == exerciseId);

            if (selectedExercise == null)
            {
                Console.WriteLine("Øvelse ikke fundet.");
                UIHelper.Wait();
                return;
            }
            int reps = UIHelper.AskInt("Antal repetitioner?");
            int sets = UIHelper.AskInt("Antal sæt?");

            if (selectedTreatment.AssignedExercises.Any(x => x.ExerciseTemplateId == selectedExercise.Id))
            {
                Console.WriteLine("Denne øvelse er allerede tildelt klienten");
            }
            else
            {
                AssignedExercise assignedExercise = new AssignedExercise
                {
                    ExerciseTemplateId = selectedExercise.Id,
                    Repetitions = reps,
                    Sets = sets
                };
                selectedTreatment.AssignedExercises.Add(assignedExercise);

                _clientService.UpdateClient(selectedClient);

                Console.WriteLine("Øvelse tildelt!");
            }

            UIHelper.Wait();
        }
    }
}