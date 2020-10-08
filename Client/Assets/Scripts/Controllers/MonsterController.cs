using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateController()
    {
        base.UpdateController();
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
}
