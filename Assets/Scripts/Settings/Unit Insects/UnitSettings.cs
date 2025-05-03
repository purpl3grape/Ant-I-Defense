using UnityEngine;

[CreateAssetMenu(fileName = "UnitSettings", menuName = "Unit/AphidSettings")]
public class UnitSettings : ScriptableObject
{
    public float moveSpeed = 0.2f;
    public float moveThreshold = 0.1f;
    public float rotationSpeed = 0.1f;
}