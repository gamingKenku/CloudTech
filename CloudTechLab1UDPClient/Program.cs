using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
class EmployeeUDPClient
{
    public static void Main(string[] args)
    {
        string ipadress = "127.0.0.1";
        Int32 port = 13000;
        if (args.Length > 0)
            ipadress = args[0];
        UdpClient udpc = new UdpClient(ipadress, port);
        IPEndPoint ep = null;
        while (true)
        {
            Console.Write("Name: ");
            string name = Console.ReadLine();
            if (name == "") break;
            byte[] sdata = Encoding.ASCII.GetBytes(name);
            udpc.Send(sdata, sdata.Length);
            byte[] rdata = udpc.Receive(ref ep);
            string job = Encoding.ASCII.GetString(rdata);
            Console.WriteLine(job);
        }
    }
}