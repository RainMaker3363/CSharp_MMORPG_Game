﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomID { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map _Map { get; private set; } = new Map();

        public void init(int mapid)
        {
            _Map.LoadMap(mapid);

            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.CellPos = new Vector2int(5, 5);
            EnterGame(monster);
        }

        public void Update()
        {
            foreach (Monster monster in _monsters.Values)
            {
                monster.Update();
            }

            foreach (Projectile projectile in _projectiles.Values)
            {
                projectile.Update();
            }

            Flush();
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                player.Room = this;

                // 방 입장
                _Map.ApplyMove(player, new Vector2int(player.CellPos.x, player.CellPos.y));

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;

                    player.Session.Send(enterPacket);


                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                        {
                            spawnPacket.Objects.Add(p.Info);
                        }
                    }

                    foreach (Monster m in _monsters.Values)
                    {
                        spawnPacket.Objects.Add(m.Info);
                    }

                    foreach (Projectile p in _projectiles.Values)
                    {
                        spawnPacket.Objects.Add(p.Info);
                    }

                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;

                // 방 입장
                _Map.ApplyMove(monster, new Vector2int(monster.CellPos.x, monster.CellPos.y));
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }

            // 타인에게 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                    {
                        p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                
                // 실제 서버 맵에서도 반영을 해줍니다.
                _Map.ApplyLeave(player);
                player.Room = null;

                // 본인한테 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                
                // 실제 서버 맵에서도 반영을 해줍니다.
                _Map.ApplyLeave(monster);
                monster.Room = null;

            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;

                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                projectile.Room = null;
            }


            // 타인한테 정보 전송
            {
                S_Despawn despawn = new S_Despawn();
                despawn.ObjectIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                    {
                        p.Session.Send(despawn);
                    }
                }
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            // 일단 서버에서 좌표 이동
            PositionInfo MovePosinfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (MovePosinfo.PosX != info.PosInfo.PosX || MovePosinfo.PosY != info.PosInfo.PosY)
            {
                if (_Map.CanGo(new Vector2int(MovePosinfo.PosX, MovePosinfo.PosY)) == false)
                    return;
            }

            info.PosInfo.State = MovePosinfo.State;
            info.PosInfo.Movedir = MovePosinfo.Movedir;
            _Map.ApplyMove(player, new Vector2int(MovePosinfo.PosX, MovePosinfo.PosY));

            // 다른 플레이어한테도 알려준다.
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;

            BroadCast(resMovePacket);
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            // 일단 서버에서 좌표 이동
            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // 스킬 사용 가능 여부 체크

            info.PosInfo.State = CreatureState.Skill;

            S_Skill skill = new S_Skill() { Info = new SKillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.Skillid = skillPacket.Info.Skillid;

            BroadCast(skill);

            Data.SKill SkillData = null;
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.Skillid, out SkillData) == false)
                return;

            switch (SkillData.skillType)
            {
                case SkillType.SkillAuto:
                    {
                        // 데미지 판정
                        Vector2int skillPos = player.GetFrontCellPos(info.PosInfo.Movedir);
                        GameObject target = _Map.Find(skillPos);
                        if (target != null)
                        {
                            Console.WriteLine("Hit GameObject!");
                        }
                    }
                    break;

                case SkillType.SkillProjectile:
                    {
                        // 화살 스킬
                        Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                        if (arrow == null)
                            return;

                        arrow.Owner = player;
                        arrow.Data = SkillData;
                        arrow.PosInfo.State = CreatureState.Moving;
                        arrow.PosInfo.Movedir = player.PosInfo.Movedir;
                        arrow.PosInfo.PosX = player.PosInfo.PosX;
                        arrow.PosInfo.PosY = player.PosInfo.PosY;
                        arrow.Speed = SkillData.projectile.speed;

                        Push(EnterGame, arrow);
                    }
                    break;
            }
        }

        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            foreach(Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
    }
}
