using ATM_Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ATM_Application
{
    internal class ATMFunction
    {
        private CashLinkedList _link = new CashLinkedList();
        private Logins _logins = new Logins();
        private string loginFilePath = "D:\\.Networking_programing\\C#\\Atm\\ATM_Application\\ATM_Application\\login.txt";
        private string cashDepositFilePath = "D:\\.Networking_programing\\C#\\Atm\\ATM_Application\\ATM_Application\\cashdeposit.txt";
        private string cashWithdrawFilePath = "D:\\.Networking_programing\\C#\\Atm\\ATM_Application\\ATM_Application\\cashwithdraw.txt";
        private string cashTransferFilePath = "D:\\.Networking_programing\\C#\\Atm\\ATM_Application\\ATM_Application\\cashtransfer.txt";

        public async Task CashDeposit(string username)
        {
            Console.WriteLine("Enter deposit cash amount:");
            if (decimal.TryParse(Console.ReadLine(), out decimal cash) && cash > 0)
            {
                decimal currentBalance = await GetCurrentBalance(cashDepositFilePath, username);
                decimal newBalance = currentBalance + cash;
                string newData = $"{username}:balance:{newBalance}";
                _link.UpdateLatestBalance(username, newData);
                await _link.WriteToFile(cashDepositFilePath);

                Console.WriteLine($"Successfully deposited: {cash}");
                Console.WriteLine($"Your new balance is: {newBalance}");

                string notification = $"{DateTime.Now}: {username} deposited {cash}. New balance is {newBalance}.";
                await WriteTransactionToFile(cashTransferFilePath, notification);
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
            }
        }

        public async Task CashWithdraw(string username)
        {
            decimal currentBalance = await GetCurrentBalance(cashDepositFilePath, username);
            Console.WriteLine($"Your current balance is: {currentBalance}");
            Console.WriteLine("Enter withdraw cash amount:");
            if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount) && withdrawAmount > 0)
            {
                if (withdrawAmount <= currentBalance)
                {
                    decimal newBalance = currentBalance - withdrawAmount;
                    string newData = $"{username}:balance:{newBalance}";
                    _link.RemoveByUsernameAndType(username, "balance");
                    _link.Add(newData);
                    string withdrawData = $"{username}:withdraw:{withdrawAmount}";
                    await WriteTransactionToFile(cashWithdrawFilePath, withdrawData);
                    await _link.WriteToFile(cashDepositFilePath);

                    Console.WriteLine($"Successfully withdrew: {withdrawAmount}");
                    Console.WriteLine($"Your new balance is: {newBalance}");

                    string notification = $"{DateTime.Now}: {username} withdrew {withdrawAmount}. New balance is {newBalance}.";
                    await WriteTransactionToFile(cashTransferFilePath, notification);
                }
                else
                {
                    Console.WriteLine("Insufficient balance.");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
            }
        }

        private async Task WriteTransactionToFile(string filePath, string transactionData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    await writer.WriteLineAsync(transactionData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }

        private async Task<decimal> GetCurrentBalance(string filePath, string username)
        {
            decimal balance = 0;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(':');
                        if (data.Length == 3 && data[0].Trim() == username && data[1].Trim() == "balance")
                        {
                            balance = decimal.Parse(data[2].Trim());
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the balance: {ex.Message}");
            }
            return balance;
        }

        public async Task CashInquiry(string username)
        {
            decimal balance = await GetCurrentBalance(cashDepositFilePath, username);
            Console.WriteLine($"{username} your current balance is : {balance}");
        }

        public async Task UserInfo(string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader(loginFilePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length >= 6 && data[0].Trim() == username)
                        {
                            Console.WriteLine($"Username : {data[0].Trim()} \nPin Code : {data[1].Trim()} \nYour Name : {data[2].Trim()} \nFather Name: {data[3].Trim()} \nCNIC Number: {data[4].Trim()} " +
                                $"\nDate-Of-Birth: {data[5].Trim()}");
                            return;
                        }
                    }
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> AuthenticateUser(string filepath, string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length >= 2 && data[0].Trim() == username)
                        {
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

        public async Task TransferCash(string senderUsername)
        {
            try
            {
                Console.WriteLine("Enter receiver's username:");
                string receiverUsername = Console.ReadLine();

                if (!await AuthenticateUser(loginFilePath, senderUsername))
                {
                    Console.WriteLine("Sender not found.");
                    return;
                }

                if (!await AuthenticateUser(loginFilePath, receiverUsername))
                {
                    Console.WriteLine("Receiver not found.");
                    return;
                }

                Console.WriteLine("Enter transfer amount:");
                if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount) && transferAmount > 0)
                {
                    decimal senderBalance = await GetCurrentBalance(cashDepositFilePath, senderUsername);
                    if (senderBalance >= transferAmount)
                    {

                        decimal senderNewBalance = senderBalance - transferAmount;
                        string senderNewData = $"{senderUsername}:balance:{senderNewBalance}";
                        _link.UpdateLatestBalance(senderUsername, senderNewData);

                        decimal receiverBalance = await GetCurrentBalance(cashDepositFilePath, receiverUsername);
                        decimal receiverNewBalance = receiverBalance + transferAmount;
                        string receiverNewData = $"{receiverUsername}:balance:{receiverNewBalance}";
                        _link.UpdateLatestBalance(receiverUsername, receiverNewData);

                        string transactionData = $"{senderUsername} transferred {transferAmount} to {receiverUsername}";
                        await WriteTransactionToFile(cashTransferFilePath, transactionData);

                        await _link.WriteToFile(cashDepositFilePath);

                        Console.WriteLine($"Successfully transferred {transferAmount} from {senderUsername} to {receiverUsername}.");
                        Console.WriteLine($"{senderUsername} new balance is: {senderNewBalance}");
                        Console.WriteLine($"{receiverUsername} new balance is: {receiverNewBalance}");

                        string senderNotification = $"{DateTime.Now}: {senderUsername} transferred {transferAmount} to {receiverUsername}.";
                        string receiverNotification = $"{DateTime.Now}: {receiverUsername} received {transferAmount} from {senderUsername}.";
                        await WriteTransactionToFile(cashTransferFilePath, senderNotification);
                        await WriteTransactionToFile(cashTransferFilePath, receiverNotification);
                    }
                    else
                    {
                        Console.WriteLine("Insufficient balance.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid amount. Please enter a positive number.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the transfer: {ex.Message}");
            }
        }

        public async Task Notification(string username)
        {
            try
            {
                using (StreamReader reader = new StreamReader(cashTransferFilePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (line.Contains(username))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading notifications: {ex.Message}");
            }
        }

        public async Task Menu(string username)
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("\n\t-----------------------------------Welcome to my ATM_Application--------------------------------");
                Console.WriteLine("\n\t------------------------------------------------------------------------------------------------");
                Console.WriteLine("\n\t--                                Press 1 to deposit cash                                     --");
                Console.WriteLine("\n\t--                                Press 2 to withdraw cash                                    --");
                Console.WriteLine("\n\t--                                Press 3 to transfer cash                                    --");
                Console.WriteLine("\n\t--                                Press 4 to cash inquiry                                     --");
                Console.WriteLine("\n\t--                                Press 5 to check notifications                              --");
                Console.WriteLine("\n\t--                                Press 6 to check your information                           --");
                Console.WriteLine("\n\t--                                Press 7 to exit                                             --");
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
                        await CashDeposit(username);
                        break;
                    case 2:
                        await CashWithdraw(username);
                        break;
                    case 3:
                        await TransferCash(username);
                        break;
                    case 4:
                        await CashInquiry(username);
                        break;
                    case 5:
                        await Notification(username);
                        break;
                    case 6:
                        await UserInfo(username);
                        break;
                    case 7:
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
