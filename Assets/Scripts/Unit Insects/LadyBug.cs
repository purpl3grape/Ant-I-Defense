using UnityEngine;

public class LadyBug : Unit
{
    [SerializeField] private LadyBugSettings _ladyBugSettings;

    private new void Awake()
    {
        base.Awake();
        SetMoveSpeed(_ladyBugSettings.moveSpeed);
        SetMoveThreshold(_ladyBugSettings.moveThreshold);
        SetRotationSpeed(_ladyBugSettings.rotationSpeed);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}