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

        public GameObject()
        {
            Info.PosInfo = PosInfo;
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
    }
}
