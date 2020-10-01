using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    
    class ClientSession : PacketSession
    {
        public int SessionId
        { 
          get;
          set;
        }

        public GameRoom Room
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

            //Packet packet = new Packet() { size = 100, packetid = 10 };
            //// 보낸다
            ////byte[] SendBuff = Encoding.UTF8.GetBytes("Welcome to RaMa MMORPG Server!");

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetid);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            ////Send(sendBuff);
            //Thread.Sleep(5000);

            //// 쫒아낸다
            //Disconnect();

            // JobQueue로 모든것을 관리해준다
            Program.Room.Push(() =>
            {
                Program.Room.Enter(this);
            });
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDiconnected(EndPoint endpoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom room = Room;

                room.Push(() =>
                {
                    room.Leave(this);
                    Room = null;
                });
            }

            Console.WriteLine($"OnDisconnected : {endpoint}");
        }



        public override void OnSend(int numofByte)
        {
            //Console.WriteLine($"Transferred bytes : {numofByte}");
        }
    }

}
