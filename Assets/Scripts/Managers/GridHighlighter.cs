using UnityEngine;
using UnityEngine.InputSystem;

public class GridHighlighter : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask gridLayer;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;

    private Cell _hoveredCell = null;
    private Cell _selectedCell = null;

    private InputSystem_Actions _inputSystemActions;

    private void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Target.performed += TrySelectCell;
    }

    private void OnEnable() => _inputSystemActions.Enable();
    private void OnDisable() => _inputSystemActions.Disable();

    private void Update()
    {
        Vector2 screenPosition = _inputSystemActions.Player.PointerPosition.ReadValue<Vector2>();

        Ray ray = _cam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            Cell newCell = hit.collider.GetComponentInParent<Cell>();
            if (newCell != _hoveredCell)
            {
                if (_hoveredCell != null && _hoveredCell != _selectedCell)
                    _hoveredCell.ResetMaterial();

                if (newCell != _selectedCell)
                    newCell.Highlight();

                _hoveredCell = newCell;
            }
        }
        else if (_hoveredCell != null && _hoveredCell != _selectedCell)
        {
            _hoveredCell.ResetMaterial();
            _hoveredCell = null;
        }
    }


    private void TrySelectCell(InputAction.CallbackContext callbackContext)
    {
        if (_hoveredCell == null) return;
        if (_selectedCell != null) _selectedCell.ResetMaterial();

        _selectedCell = _hoveredCell;
        _selectedCell.Select();
    }

}
