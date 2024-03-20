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

namespace CloudTechLab2WordSorterServer
{
    internal class WordSorterServer
    {
        static TcpListener listener;

        const int LIMIT = 5;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            listener.Start();

            for (int i = 0; i < LIMIT; i++)
            {
                Thread t = new Thread(new ThreadStart(Service));
                t.Start();
            }
        }

        public static List<string> SortWords(string text)
        {
            string[] words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> sortedWords = words.OrderBy(word => word, StringComparer.OrdinalIgnoreCase).ToList();

            return sortedWords;
        }

        public static void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();

                try
                {
                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true;

                    while (true)
                    {
                        string text = sr.ReadLine();
                        if (text == "" || text == null) break;
                        List<string> words = new List<string>();

                        string json = JsonConvert.SerializeObject(SortWords(text));

                        sw.WriteLine(json);
                    }

                    s.Close();
                }
                catch
                {
                    soc.Close();
                }
            }
        }
    }
}
