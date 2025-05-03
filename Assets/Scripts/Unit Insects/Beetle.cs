using UnityEngine;

public class Beetle : Unit
{
    [SerializeField] private BeetleSettings _beetleSettings;

    private new void Awake()
    {
        base.Awake();
        SetMoveSpeed(_beetleSettings.moveSpeed);
        SetMoveThreshold(_beetleSettings.moveThreshold);
        SetRotationSpeed(_beetleSettings.rotationSpeed);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}