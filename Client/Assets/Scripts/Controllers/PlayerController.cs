﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangeSkill = false;

    protected override void Init()
    {
        base.Init();

    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected override void UpdateAnimation()
    {
        if (_animator == null || _sprite == null)
            return;

        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
            switch (Dir)
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
            switch (Dir)
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

    public void UseSkill(int skillid)
    {
        if(skillid == 1)
        {
            _coSkill = StartCoroutine("coPunchSkill");
        }
    }

    protected virtual void CheckUpdatedFlag()
    {

    }

    IEnumerator coPunchSkill()
    {
        _rangeSkill = false;

        State = CreatureState.Skill;

        // 대기 시간
        yield return new WaitForSeconds(0.5f);

        State = CreatureState.Idle;
        _coSkill = null;

        CheckUpdatedFlag();
    }

    IEnumerator coStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = Dir;
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
