using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class MonsterController : CreatureController
{

    private Coroutine _coSkill = null;


    protected override void Init()
    {
        base.Init();

        //State = CreatureState.Idle;
        //Dir = MoveDir.Down;
    }



    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        // 몬스터를 오브젝트 매니저에서 삭제
        //Managers.Object.Remove(Id);
        // 리소스도 삭제합니다.
        //Managers.Resource.Destroy(this.gameObject);
    }

    public override void UseSkill(int skillid)
    {
        if (skillid == 1)
        {
            State = CreatureState.Skill;
        }
        else if (skillid == 2)
        {
            
        }
    }
}
