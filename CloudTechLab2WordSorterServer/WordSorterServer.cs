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
using System.Text.RegularExpressions;

namespace CloudTechLab2WordSorterServer
{
    static public class WordSorter
    {
        public static List<string> Sort(string text)
        {
            Regex pattern = new Regex("[;,\t\r\n .:]|[\n]{2}");
            string input = pattern.Replace(text, " ");

            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> sortedWords = words.OrderBy(word => word, StringComparer.OrdinalIgnoreCase).Distinct().ToList();

            return sortedWords;
        }
    }

    internal class WordSorterServer
    {
        static TcpListener listener;

        const int LIMIT = 5;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            listener.Start();
            Console.WriteLine("Server started.");

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
                        string text = sr.ReadLine();
                        
                        if (text == "" || text == null)
                        {
                            sw.WriteLine("break");
                            break;
                        }

                        Console.WriteLine($"Text accepted at thread {Thread.CurrentThread.Name}.");

                        List<string> words = new List<string>();

                        string json = JsonConvert.SerializeObject(WordSorter.Sort(text));

                        sw.WriteLine(json);

                        Console.WriteLine($"Text sent at thread {Thread.CurrentThread.Name}.");
                    }

                    s.Close();
                }
                catch
                {
                    Console.WriteLine($"Exception caught at thread {Thread.CurrentThread.Name}.");
                }
                finally
                {
                    Console.WriteLine($"Socket closed at thread {Thread.CurrentThread.Name}.");
                    soc.Close();
                }
            }
        }
    }
}
