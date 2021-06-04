using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class CreatureController : BaseController
{
    HPBar _hpBar = null;

    public override StatInfo Stat
    {
        get
        {
            return base._stat;
        }
        set
        {
            base._stat = value;
            UpdateHPBar();
        }
    }

    public override int Hp
    {
        get { return Stat.Hp; }
        set
        {
            base.Hp = value;
            UpdateHPBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.5f, 0);
        go.name = "HPBar";
        _hpBar = go.GetComponent<HPBar>();

        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;
        if(Stat.MaxHp > 0)
        {
            ratio = (((float)Hp) / Stat.MaxHp);
        }

        _hpBar.SetHPBar(ratio);
    }



    protected override void Init()
    {
        base.Init();

        AddHpBar();
    }

    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        // TEMP
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = this.transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);


    }
}
