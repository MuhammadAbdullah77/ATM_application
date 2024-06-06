using ATM_Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main()
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        var logins = new Logins();
        var atm = new ATMFunction();
        bool flag = true;
        int choice;

        await logins.Menu();
        Console.Clear();

        if (!logins.IsAuthenticated)
        {
            Console.WriteLine("Authentication failed. Exiting...");
            return;
        }

        while (flag)
        {
            Console.WriteLine("\n\t-----------------------------------Welcome to my ATM_Application--------------------------------");
            Console.WriteLine("\n\t------------------------------------------------------------------------------------------------");
            Console.WriteLine("\n\t--                                Press 1 to perform cash functions                           --");
            Console.WriteLine("\n\t--                                Press 2 to go back to login page                            --");
            Console.WriteLine("\n\t--                                Press 3 to exit                                             --");
            Console.WriteLine("\n\t------------------------------------------------------------------------------------------------");
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("\t-----------------------------------");
                Console.WriteLine("\tInvalid choice, please try again.");
                Console.WriteLine("\t-----------------------------------");
                continue;
            }
            switch (choice)
            {
                case 1:
                    await atm.Menu(logins.Username);
                    Console.Clear();
                    break;
                case 2:
                    await logins.Menu();
                    Console.Clear();
                    break;
                case 3:
                    flag = false;
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    break;
            }
        }
    }
}
