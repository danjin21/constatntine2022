using DummyClient.Session;
using ServerCore;
using System;
using System.Net;
using System.Threading;

namespace DummyClient
{


    class Program
    {
        static int DummyClientCount { get; } = 500;

        static void Main(string[] args)
        {
            // 3초 
            Thread.Sleep(3000);

            Console.WriteLine("Hello World!");

            //// DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[1];

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //IPAddress ipAddr = IPAddress.Parse(info.IpAddress);
            //IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("120.50.85.61"), 7777);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("182.195.210.197"), 7777);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.46"), 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                Program.DummyClientCount);

            while(true)
            {
                Thread.Sleep(10000);
            }

        }
    }
}
