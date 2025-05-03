using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid<TCell>
{
    public LayerMask GridLayerMask => _gridLayerMask;
    [SerializeField] private LayerMask _gridLayerMask;

    public int CellSize => _cellSize;
    [SerializeField]private int _cellSize;

    public Dictionary<(int, int), TCell> Cells => _cells;
    private Dictionary<(int, int), TCell> _cells = new();

    public Dictionary<(int, int), List<IUnit>> Units => _units;
    private Dictionary<(int, int), List<IUnit>> _units = new();

    public void AddCell(int i, int j, TCell cell)
    {
        _cells[(i, j)] = cell;
    }

    public void DeleteCell(int i, int j)
    {
        _cells.Remove((i, j));
        _units.Remove((i, j));
    }

    public void RegisterUnit(int i, int j, IUnit unit)
    {
        if (!_units.ContainsKey((i, j)))
            _units[(i, j)] = new List<IUnit>();

        _units[(i, j)].Add(unit);
    }

    public void UnregisterUnit(int i, int j, IUnit unit)
    {
        if (_units.TryGetValue((i, j), out var list))
        {
            if (!list.Remove(unit))
            {
                //Debug.LogWarning($"Unit {unit.GameObject.name} not found in [{i:D2}, {j:D2}]");
            }

            if (list.Count == 0)
                _units.Remove((i, j));
        }
        //else { Debug.LogWarning($"No unit at [{i:D2}, {j:D2}] to unregister."); }
    }

    public void Move(Vector2Int from, Vector2Int to, IUnit unit)
    {
        UnregisterUnit(from.x, from.y, unit);
        RegisterUnit(to.x, to.y, unit);


        if (!_units.TryGetValue((from.x, from.y), out var u))
        {
            //Debug.LogWarning($"No unit at [{from.x:D2}, [{from.y:D2}] to move.");
            return;
        }

        if (_units.ContainsKey((to.x, to.y)))
        {
            //Debug.LogWarning($"Target position [{from.x:D2} , [ {from.y:D2}] is already occupied.");
            return;
        }

        _units.Remove((from.x, from.y));
        _units[(to.x, to.y)].Add(unit);

        // We are setting the Unit's grid position.
        // Each unit will handle their respective movement.
        unit.SetGridPosition(to.x, to.y);
    }

    public TCell GetCell(int i, int j)
    {
        return _cells.TryGetValue((i, j), out var cell) ? cell : default;
    }

    public List<IUnit> GetUnitsAt(int i, int j)
    {
        //if ((_units.TryGetValue((i, j), out _)))
        //    Debug.Log($"Found Units at [{i}, {j}]");
        return _units.TryGetValue((i, j), out var unitList) ? unitList : new List<IUnit>();
    }

    public Vector2Int GridToWorld(int i, int j)
    {
        return new Vector2Int(i * CellSize, j * CellSize);
    }

    public Vector2Int WorldToGrid(float x, float y)
    {
        return new Vector2Int(Mathf.RoundToInt(x / CellSize), Mathf.RoundToInt(y / CellSize));
    }
}