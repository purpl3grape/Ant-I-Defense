using UnityEngine;

public class Aphid : Unit
{
    [SerializeField] private AphidSettings _aphidSettings;

    private new void Awake()
    {
        base.Awake();
        SetMoveSpeed(_aphidSettings.moveSpeed);
        SetMoveThreshold(_aphidSettings.moveThreshold);
        SetRotationSpeed(_aphidSettings.rotationSpeed);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
