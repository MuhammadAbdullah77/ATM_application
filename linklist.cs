using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ATM_Applicaton
{
    
    public class Node
    {
        public string Data { get; set; }
        public Node Next { get; set; }

        public Node(string data)
        {
            Data = data;
            Next = null;
        }
    }

    public class LinkedList
    {
        public Node First { get; private set; }
        public Node Last { get; private set; }
        public int Count { get; private set; }

        public LinkedList()
        {
            First = null;
            Last = null;
        }

        public void Add(string data)
        {
            Node newNode = new Node(data);
            if (First == null)
            {
                First = newNode;
                Last = newNode;
            }
            else
            {
                newNode.Next = First;
                First = newNode;
            }
            Count++;
        }

        public async Task WriteToFile(string filepath)
        {
            using (StreamWriter writer = new StreamWriter(filepath , append:true))
            {
                Node current = First;
                while (current != null)
                {
                    await writer.WriteLineAsync(current.Data);
                    current = current.Next;
                }
                Console.WriteLine("Account Created Successfully");
            }
        }
    }

   
}