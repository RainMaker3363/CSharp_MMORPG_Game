using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.Monster;

        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        // 내가 어느 게임 방에 속해있는지..
        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo() { PosInfo = new PositionInfo() };

        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get
            {
                return Stat.Speed;
            }
            set
            {
                Stat.Speed = value;
            }
        }

        public int HP
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); }
        }

        public MoveDir Dir
        {
            get { return PosInfo.Movedir; }
            set { PosInfo.Movedir = value; }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }
       

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public virtual void Update()
        {

        }

        public Vector2int CellPos
        {
            get
            {
                return new Vector2int(PosInfo.PosX, PosInfo.PosY);
            }

            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        public Vector2int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.Movedir);
        }

        public Vector2int GetFrontCellPos(MoveDir dir)
        {
            Vector2int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2int.up;
                    break;

                case MoveDir.Down:
                    cellPos += Vector2int.down;
                    break;

                case MoveDir.Left:
                    cellPos += Vector2int.left;
                    break;

                case MoveDir.Right:
                    cellPos += Vector2int.right;
                    break;
            }

            return cellPos;
        }

        public static MoveDir GetDirFromVec(Vector2int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);

            // Todo

            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.Objectid = Id;
            changePacket.Hp = Stat.Hp;

            Room.BroadCast(changePacket);

            if(Stat.Hp <= 0)
            {
                Stat.Hp = 0;
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            S_Die diePacket = new S_Die();
            diePacket.Objectid = Id;
            diePacket.Attackerid = attacker.Id;

            Room.BroadCast(diePacket);

            GameRoom room = Room;
            room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.Movedir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;

            room.EnterGame(this);
        }
    }
}
