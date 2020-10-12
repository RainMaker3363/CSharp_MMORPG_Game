using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class MonsterController : CreatureController
{
    private Coroutine _coPatrol = null;
    private Vector3Int _destCellPos = new Vector3Int();

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;
            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }



    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if(_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int moveCellDir = _destCellPos - CellPos;

        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;

            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;

            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;

            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos) && Managers.Object.Find(destPos) == null)
        {
            CellPos = destPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        // TEMP
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = this.transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);


        // 몬스터를 오브젝트 매니저에서 삭제
        Managers.Object.Remove(this.gameObject);
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

            if(Managers.Map.CanGo(RnddestPos) && Managers.Object.Find(RnddestPos) == null)
            {
                _destCellPos = RnddestPos;
                State = CreatureState.Moving;

                yield break;
            }
        }

        State = CreatureState.Idle;

    }
}
