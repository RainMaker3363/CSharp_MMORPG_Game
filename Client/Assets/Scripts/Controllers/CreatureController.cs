using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    protected Vector3Int _cellPos = Vector3Int.zero;

    protected Animator _animator = null;

    protected SpriteRenderer _sprite;

    private CreatureState _state = CreatureState.Idle;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }

    private MoveDir _dir = MoveDir.Down;
    private MoveDir _lastDir = MoveDir.Down;
    public MoveDir Dir
    {
        get
        {
            return _dir;
        }

        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    protected virtual void UpdateAnimation()
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
        else if(State == CreatureState.Skill)
        {

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

        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        UpdateIsMoving();
        UpdatePosition();
    }

    // 스르륵 이동
    void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;

            // 예외적으로 애니메이션 직접 컨트롤
            _state = CreatureState.Idle;
            if (_dir == MoveDir.None)
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    // 이동 가능 상태일 때, 실제 좌표를 이동
    void UpdateIsMoving()
    {

        if (State == CreatureState.Idle && Dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos;

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

            if (Managers.Map.CanGo(destPos))
            {
                _cellPos = destPos;
                State = CreatureState.Moving;
            }
        }
    }
}
