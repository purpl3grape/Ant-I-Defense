using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Game Settings")]
    [SerializeField] private GameSettings _gameSettings;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _gameLoopMixerGroup;
    [SerializeField] private AudioMixerGroup _gameFXMixerGroup;

    [Header("Main Menu UI Elements")]
    [SerializeField] private Button _playerControlToggle;
    [SerializeField] private Button _musicToggle;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _camModeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _mainMenuPanel;

    [SerializeField] private TMP_Text _playerToggleText;
    [SerializeField] private TMP_Text _musicToggleText;
    [SerializeField] private TMP_Text _startButtonText;
    [SerializeField] private TMP_Text _camModeButtonText;

    // we shall do the scoring here for now. It should be moved to its own class to define the scoring structure.
    // but since we are only displaying: simple points scoring #units destroyed, and #units remaining I'll add those in here.
    [Header("Game Over UI Elements")]
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _enemiesRemainingText;
    [SerializeField] private TMP_Text _endGameText;
    [SerializeField] private Button _returnToMainMenuButton;

    [Header("Container Elements")]
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private GameObject _enemiesRemaining;

    public bool GameOver;

    private Func<CameraMode> _onSwitchCameraMode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _gameSettings.isGameStarted = false;
        _gameSettings.isPlayerControlled = true;
        _gameSettings.isMusicOn = true;
        _gameSettings.isPaused = false;

        CreateInstance();
        _playerControlToggle.onClick.AddListener(OnTapPlayerControlToggle);
        _musicToggle.onClick.AddListener(OnTapMusicToggle);
        _startButton.onClick.AddListener(OnTapStartOrContinueGame);
        _exitButton.onClick.AddListener(OnTapExitGame);
        _camModeButton.onClick.AddListener(OnTapCameraMode);
        _returnToMainMenuButton.onClick.AddListener(OnTapReturnToMainMenu);

        _enemiesRemaining.SetActive(false);
        _scoreBoard.SetActive(false);

        _playerToggleText.text = _gameSettings.isPlayerControlled ? "Player Control: Player Input" : "Player Control: AI";
        _musicToggleText.text = _gameSettings.isMusicOn ? "Music: ON" : "Music: OFF";
        _startButtonText.text = _gameSettings.isGameStarted ? "Continue Game" : "Start Game";
        _camModeButtonText.text = "Camera Mode: Follow Ant"; // default camera mode
        _enemiesRemainingText.text = "Enemies Remaining: 0"; // default enemies remaining
        _scoreText.text = "Score: 0"; // default score
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static async Task<MainMenuManager> GetInstanceAsync()
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

    // score a.k.a. number of enemies eliminated.
    public void SetScore(int score) => _scoreText.text = $"Score: {score}";

    public void SetEnemiesRemaining(int enemiesRemaining) => _enemiesRemainingText.text = $"Enemies Remaining: {enemiesRemaining}";

    public void SetSwitchCameraModeCallback(Func<CameraMode> cb)
    {
        _onSwitchCameraMode = cb;
    }

    public void EndGame(bool win)
    {
        _endGamePanel.SetActive(true);
        _endGameText.text = win ? $"YOU WIN!" : $"YOU LOSE!";
        _endGameText.color = win ? Color.green : Color.red;

        _gameSettings.isPaused = true;
        GameOver = true;

        _mainMenuPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        _enemiesRemaining.SetActive(false);
        _scoreBoard.SetActive(false);

        _mainMenuPanel.SetActive(true);
        _gameSettings.isPaused = true;
        _startButtonText.text = "Continue Game";
    }

    private void OnTapPlayerControlToggle()
    {
        _gameSettings.isPlayerControlled = !_gameSettings.isPlayerControlled;
        _playerToggleText.text = _gameSettings.isPlayerControlled ? "Player Control: Player Input" : "Player Control: AI";
    }

    private void OnTapMusicToggle()
    {
        _gameSettings.isMusicOn = !_gameSettings.isMusicOn;
        _musicToggleText.text = _gameSettings.isMusicOn ? "Music: ON" : "Music: OFF";
        _gameFXMixerGroup.audioMixer.SetFloat("GameLoopVolume", _gameSettings.isMusicOn ? -6f : -80f);
        _gameFXMixerGroup.audioMixer.SetFloat("GameFXVolume", _gameSettings.isMusicOn ? 0f : -80f);
    }

    private void OnTapStartOrContinueGame()
    {
        if (!_gameSettings.isGameStarted)
        {
            // Set the game state to started
            _gameSettings.isGameStarted = true;
            _startButtonText.text = "Start Game";
        }
        else
        {
            _startButtonText.text = "Continue Game";
        }

        _gameSettings.isPaused = false;
        _mainMenuPanel.SetActive(false);

        _enemiesRemaining.SetActive(true);
        _scoreBoard.SetActive(true);
    }

    private void OnTapExitGame()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        Application.Quit();
#elif UNITY_EDITOR
        ResetGameSettings();
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnTapCameraMode()
    {
        CameraMode camMode = _onSwitchCameraMode?.Invoke() ?? CameraMode.FollowAnt;

        switch (camMode)
        {
            case CameraMode.FollowAnt:
                _camModeButtonText.text = "Camera Mode: Follow Ant";
                break;
            case CameraMode.FollowAntAndFlag:
                _camModeButtonText.text = "Camera Mode: Follow Ant and Flag";
                break;
        }
    }

    private void OnTapReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void ResetGameSettings()
    {
        _gameSettings.isGameStarted = false;
        _gameSettings.isPaused = false;
        _gameSettings.isPlayerControlled = true;
        _gameSettings.isMusicOn = true;
    }
}