using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        long _nextMoveTick = 0;

        public override void Update()
        {
            if (Data == null || Owner == null || Room == null)
                return;

            if (Data.projectile == null)
                return;

            if (_nextMoveTick >= Environment.TickCount64)
                return;

            long tick = (long)(1000 / Data.projectile.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            Vector2int destPos = GetFrontCellPos();
            if(Room._Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.BroadCast(movePacket);

                Console.WriteLine("Move Arrow");
            }
            else
            {
                GameObject target = Room._Map.Find(destPos);
                if(target != null)
                {
                    // 피격 판정
                    target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
                    //Console.WriteLine($"damage : {Data.damage}");
                }

                // 소멸
                Room.Push(Room.LeaveGame, Id);
            }
        }
    }
}
