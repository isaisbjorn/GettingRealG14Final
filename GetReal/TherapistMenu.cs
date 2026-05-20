using System;
using System.Collections.Generic;
using System.Text;

namespace GetReal
{
    public class TherapistMenu
    {
        private readonly TherapistService _therapistService;
        
        public TherapistMenu(TherapistService therapistService)
        {
            _therapistService = therapistService;
        }

        public void Show()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== Behandlermenu ===");
                Console.WriteLine("1. Opret behandler");
                Console.WriteLine("2. Se behandlere");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg et punkt: ");

                string? userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        CreateTherapist();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("=== Se behandlere ===\n");
                        List<Therapist> therapists = _therapistService.GetAllTherapists();

                        foreach (Therapist therapist in therapists)
                        {
                            Console.WriteLine(
                                $"{therapist.Id} {therapist.FirstName} {therapist.LastName} {therapist.Title} {therapist.Email} {therapist.UserName}");
                        }
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
        public void CreateTherapist()
        {
            Console.Clear();
            Console.WriteLine("=== Opret Behandler ===\n");
            string firstName = UIHelper.AskString("Fornavn?");
            string lastName = UIHelper.AskString("Efternavn?");
            string title = UIHelper.AskString("Titel?");
            string email = UIHelper.AskString("E-mail?");
            string userName = UIHelper.AskString("Brugernavn?");
            string password = UIHelper.AskString("Password?");
            _therapistService.CreateTherapist(firstName, lastName, title, email, userName, password);
        }
    }
}
