using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private GridSettings _gridSettings;

    [Header("Events")]
    public UnityEvent OnGridCreated;

    [Header("Grid Settings")]
    [SerializeField] private GameObject _cellPrefab;
    public int Width => _width;
    [Range(min: 5, max: 25)][SerializeField] private int _width = 10;
    public int Height => _height;
    [Range(min: 5, max: 25)][SerializeField] private int _height = 10;

    [Header("Flag")]
    public Vector2Int FlagSpawnPosition => _flagSpawnPosition;
    [SerializeField] private Vector2Int _flagSpawnPosition;
    [SerializeField] private GameObject _flagPrefab;

    [Header("Containers/Flag")]
    [SerializeField] private Transform _flagContainer;
    [Header("Grid Container")]
    [SerializeField] private Transform _gridContainer;

    public Grid<Cell> Grid => _grid;
    [SerializeField] private Grid<Cell> _grid;

    private void Awake()
    {
        _width = _gridSettings.gridSize;
        _height = _gridSettings.gridSize;

        _flagSpawnPosition = new Vector2Int(Random.Range(0, Width - 1), Random.Range(0, Height - 1));

        CreateInstance();
        CreateGrid();
        CreateFlag();
    }

    public static async Task<GridManager> GetInstanceAsync()
    {
        if (Instance == null)
        {
            while (Instance == null)
            {
                await Task.Yield();
            }
            return Instance;
        }
        else
        {
            return Instance;
        }
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 position = new Vector3(x * _grid.CellSize, 0, y * _grid.CellSize);
                GameObject cellObj = Instantiate(_cellPrefab, position, Quaternion.identity, _gridContainer);
                cellObj.name = $"Cell [{x:D2},{y:D2}]";
                Cell cell = cellObj.GetComponent<Cell>();

                int randTileType = Random.Range(0, 101);

                cell.InitializeCell(gridPosition: new(x, y), cellType:
                    randTileType < _gridSettings.probabilityPoisonSpawn ?
                    CellType.Poison :
                    CellType.Normal
                );
                _grid.AddCell(x, y, cell);
            }
        }
    }

    private void CreateFlag()
    {
        SpawnFlag(_flagPrefab, _flagSpawnPosition, _flagContainer);
    }

    private async void SpawnFlag(GameObject prefab, Vector2Int spawnPos, Transform parent)
    {
        Vector2Int clampedPos =
            new(
                spawnPos.x > _width ? _width - 1 : spawnPos.x < 0 ? 0 : spawnPos.x,
                spawnPos.y > _height ? _height - 1 : spawnPos.y < 0 ? 0 : spawnPos.y
            );

        GameObject obj = Instantiate(prefab, _grid.GetCell(clampedPos.x, clampedPos.y).transform.position, Quaternion.identity, parent);

        var cM = await CameraManager.GetInstanceAsync();
        cM.SetFlag(obj.transform);
    }

    private void OnDrawGizmos()
    {
        if (_grid == null) return;
        Gizmos.color = Color.red;
        foreach (var cell in _grid.Cells)
        {
            Vector3 cellPos = new Vector3(cell.Value.GridPosition.x, 0, cell.Value.GridPosition.y);
            Gizmos.DrawWireCube(cellPos, new Vector3(Grid.CellSize, 0.1f, Grid.CellSize));
        }
    }
}