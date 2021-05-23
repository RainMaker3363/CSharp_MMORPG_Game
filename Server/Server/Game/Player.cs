using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
        // 내가 어느 게임 방에 속해있는지..
        public GameRoom Room { get; set; }

        public ClientSession Session { get; set; }

        public Vector2int CellPos
        {
            get
            {
                return new Vector2int(Info.PosInfo.PosX, Info.PosInfo.PosY);
            }

            set
            {
                Info.PosInfo.PosX = value.x;
                Info.PosInfo.PosY = value.y;
            }
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
