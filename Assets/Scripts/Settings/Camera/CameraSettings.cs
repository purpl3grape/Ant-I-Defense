using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Camera/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    public CameraMode cameraMode = CameraMode.FollowAnt;
}