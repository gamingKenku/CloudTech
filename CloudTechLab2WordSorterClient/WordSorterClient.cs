using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudTechLab2WordSorterClient
{
    class WordSorterClient
    {
        public static void Main(string[] args)
        {
            bool break_flag = false;

            Int32 port = 13000;
            string serverAddr = "127.0.0.1";

            TcpClient client = new TcpClient(serverAddr, port);

            try
            {
                Stream s = client.GetStream();
                StreamReader sr = new StreamReader(s);
                StreamWriter sw = new StreamWriter(s);
                sw.AutoFlush = true;

                while (true)
                {
                    Console.WriteLine("Enter the text and press \"Enter\" key to send. Enter empty line to end execution.");
                    string text = Console.ReadLine();
                    if (text == "" || text == null) break_flag = true;
                    sw.WriteLine(text);

                    Console.WriteLine("Entered words:");

                    string json = sr.ReadLine();
                    List<string> words = JsonConvert.DeserializeObject<List<string>>(json);

                    foreach (string word in words)
                    {
                        Console.WriteLine($"{word}");
                    }

                    if (break_flag) break;
                }

                Console.WriteLine("Have a good day.");
                s.Close();
            }
            finally
            {
                client.Close();
            }
        }
    }
}
