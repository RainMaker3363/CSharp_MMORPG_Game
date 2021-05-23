using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomID { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        Map _map = new Map();

        public void init(int mapid)
        {
            _map.LoadMap(mapid);
        }

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock(_lock)
            {
                _players.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;

                    newPlayer.Session.Send(enterPacket);


                    S_Spawn spawnPacket = new S_Spawn();
                    foreach(Player p in _players.Values)
                    {
                        if (newPlayer != p)
                        {
                            spawnPacket.Players.Add(p.Info);
                        }
                    }

                    newPlayer.Session.Send(spawnPacket);
                }

                // 타인에게 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach(Player p in _players.Values)
                    {
                        if(newPlayer != p)
                        {
                            p.Session.Send(spawnPacket);
                        }
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock(_lock)
            {
                Player player = null;
                if (_players.Remove(playerId, out player) == false)
                    return;

                player.Room = null;

                // 본인한테 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 타인한테 정보 전송
                {
                    S_Despawn despawn = new S_Despawn();
                    despawn.PlayerIds.Add(player.Info.PlayerId);
                    foreach(Player p in _players.Values)
                    {
                        if(player != p)
                        {
                            p.Session.Send(despawn);
                        }
                    }
                }
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;


            lock(_lock)
            {
                // 일단 서버에서 좌표 이동
                PositionInfo MovePosinfo = movePacket.PosInfo;
                PlayerInfo info = player.Info;

                // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
                if(MovePosinfo.PosX != info.PosInfo.PosX || MovePosinfo.PosY != info.PosInfo.PosY)
                {
                    if (_map.CanGo(new Vector2int(MovePosinfo.PosX, MovePosinfo.PosY)) == false)
                        return;
                }

                info.PosInfo.State = MovePosinfo.State;
                info.PosInfo.Movedir = MovePosinfo.Movedir;
                _map.ApplyMove(player, new Vector2int(MovePosinfo.PosX, MovePosinfo.PosY));

                // 다른 플레이어한테도 알려준다.
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.PlayerId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                BroadCast(resMovePacket);
            }
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;


            lock (_lock)
            {
                // 일단 서버에서 좌표 이동
                PlayerInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // 스킬 사용 가능 여부 체크

                // 통과
                info.PosInfo.State = CreatureState.Skill;

                S_Skill skill = new S_Skill() { Info = new SKillInfo() };
                skill.Playerid = info.PlayerId;
                skill.Info.Skillid = 1;

                BroadCast(skill);

                // 데미지 판정
                Vector2int skillPos = player.GetFrontCellPos(info.PosInfo.Movedir);
                Player target = _map.Find(skillPos);
                if(target != null)
                {
                    Console.WriteLine("Hit Player!");
                }
            }
        }

        public void BroadCast(IMessage packet)
        {
            lock(_lock)
            {
                foreach(Player p in _players.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }
    }
}
