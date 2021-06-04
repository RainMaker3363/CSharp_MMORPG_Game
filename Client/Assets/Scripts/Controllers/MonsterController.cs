using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class MonsterController : CreatureController
{
    private Coroutine _coPatrol = null;
    private Coroutine _coSearch = null;

    private Coroutine _coSkill = null;

    [SerializeField]
    private Vector3Int _destCellPos = new Vector3Int();

    [SerializeField]
    private GameObject _target;
    [SerializeField]
    float _searchRange = 10.0f;
    [SerializeField]
    float _skillRange = 1.0f;
    [SerializeField]
    bool _rangeSkill = false;

    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            base.State = value;
            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }

            if(_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        _rangeSkill = (Random.Range(0, 2) == 0) ? true : false;

        if (_rangeSkill)
            _skillRange = 10.0f;
        else
            _skillRange = 1.0f;
    }



    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if(_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if(_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;
            if(dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;

                if(_rangeSkill)
                    _coSkill = StartCoroutine("coStartShootArrow");
                else
                    _coSkill = StartCoroutine("coPunchSkill");

                return;
            }
        }

        // A*를 이용한 길 찾기
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if(path.Count < 2 || (_target != null && path.Count > 20))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        
        Vector3Int moveCellDir = nextPos - CellPos;


        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.CanGo(nextPos) && Managers.Object.FindCreature(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        // 몬스터를 오브젝트 매니저에서 삭제
        Managers.Object.Remove(Id);
        // 리소스도 삭제합니다.
        Managers.Resource.Destroy(this.gameObject);
    }

    IEnumerator CoPatrol()
    {
        int waitSec = Random.Range(1, 4);

        yield return new WaitForSeconds(waitSec);

        for(int i = 0; i< 10; ++i)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);

            Vector3Int RnddestPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if(Managers.Map.CanGo(RnddestPos) && Managers.Object.FindCreature(RnddestPos) == null)
            {
                _destCellPos = RnddestPos;
                State = CreatureState.Moving;

                yield break;
            }
        }

        State = CreatureState.Idle;

    }

    IEnumerator CoSearch()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);

            if (_target != null)
                continue;

            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator coPunchSkill()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        // 대기 시간
        yield return new WaitForSeconds(0.5f);

        State = CreatureState.Moving;
        _coSkill = null;
    }

    IEnumerator coStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = Dir;
        ac.CellPos = CellPos;


        // 대기 시간
        yield return new WaitForSeconds(0.3f);

        State = CreatureState.Moving;
        _coSkill = null;
    }
}
