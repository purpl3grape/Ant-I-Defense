using System.Threading.Tasks;
using UnityEngine;

public enum CameraMode
{
    FollowAnt,
    FollowAntAndFlag,
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [Header("Camera Settings")]
    [SerializeField] private CameraSettings _camSettings;


    private Transform _target;
    private Transform _flag; // Flag
    [SerializeField] private Vector3 _offset = new Vector3(0, 10, -10);
    [SerializeField] private Vector3 _offsetAntAndFlag = new Vector3(0, 2.5f, -2.5f);
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private CameraMode _cameraMode = CameraMode.FollowAntAndFlag;

    private Transform _tr;


    private async void Awake()
    {
        _tr = transform;
        _camSettings.cameraMode = CameraMode.FollowAnt;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        MainMenuManager mainMenuManager = await MainMenuManager.GetInstanceAsync();
        mainMenuManager.SetSwitchCameraModeCallback(SwitchCameraModeCallback);
    }

    public static async Task<CameraManager> GetInstanceAsync()
    {
        if (Instance == null)
        {
            while (Instance == null)
            {
                await Task.Yield();
            }
            return Instance;
        }
        else
        {
            return Instance;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetFlag(Transform flag)
    {
        _flag = flag;
    }

    private CameraMode SwitchCameraModeCallback()
    {
        if (_cameraMode == CameraMode.FollowAnt)
        {
            _cameraMode = CameraMode.FollowAntAndFlag;
        }
        else
        {
            _cameraMode = CameraMode.FollowAnt;
        }

        _camSettings.cameraMode = _cameraMode;
        return _cameraMode;
    }

    private void LateUpdate()
    {
        switch (_cameraMode)
        {
            case CameraMode.FollowAnt:
                FollowAnt();
                break;
            case CameraMode.FollowAntAndFlag:
                FollowAntAndFlag();
                break;
        }
    }

    private void FollowAnt()
    {
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _followSpeed * Time.deltaTime);
    }

    private void FollowAntAndFlag()
    {
        if (_target == null && _flag == null) return;

        var midPoint = (_target.position + _flag.position) / 2;
        Vector3 desiredPosition = midPoint + _offsetAntAndFlag;
        _tr.position = Vector3.Lerp(_tr.position, desiredPosition, _followSpeed * Time.deltaTime);
    }
}