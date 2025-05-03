using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// All units should implement this interface.
/// </summary>
public interface IUnit
{
    GameObject GameObject { get; }
    UnitType UnitType { get; }
    UnitState UnitState { get; }
    bool IsMoving { get; }
    bool IsEnemy { get; }
    GameSettings GameSettings { get; }
    void Initialize(Grid<Cell> grid, Vector2Int flagSpawnPos, UnityAction<IUnit> onUnitDied);
    void SetGridPosition(int x, int y);
    (int i, int j) GetGridPosition();
    void SetMoveSpeed(float speed);
    void SetMoveThreshold(float threshold);
    void SetRotationSpeed(float rotationSpeed);

    void MoveTo(Vector2Int gridPos);
    void Die();
}

public enum UnitType
{
    Ant,
    Aphid,
    Beetle,
    Ladybug,
}

public enum UnitState
{
    Alive,
    Dead
}