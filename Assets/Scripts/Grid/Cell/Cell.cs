using UnityEngine;

/// <summary>
/// We can use this enum to define the type of cell.
/// Unit behaviors can be defined based on the cell type.
/// </summary>
public enum CellType
{
    AntSpawn,
    AphidSpawn,
    BeetleSpawn,
    LadybugSpawn,
    FlagSpawn,
    Normal,
    Poison,
    Water,
    None,
}

public class Cell : MonoBehaviour
{
    public CellType CellType => _cellType;
    [SerializeField] private CellType _cellType;

    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _poisonMaterial;
    [SerializeField] Material _waterMaterial;
    [SerializeField] Material _highlightMaterial;
    [SerializeField] Material _selectMaterial;
    private Material _cellBaseMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    public Vector2Int GridPosition => _gridPosition;
    private Vector2Int _gridPosition;
    
    private bool _isSelected = false;

    public void InitializeCell(Vector2Int gridPosition, CellType cellType)
    {
        _gridPosition = gridPosition;
        _cellType = cellType;

        _cellBaseMaterial = cellType switch
        {
            CellType.Normal => _normalMaterial,
            CellType.Poison => _poisonMaterial,
            CellType.Water => _waterMaterial,
            _ => _normalMaterial,
        };
        _meshRenderer.material = _cellBaseMaterial;
    }

    public void SetCellType(CellType cellType)
    {
        _cellType = cellType;
    }

    public void Highlight()
    {
        if (!_isSelected)
            _meshRenderer.material = _highlightMaterial;
    }

    public void Select()
    {
        _isSelected = true;
        _meshRenderer.material = _selectMaterial;
    }

    public void ResetMaterial()
    {
        _isSelected = false;
        _meshRenderer.material = _cellBaseMaterial;
    }
}
