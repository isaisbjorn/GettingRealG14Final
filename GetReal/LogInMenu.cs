using System;
using System.Collections.Generic;
using System.Text;

namespace SofusGettingReal
{
    public class LogInMenu
    {
        string workerID = "";
        string workerName = "";
        string password = "";
        bool isValidLogin = false;

        public void StartLogin()
        {

            while (!isValidLogin) // Så længe der ikke kommer et "ValidLogin" kører loopet videre og brugeren kan forsøge igen.
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=========================");
                Console.WriteLine("ManuVision");
                Console.WriteLine("=========================");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.ResetColor();

                Console.WriteLine("Indtast dit medarbejder ID for at begynde: ");
                workerID = Console.ReadLine();

                if (workerID == "AP")
                {
                    workerName = "Andreas Papadakis";

                    Console.WriteLine("Indtast kodeord: ");
                    password = Console.ReadLine();
                    if (password == "Papas")
                    {
                        isValidLogin = true;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("Ugyldigt medarbejder ID!");
                    Console.ResetColor();
                    Console.WriteLine("Tryk på en tast for at prøve igen...");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }
                if (!isValidLogin)
                {
                    Console.WriteLine();
                    Console.WriteLine("Forkert adgangskode eller login");
                    Console.WriteLine("Tryk for at forsøge igen");
                    Console.ReadKey();
                }
            }

            // Til næste step i menuen - en velkomst besked når der er logget ind
            Console.Clear();
            Console.WriteLine("============================");
            Console.WriteLine($"Velkommen {workerName}");
            Console.WriteLine("============================");
            Console.ReadLine();

            // MENU VISES UNDER?


        }
    }
}
