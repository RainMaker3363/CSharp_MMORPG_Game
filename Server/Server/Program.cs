﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using Google.Protobuf.Protocol;
using static Google.Protobuf.Protocol.Person.Types;
using Google.Protobuf;

namespace Server
{

    class Program
    {
        static Listener _listener = new Listener();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            // DNS (Domain name System)
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iphost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


            _listener.init(endPoint, () =>
            {
                return SessionManager.Instance.Generate();
            });
            Console.WriteLine("Listening...");

            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}
