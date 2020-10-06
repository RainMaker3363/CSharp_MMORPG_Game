using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public float _speed = 5.0f;

    private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;

    private Animator _animator = null;
    private MoveDirection _dir = MoveDirection.Down;
    public MoveDirection Dir
    {
        get
        {
            return _dir;
        }

        set
        {
            if (_dir == value)
                return;

            switch(value)
            {
                case MoveDirection.Up:
                    _animator.Play("WALK_BACK");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;

                case MoveDirection.Down:
                    _animator.Play("WALK_FRONT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;

                case MoveDirection.Left:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    break;

                case MoveDirection.Right:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;

                case MoveDirection.None:
                    {
                        if (_dir == MoveDirection.Up)
                        {
                            _animator.Play("IDLE_BACK");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else if (_dir == MoveDirection.Down)
                        {
                            _animator.Play("IDLE_FRONT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else if (_dir == MoveDirection.Left)
                        {
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }

                    }

                    break;
            }

            _dir = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirInput();
        UpdateIsMoving();
        UpdatePosition();
    }

    /// <summary>
    ///  키보드 입력
    /// </summary>
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            Dir = MoveDirection.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            Dir = MoveDirection.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            Dir = MoveDirection.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            Dir = MoveDirection.Right;
        }
        else
        {
            Dir = MoveDirection.None;
        }
    }

    // 스르륵 이동
    void UpdatePosition()
    {
        if (_isMoving == false)
            return;

        Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            _isMoving = true;
        }
    }

    // 이동 가능 상태일 때, 실제 좌표를 이동
    void UpdateIsMoving()
    {

        if (_isMoving == false)
        {
            switch (_dir)
            {
                case MoveDirection.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true;
                    break;

                case MoveDirection.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true;
                    break;

                case MoveDirection.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true;
                    break;

                case MoveDirection.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true;
                    break;
            }
        }
    }
}
