using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        // FSM(Infinite State Machine)
        public override void Update()
        {
            switch(State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;

                case CreatureState.Moving:
                    UpdateMoving();
                    break;

                case CreatureState.Skill:
                    UpdateSkill();
                    break;

                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }

        Player _target;
        int _searchCellDist = 10;
        int _chaseCellDist = 20;

        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;

            _nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindPlayer(p =>
            {
                Vector2int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero <= _searchCellDist;
            });

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }

        int _skillRange = 1;
        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if(_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();

                return;
            }

            Vector2int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            if(dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();

                return;
            }

            List<Vector2int> path = Room._Map.FindPath(CellPos, _target.CellPos, checkObjects : false);
            if(path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();

                return;
            }

            // 스킬로 넘어갈지 체크
            if(dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room._Map.ApplyMove(this, path[1]);

            BroadCastMove();
        }

        void BroadCastMove()
        {
            // 다른 플레이어에게 알려줍니다.
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;

            Room.BroadCast(movePacket);
        }

        long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟
                if (_target == null || _target.Room != Room || _target.HP <= 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadCastMove();
                    return;
                }

                // 스킬이 아직 사용 가능한지
                Vector2int dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if (canUseSkill == false)
                {
                    State = CreatureState.Moving;
                    BroadCastMove();
                    return;
                }

                // 타게팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadCastMove();
                }

                SKill skilldata = null;
                DataManager.SkillDict.TryGetValue(1, out skilldata);

                // 데미지 판정
                _target.OnDamaged(this, skilldata.damage + Stat.Attack);

                // 스킬 사용 BroadCast
                S_Skill skillPacket = new S_Skill() { Info = new SKillInfo() };
                skillPacket.ObjectId = Id;
                skillPacket.Info.Skillid = skilldata.id;
                Room.BroadCast(skillPacket);

                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skilldata.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }

        protected virtual void UpdateDead()
        {

        }


    }
}
