using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 0.1f;

    public bool IsMoving => _isMoving;
    [SerializeField] private bool _isMoving = false;

    public bool IsEnemy => UnitType != UnitType.Ant;

    [SerializeField] private Vector2Int _gridPos;

    public Vector2Int TargetWorldPosition => _targetWorldPosition;
    private Vector2Int _targetWorldPosition;
    public UnitType UnitType => _unitType;
    [SerializeField] private UnitType _unitType;

    public UnitState UnitState => _unitState;
    [SerializeField] private UnitState _unitState = UnitState.Alive;

    public GameObject GameObject => gameObject;

    [SerializeField] private Transform _visual;
    [SerializeField] private float _moveThreshold = 0.01f;

    [SerializeField] private UnityEvent<IUnit> _onUnitDied;

    public Grid<Cell> GridRef => _gridRef;
    private Grid<Cell> _gridRef;

    private Vector2Int _flagPosition;

    public GameSettings GameSettings => _gameSettings;
    [SerializeField] private GameSettings _gameSettings;

    private Transform _tr;


    public void Awake()
    {
        _tr = transform;
        _unitState = UnitState.Alive;
    }

    public void Initialize(Grid<Cell> grid, Vector2Int flagPosition, UnityAction<IUnit> onUnitDied)
    {
        _gridRef = grid;
        _flagPosition = flagPosition;
        _onUnitDied.AddListener(onUnitDied);
    }

    public void SetGridPosition(int i, int j) => _gridPos = new (i, j);

    public void SetUnitState(UnitState unitState) => _unitState = unitState;

    public (int i, int j) GetGridPosition() => (_gridPos.x, _gridPos.y);

    public void SetMoveSpeed(float speed) => _moveSpeed = speed;
    public void SetMoveThreshold(float threshold) => _moveThreshold = threshold;
    public void SetRotationSpeed(float rotationSpeed) => _rotationSpeed = rotationSpeed;

    public void MoveTo(Vector2Int gridPos)
    {
        _targetWorldPosition = _gridRef.GridToWorld(gridPos.x, gridPos.y);
        _isMoving = true;
    }

    public void Die()
    {
        if (_unitState == UnitState.Dead) return;

        _unitState = UnitState.Dead;
        _onUnitDied?.Invoke(this);
        Debug.Log($"Unit {gameObject.name} died.");
        gameObject.SetActive(false); // or destroy or animate
    }

    public void FixedUpdate()
    {
        if (_gameSettings.isPaused || !_gameSettings.isGameStarted)
            return;

        if (_gridRef == null) return;

        var targetWorldPosition = new Vector3(_targetWorldPosition.x, 0, _targetWorldPosition.y);
        _tr.position = Vector3.MoveTowards(_tr.position, targetWorldPosition, _moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(new Vector2(_tr.position.x, _tr.position.z), _targetWorldPosition) < 0.1f)
        {
            // 1) Movement complete
            _isMoving = false;

            // 2) Update our grid unit references
            _gridRef.Move(_gridPos, _targetWorldPosition, this);

            // 3) Update the grid position
            _gridPos = _targetWorldPosition;

            PoisonEffectCheck();

            if (_flagPosition == _gridPos && IsEnemy)
            {
                if (!MainMenuManager.Instance.GameOver)
                {
                    MainMenuManager.Instance.EndGame(win: false);
                }
            }
        }

        var lookDir = targetWorldPosition - _tr.position;
        if (lookDir != Vector3.zero)
        {
            _visual.rotation = Quaternion.Slerp(_visual.rotation, Quaternion.LookRotation(lookDir, Vector3.up), _rotationSpeed);
        }
    }

    private void PoisonEffectCheck()
    {
        switch (_gridRef.GetCell(_gridPos.x, _gridPos.y).CellType)
        {
            case CellType.Normal:
                break;
            case CellType.Poison:
                if (IsEnemy && UnitState == UnitState.Alive)
                {
                    Debug.Log($"Unit {gameObject.name} died from poison.");
                    Die();
                }
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isMoving) return;

        Gizmos.color = _unitType == UnitType.Ant? Color.green : Color.blue;
        var targetWorldPosition = new Vector3(_targetWorldPosition.x, 0, _targetWorldPosition.y);
        Gizmos.DrawWireSphere(targetWorldPosition, .2f);

        var curPos = new Vector3(_tr.position.x, 0, _tr.position.z);
        Ray r = new Ray(curPos, targetWorldPosition - curPos);
        Gizmos.DrawRay(r);
        //Gizmos.DrawWireSphere(_tr.position, .2f);
    }

}