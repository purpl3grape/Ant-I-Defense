using System.Collections.Generic;
using UnityEngine;

public class Ant : Unit
{
    [SerializeField] private AntSettings _antSettings;
    public int searchRange => _searchRange;
    [SerializeField] private int _searchRange = 3;

    public bool useRandomMovement;
    private CameraManager _cameraManager;

    private new async void Awake()
    {
        base.Awake();
        SetMoveSpeed(_antSettings.moveSpeed);
        SetMoveThreshold(_antSettings.moveThreshold);
        SetRotationSpeed(_antSettings.rotationSpeed);

        _cameraManager = await CameraManager.GetInstanceAsync();
        _cameraManager.SetTarget(transform);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if(GameSettings.isPaused || !GameSettings.isGameStarted)
            return;

        if (GridRef == null) return;

        if (!IsMoving)
        {
            // We are using a temporary list to avoid modifying the collection while iterating
            var unitsForGivenCell = new List<IUnit>(GridRef.GetUnitsAt(TargetWorldPosition.x, TargetWorldPosition.y));

            foreach (var unit in unitsForGivenCell)
            {
                if (unit.IsEnemy && unit.UnitState == UnitState.Alive)
                {
                    unit.Die();
                }
            }
        }
    }
}
