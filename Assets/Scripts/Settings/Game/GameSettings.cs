using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public bool isGameStarted = false;
    public bool isPlayerControlled = true;
    public bool isMusicOn = true;
    public bool isPaused = false;
}