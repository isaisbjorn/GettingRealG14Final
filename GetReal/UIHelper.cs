using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GetReal
{
    public static class UIHelper
    {
        public static void Wait()
        {
            Console.WriteLine("\nTryk en tast for at fortsætte...");
            Console.ReadKey(true);
        }
        public static string AskString(string question)
        {
            while (true)
            {
                Console.WriteLine(question);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input;
                Console.WriteLine("Ugyldigt input, prøv igen.");
            }
        }
        public static int AskInt(string question)
        {
            while (true)
            {
                Console.WriteLine(question);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int result)) return result;
                Console.WriteLine("Ugyldigt input, indtast et heltal.");
            }
        }
        public static int AskIntInRange(string question, int min, int max)
        {
            while (true)
            {
                int input = AskInt(question);
                if (input >= min && input <= max) return input;
                Console.WriteLine($"Indtast et tal mellem {min} og {max}.");
            }
        }
        public static DateOnly AskDate(string question)
        {
            while (true)
            {
                Console.WriteLine(question);
                string? input = Console.ReadLine();
                if (DateOnly.TryParseExact(input, new[] { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "ddMMyyyy", "ddMMyy" }, null, DateTimeStyles.None, out DateOnly date)) return date;
                Console.WriteLine("Ugyldigt dato format, prøv igen. (ddMMåå)");
            }
        }
        public static DateOnly AskDateEdit(string question, DateOnly currentValue)
        {
            while (true)
            {
                Console.WriteLine($"{question} (nuværende: {currentValue:dd-MM-yyyy}, tryk Enter for at beholde)");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return currentValue;
                if (DateOnly.TryParseExact(input, new[] { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "ddMMyyyy", "ddMMyy" }, out DateOnly date)) return date;
                Console.WriteLine("Ugyldigt dato format, prøv igen. (DD-MM-YYYY)");
            }
        }
        public static TimeOnly AskTime(string question)
        {
            while (true)
            {
                Console.WriteLine(question);
                string? input = Console.ReadLine();
                if (TimeOnly.TryParseExact(input, new[] { "HH:mm", "HHmm"}, null, DateTimeStyles.None, out TimeOnly time)) return time;
                Console.WriteLine("Ugyldigt tidsformat, prøv igen. (TTmm)");
            }
        }
        public static (TimeOnly start, TimeOnly end) AskTimeRange(string startQuestion, string endQuestion)
        {
            while (true)
            {
                TimeOnly start = AskTime(startQuestion);
                TimeOnly end = AskTime(endQuestion);
                if (start < end) return (start, end);
                Console.WriteLine("Sluttidspunkt skal være efter starttidspunkt, prøv igen.");
            }
        }
        public static string AskStringEdit(string question, string currentValue)
        {
            Console.WriteLine($"{question} (nuværende: {currentValue}, tryk Enter for at beholde)");
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? currentValue : input;
        }
        public static bool AskYesNo(string question)
        {
            while (true)
            {
                Console.WriteLine($"{question} (j/n)");
                string? input = Console.ReadLine()?.ToLower().Trim();
                if (input == "j") return true;
                if (input == "n") return false;
                Console.WriteLine("Ugyldigt input, skriv j eller n.");
            }
        }
    }
}
