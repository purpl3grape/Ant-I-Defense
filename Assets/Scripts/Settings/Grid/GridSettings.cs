using UnityEngine;

[CreateAssetMenu(fileName = "GridSettings", menuName = "Grid/GridSettings")]
public class GridSettings : ScriptableObject
{
    public int gridSize = 25;
    [Range(0, 100)] public int probabilityPoisonSpawn = 10;
}
