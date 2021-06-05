using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }
    protected StatInfo _stat = new StatInfo();
    
    public virtual StatInfo Stat
    {
        get
        {
            return _stat;
        }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public virtual int Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
        }
    }


    protected bool _updated = false;

    PositionInfo _posItionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _posItionInfo; }
        set
        {
            if (_posItionInfo.Equals(value))
                return;

            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.Movedir;
        }
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set
        {
            if (PosInfo.PosX == value.x &&
                PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }

    protected Animator _animator = null;

    protected SpriteRenderer _sprite;


    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
            _updated = true;
        }
    }

    public MoveDir Dir
    {
        get
        {
            return PosInfo.Movedir;
        }

        set
        {
            if (PosInfo.Movedir == value)
                return;

            PosInfo.Movedir = value;

            UpdateAnimation();
            _updated = true;
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else
            return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;

            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;

            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;

            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
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
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;

            }
        }
        else
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        //State = CreatureState.Idle;
        //Dir = MoveDir.Down;
        //CellPos = new Vector3Int(0, 0, 0);
        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;

            case CreatureState.Moving:
                UpdatePosition();
                break;

            case CreatureState.Skill:
                UpdateSkill();
                break;

            case CreatureState.Dead:
                UpdateDead();
                break;
        }


    }

    // 스르륵 이동
    protected virtual void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    // 이동 가능 상태일 때, 실제 좌표를 이동
    protected virtual void UpdateIdle()
    {

    }

    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

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
