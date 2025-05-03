using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private UnitSpawnSettings _unitSpawnSettings;
    private GridManager gridManager;

    [Header("Unit Update Settings")]
    [SerializeField] private float updateInterval = 1f;

    [Header("Prefab/Units")]
    [SerializeField] private GameObject _antPrefab;
    [SerializeField] private GameObject _aphidPrefab;
    [SerializeField] private GameObject _beetlePrefab;
    [SerializeField] private GameObject _ladyBugPrefab;

    [Header("Spawns/Units")]
    [SerializeField] private List<Vector2Int> _antSpawns = new();
    [SerializeField] private List<Vector2Int> _aphidSpawns = new();
    [SerializeField] private List<Vector2Int> _beetleSpawns = new();
    [SerializeField] private List<Vector2Int> _ladyBugSpawns = new();

    [Header("Containers/Units")]
    [SerializeField] private Transform _antContainer;
    [SerializeField] private Transform _aphidContainer;
    [SerializeField] private Transform _beetleContainer;
    [SerializeField] private Transform _ladyBugContainer;

    public List<IUnit> AllUnits { get; private set; } = new();
    public List<IUnit> EliminatedUnits { get; private set; } = new();
    private IEnumerator _enemyMoveCoroutine;
    private IEnumerator _antMoveCoroutine;

    private Camera _cam;

    private async void Awake()
    {
        _cam = Camera.main;
        gridManager = await GridManager.GetInstanceAsync();
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        while(gridManager == null)
        {
            yield return null;
        }
        GenerateRandomSpawns();
        CreateUnits();

        if (_enemyMoveCoroutine != null)
            StopCoroutine(_enemyMoveCoroutine);

        _enemyMoveCoroutine = EnemyMoveCoroutine();
        StartCoroutine(_enemyMoveCoroutine);

        if (_antMoveCoroutine != null)
            StopCoroutine(_antMoveCoroutine);
        _antMoveCoroutine = AntMoveCoroutine();
        StartCoroutine(_antMoveCoroutine);
    }

    private void GenerateRandomSpawns()
    {
        _antSpawns = new();
        for (int i = 0; i < _unitSpawnSettings.antCount; i++)
        {
            Vector2Int spawnPos = new(
                Random.Range(0, gridManager.Width),
                Random.Range(0, gridManager.Height)
            );
            _antSpawns.Add(spawnPos);
        }
        _aphidSpawns = new();
        for (int i = 0; i < _unitSpawnSettings.aphidCount; i++) {
            Vector2Int spawnPos = new(
                Random.Range(0, gridManager.Width),
                Random.Range(0, gridManager.Height)
            );
            _aphidSpawns.Add(spawnPos);
        }
        _beetleSpawns = new();
        for (int i = 0; i < _unitSpawnSettings.beetleCount; i++)
        {
            Vector2Int spawnPos = new(
                Random.Range(0, gridManager.Width),
                Random.Range(0, gridManager.Height)
            );
            _beetleSpawns.Add(spawnPos);
        }
        _ladyBugSpawns = new();
        for (int i = 0; i < _unitSpawnSettings.ladybugCount; i++)
        {
            Vector2Int spawnPos = new(
                Random.Range(0, gridManager.Width),
                Random.Range(0, gridManager.Height)
            );
            _ladyBugSpawns.Add(spawnPos);
        }
    }

    /// <summary>
    /// Updates the cell (pos.x, pos.y) to the given cellType.
    /// Creates the units in the grid based on the spawn positions defined in the inspector.
    /// </summary>
    private void CreateUnits()
    {
        foreach (var spawnPos in _antSpawns)
        {
            gridManager.Grid.GetCell(spawnPos.x, spawnPos.y).SetCellType(cellType: CellType.AntSpawn);
            IUnit antUnit = SpawnObject(_antPrefab, spawnPos, _antContainer);
            Ant ant = (Ant)antUnit;
            PlayerController playerController = ant.GetComponent<PlayerController>();
            playerController.Initialize(ant, OnTargetClick);
            ant.Initialize(gridManager.Grid, gridManager.FlagSpawnPosition, onUnitDied: (IUnit unit) => { OnUnitDeathEvent(unit); });

        }

        foreach (var spawnPos in _aphidSpawns)
        {
            gridManager.Grid.GetCell(spawnPos.x, spawnPos.y).SetCellType(cellType: CellType.AphidSpawn);
            SpawnObject(_aphidPrefab, spawnPos, _aphidContainer);
        }
        foreach (var spawnPos in _beetleSpawns)
        {
            gridManager.Grid.GetCell(spawnPos.x, spawnPos.y).SetCellType(cellType: CellType.BeetleSpawn);
            SpawnObject(_beetlePrefab, spawnPos, _beetleContainer);
        }
        foreach (var spawnPos in _ladyBugSpawns)
        {
            gridManager.Grid.GetCell(spawnPos.x, spawnPos.y).SetCellType(cellType: CellType.LadybugSpawn);
            SpawnObject(_ladyBugPrefab, spawnPos, _ladyBugContainer);
        }
    }

    private Unit SpawnObject(GameObject prefab, Vector2Int spawnPos, Transform parent)
    {
        (int i, int j) clampedGridPos =
            new(
                spawnPos.x > gridManager.Width ? gridManager.Width - 1 : spawnPos.x < 0 ? 0 : spawnPos.x,
                spawnPos.y > gridManager.Height ? gridManager.Height - 1 : spawnPos.y < 0 ? 0 : spawnPos.y
            );

        GameObject unitObj = Instantiate(prefab, gridManager.Grid.GetCell(clampedGridPos.i, clampedGridPos.j).transform.position, Quaternion.identity, parent);
        gridManager.Grid.RegisterUnit(clampedGridPos.i, clampedGridPos.j, unitObj.GetComponent<IUnit>());

        Unit u = unitObj.GetComponent<Unit>();
        u.SetGridPosition(clampedGridPos.i, clampedGridPos.j);
        u.Initialize(gridManager.Grid, flagPosition: gridManager.FlagSpawnPosition, onUnitDied: (IUnit unit) => { OnUnitDeathEvent(unit); });

        AllUnits.Add(u);
        return u;
    }

    private IEnumerator EnemyMoveCoroutine()
    {
        while (true)
        {
            if (_gameSettings.isPaused || !_gameSettings.isGameStarted)
                yield return null;

            var enemyUnits = AllUnits.Where(u => u.UnitType != UnitType.Ant).ToList();
            MoveAllEnemies(enemyUnits);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    public void MoveAllEnemies(List<IUnit> enemyUnits)
    {
        foreach (var enemyUnit in enemyUnits)
        {
            if (enemyUnit.IsMoving) 
                continue;

            (int i, int j) = enemyUnit.GetGridPosition();

            Vector2Int dir = (gridManager.FlagSpawnPosition - new Vector2Int(i, j));
            
            (int i, int j) nextPos = new(
                dir.x != 0 ? (int)Mathf.Sign(dir.x) : 0,
                dir.y != 0 ? (int)Mathf.Sign(dir.y) : 0
            );
            
            (int nextI, int nextJ) = (i + nextPos.i, j + nextPos.j);
            enemyUnit.MoveTo(new Vector2Int(nextI, nextJ));
            //Debug.Log($"Moving {enemyUnit.GameObject.name} From: [{i}, {j}] to: [{nextI}, {nextJ}]");
        }
    }

    private IEnumerator AntMoveCoroutine()
    {
        while (true)
        {
            if (_gameSettings.isPaused || !_gameSettings.isGameStarted)
                yield return null;

            var antUnits = AllUnits.Where(u => u.UnitType == UnitType.Ant).ToList();
            MoveAllAntDefenders(antUnits);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// AI Control for Ant Defenders.
    /// - Find the closest enemy unit within a given range of cells.
    ///     - Starting from our given Ant's grid position
    ///     - Search from -searchRadius to +searchRadius, horizontally and vertically
    /// - Move towards the enemy unit.
    /// </summary>
    public void MoveAllAntDefenders(List<IUnit> antUnits)
    {
        foreach (var antUnit in antUnits)
        {
            if(antUnit.GameSettings.isPlayerControlled)
                continue;

            if (antUnit.IsMoving)
                continue;

            Ant ant = antUnit as Ant;

            (int i, int j) antPos = ant.GetGridPosition();
            IUnit closestEnemy = null;
            int closestDistance = int.MaxValue;

            for (int dx = -ant.searchRange; dx <= ant.searchRange; dx++)
            {
                for (int dy = -ant.searchRange; dy <= ant.searchRange; dy++)
                {
                    (int i, int j) cellToScan = (antPos.i + dx, antPos.j + dy);

                    // the Grid's Cell-Units dictionary does not contain any Units. Skip.
                    if (!gridManager.Grid.Units.TryGetValue(cellToScan, out var unitsAtPos))
                        continue;

                    // This cell contains at least 1 unit. Check the units in the given cell for enemies.
                    foreach (var u in unitsAtPos)
                    {
                        // Skip if the unit is an Ant (Since Ants should not be targeting each other)
                        if (u.UnitType == UnitType.Ant)
                            continue;

                        int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestEnemy = u;
                            Debug.Log("Found Closest Enemy: " + closestEnemy.GameObject.name);
                        }
                    }
                }
            }

            ant.useRandomMovement = closestEnemy == null;

            if (closestEnemy != null)
            {
                Debug.Log($"Closest Enemy: {closestEnemy.GameObject.name} at [{closestEnemy.GetGridPosition().i}, {closestEnemy.GetGridPosition().j}]");
            }

            (int i, int j) antGridPos = ant.GetGridPosition();
            (int i, int j) nextGridPos = closestEnemy != null ?
                closestEnemy.GetGridPosition() :
                (Random.Range(0, gridManager.Width), Random.Range(0, gridManager.Height));

            Vector2Int dir = new Vector2Int(nextGridPos.i, nextGridPos.j) - new Vector2Int(antGridPos.i, antGridPos.j);
            (int i, int j) nextPos = new(
                dir.x != 0 ? (int)Mathf.Sign(dir.x) : 0,
                dir.y != 0 ? (int)Mathf.Sign(dir.y) : 0
            );

            (int nextI, int nextJ) = (antGridPos.i + nextPos.i, antGridPos.j + nextPos.j);
            ant.MoveTo(new Vector2Int(nextI, nextJ));
        }
    }

    public void OnTargetClick(Ant ant, Vector3 screenPosition)
    {
        Ray ray = _cam.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: gridManager.Grid.GridLayerMask))
        {
            Vector2Int cellIndex = gridManager.Grid.WorldToGrid(hit.point.x, hit.point.z);
            ant.MoveTo(cellIndex);
        }
    }


    private void OnUnitDeathEvent(IUnit unit)
    {
        (int i, int j) = unit.GetGridPosition();
        gridManager.Grid.UnregisterUnit(i, j, unit);
        AllUnits.Remove(unit);
        
        MainMenuManager.Instance.SetEnemiesRemaining(AllUnits.Count(u => u.IsEnemy));
        
        EliminatedUnits.Add(unit);
        MainMenuManager.Instance.SetScore(EliminatedUnits.Count());
        
        var enemyUnits = AllUnits.Where(u => u.UnitType != UnitType.Ant).ToList();
        
        if (enemyUnits.Count == 0)
            MainMenuManager.Instance.EndGame(win: true);
    }

}