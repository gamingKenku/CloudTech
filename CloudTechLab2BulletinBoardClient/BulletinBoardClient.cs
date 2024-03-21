using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudTechLab2BulletinBoardClient
{
    internal class BulletinBoardClient
    {
        public static void Main(string[] args)
        {
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
                    Console.WriteLine("Enter your message or type \"LIST\" to view all messages.");
                    string command = Console.ReadLine();
                    sw.WriteLine(command);

                    if (command == "") break;

                    if (command == "LIST")
                    {
                        List<string> messages = JsonConvert.DeserializeObject<List<string>>(sr.ReadLine());
                        foreach (string message in messages)
                        {
                            Console.WriteLine("{0};", message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(sr.ReadLine());
                    }
                }

                Console.WriteLine("Have a good day.");

                s.Close();
            }
            catch
            {
                Console.WriteLine("Exception caught at client side.");
            }
            finally
            {
                client.Close();
            }

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
