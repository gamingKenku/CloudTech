using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace CloudTechLab1Server
{
    internal class EmployeeTCPServer
    {
        static TcpListener listener;

        const int LIMIT = 5;

        static Dictionary<string, string> employees = new Dictionary<string, string>()
        {
            {"john", "manager"},
            {"jane", "steno"},
            {"jim", "clerk"},
            {"jack", "salesman"}
        };


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

                    sw.WriteLine("{0} Employees available", employees.Count());

                    while (true)
                    {
                        string name = sr.ReadLine();
                        if (name == "" || name == null) break;
                        string job = employees[name];
                        if (job == null) job = "No such employee";
                        sw.WriteLine(job);
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
