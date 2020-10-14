using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using static Google.Protobuf.Protocol.Person.Types;

namespace Server
{
    
    class ClientSession : PacketSession
    {
        public int SessionId
        { 
          get;
          set;
        }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Person person = new Person()
            {
                Name = "LeinMaker",
                Id = 123,
                Email = "LeinMaker@naver.com",
                Phones = { new PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
            };

            ushort size = (ushort)person.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
            ushort protocolid = 1;
            Array.Copy(BitConverter.GetBytes(protocolid), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(person.ToByteArray(), 0, sendBuffer, 4, size);


            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            //PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDiconnected(EndPoint endpoint)
        {
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endpoint}");
        }



        public override void OnSend(int numofByte)
        {
            //Console.WriteLine($"Transferred bytes : {numofByte}");
        }
    }

}
