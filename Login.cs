using ATM_Applicaton;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ATM_Application
{
    public class Logins
    {
        private string username;
        public string pin;
        public string dob;
        public string cnic;
        public string fatherName;
        public string name;
        public string Username => username;
        public bool IsAuthenticated { get; private set; }
        private LinkedList _linkedList = new LinkedList();
        private const string FilePath = "D:\\.Networking_programing\\C#\\Atm\\ATM_Application\\ATM_Application\\login.txt";

        public Logins()
        {
            IsAuthenticated = false;
        }

        public async Task<bool> AuthenticateLogin(string username, string pin)
        {
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length >= 2 && data[0].Trim() == username && data[1].Trim() == pin)
                        {
                            Console.WriteLine("Account login Successfully");
                            this.username = username;
                            IsAuthenticated = true;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while authenticating: {ex.Message}");
            }
            Console.WriteLine("Invalid username or pin");
            return false;
        }

        public async Task<bool> AuthenticateUserName(string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length > 0 && data[0].Trim() == username.Trim())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking the username: {ex.Message}");
            }
            return false;
        }

        public async Task CreateAccount()
        {
            Console.WriteLine("Enter your name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter your Father name: ");
            fatherName = Console.ReadLine();
            Console.WriteLine("Enter your CNIC number: ");
            cnic = Console.ReadLine();
            Console.WriteLine("Enter your Date of Birth: ");
            dob = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Enter your username: ");
                username = Console.ReadLine();
                if (!await AuthenticateUserName(username))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Username already exists. Please choose a different username.");
                }
            }

            
            while (true)
            {
                Console.WriteLine("Enter your 4-digit Pin code: ");
                pin = Console.ReadLine();
                if (pin.Length == 4 && pin.All(char.IsDigit))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid PIN. Please enter a 4-digit numeric PIN.");
                }
            }

            string data = $"{username},{pin},{name},{fatherName},{cnic},{dob}";
            _linkedList.Add(data);
            await _linkedList.WriteToFile(FilePath);
        }

        public async Task Login(string filepath)
        {
            Console.WriteLine("Enter your Username:");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter your Pin:");
            string pinCode = Console.ReadLine();

            await AuthenticateLogin(userName, pinCode);
        }

        public async Task Menu()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("\n\t-----------------------------------Welcome to my ATM_Application--------------------------------");
                Console.WriteLine("\n\t------------------------------------------------------------------------------------------------");
                Console.WriteLine("\n\t--                               Press 1 to create account                                    --");
                Console.WriteLine("\n\t--                               Press 2 to login to your account                             --");
                Console.WriteLine("\n\t--                               Press 3 to exit                                              --");
                Console.WriteLine("\n\t------------------------------------------------------------------------------------------------");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("\t-----------------------------------");
                    Console.WriteLine("\tInvalid choice, please try again.");
                    Console.WriteLine("\t-----------------------------------");
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        await CreateAccount();
                        break;
                    case 2:
                        await Login(FilePath);
                        if (this.IsAuthenticated)
                        {
                            Console.Clear();
                            flag = false;
                        }
                        break;
                    case 3:
                        Console.WriteLine("Exiting Login page");
                        Console.Clear();
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }
    }
}
