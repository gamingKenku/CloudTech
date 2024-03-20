using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
class EmployeeUDPServer
{
    static Dictionary<string, string> employees = new Dictionary<string, string>()
    {
        {"john", "manager"},
        {"jane", "steno"},
        {"jim", "clerk"},
        {"jack", "salesman"}
    };
    public static void Main()
    {
        Int32 port = 13000;
        UdpClient udpc = new UdpClient(port);
        Console.WriteLine("Server started, servicing on port " + port.ToString());
        IPEndPoint ep = null;
        while (true)
        {
            byte[] rdata = udpc.Receive(ref ep);
            string name = Encoding.ASCII.GetString(rdata);
            string job = employees[name];
            if (job == null) job = "No such employee";
            byte[] sdata = Encoding.ASCII.GetBytes(job);
            udpc.Send(sdata, sdata.Length, ep);
        }
    }
}