using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine _coSkill;
    bool _rangeSkill = false;

    protected override void Init()
    {
        base.Init();

    }

    protected override void UpdateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;

            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    protected override void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    {
                        _animator.Play("IDLE_BACK");
                        _sprite.flipX = false;
                    }
                    break;

                case MoveDir.Down:
                    {
                        _animator.Play("IDLE_FRONT");
                        _sprite.flipX = false;
                    }
                    break;

                case MoveDir.Left:
                    {
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = true;
                    }
                    break;

                case MoveDir.Right:
                    {
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = false;
                    }
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;

            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;

            }
        }
        else
        {

        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    /// <summary>
    ///  키보드 입력
    /// </summary>
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //_coSkill = StartCoroutine("coPunchSkill");
            _coSkill = StartCoroutine("coStartShootArrow");
        }
    }

    IEnumerator coPunchSkill()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        _rangeSkill = false;

        // 대기 시간
        yield return new WaitForSeconds(0.5f);

        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator coStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        _rangeSkill = true;
        // 대기 시간
        yield return new WaitForSeconds(0.3f);

        State = CreatureState.Idle;
        _coSkill = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit!!");
    }
}
