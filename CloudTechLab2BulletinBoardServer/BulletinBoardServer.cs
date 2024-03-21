using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudTechLab2BulletinBoardServer
{
    internal class BulletinBoardServer
    {
        static TcpListener listener;

        const int LIMIT = 5;

        const string FILE_PATH = "messages.txt";

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            listener.Start();
            Console.WriteLine("Server started.");

            if (!File.Exists(FILE_PATH))
            {
                using (FileStream fs = File.Create(FILE_PATH)) { }
            }

            for (int i = 0; i < LIMIT; i++)
            {
                Thread t = new Thread(new ThreadStart(Service));
                t.Name = i.ToString();
                t.Start();
            }
        }

        public static void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();
                Console.WriteLine($"Socket accepted at {Thread.CurrentThread.Name}.");

                try
                {
                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true;

                    while (true)
                    {
                        string command = sr.ReadLine();
                        if (command == "" || command == null)
                        {
                            sw.WriteLine("break");
                            break;
                        }

                        if (command == "LIST")
                        {
                            using (StreamReader fsr = new StreamReader(FILE_PATH)) 
                            {
                                List<string> messages = new List<string>();

                                while (!fsr.EndOfStream)
                                {
                                    messages.Add(fsr.ReadLine());
                                }

                                string json = JsonConvert.SerializeObject(messages);
                                sw.WriteLine(json);
                                Console.WriteLine($"List of messages requested at thread {Thread.CurrentThread.Name}.");
                            }
                        }
                        else
                        {
                            using (StreamWriter fsw = new StreamWriter(FILE_PATH, true))
                            {
                                fsw.WriteLine(command);
                                sw.WriteLine("Message added: \"{0}\".", command);
                                Console.WriteLine($"Message accepted: {command} by thread {Thread.CurrentThread.Name}.");
                            }
                        }
                    }

                    s.Close();
                }
                catch
                {
                    Console.WriteLine($"Exception caught at thread {Thread.CurrentThread.Name}.");
                }
                finally
                {
                    soc.Close();
                    Console.WriteLine($"Socket closed at {Thread.CurrentThread.Name}.");
                }
            }
        }
    }
}
