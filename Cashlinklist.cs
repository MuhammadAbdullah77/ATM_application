using ATM_Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ATM_Application
{
    public class CashNode
    {
        public string Data { get; set; }
        public CashNode Next { get; set; }

        public CashNode(string data)
        {
            Data = data;
            Next = null;
        }
    }

    public class CashLinkedList
    {
        private CashNode _first;
        private CashNode _last;

        public CashLinkedList()
        {
            _first = null;
            _last = null;
        }

        public void Add(string data)
        {
            CashNode newNode = new CashNode(data);
            if (_first == null)
            {
                _first = newNode;
                _last = newNode;
            }
            else
            {
                newNode.Next = _first;
                _first = newNode;
            }
        }

        public void RemoveByUsernameAndType(string username, string type)
        {
            CashNode current = _first;
            CashNode previous = null;

            while (current != null)
            {
                string[] data = current.Data.Split(':');
                if (data.Length == 3 && data[0] == username && data[1] == type)
                {
                    if (previous != null)
                    {
                        previous.Next = current.Next;
                        if (current.Next == null)
                        {
                            _last = previous;
                        }
                    }
                    else
                    {
                        _first = current.Next;
                        if (current.Next == null)
                        {
                            _last = null;
                        }
                    }
                    break;
                }
                previous = current;
                current = current.Next;
            }
        }

        public bool UpdateLatestBalance(string username, string newData)
        {
            CashNode current = _first;
            CashNode previous = null;
            while (current != null)
            {
                string[] data = current.Data.Split(':');
                if (data.Length == 3 && data[0] == username && data[1] == "balance")
                {
                    
                    current.Data = newData;
                    if (previous != null)
                    {
                       
                        previous.Next = current.Next;
                        current.Next = _first;
                        _first = current;
                    }
                    return true;
                }
                previous = current;
                current = current.Next;
            }

           
            Add(newData);
            return false;
        }

        public async Task WriteToFile(string filepath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filepath))
                {
                    CashNode current = _first;
                    while (current != null)
                    {
                        await writer.WriteLineAsync(current.Data);
                        current = current.Next;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }
    }
}
