using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsPanel;

    private void Start()
    {
        Time.timeScale = 1f;

        InputManager.Instance.OnPauseStarted += GameInput_OnPauseStarted;
    }

    public void OnSettingsButton() => _settingsPanel.SetActive(true);
    public void OnMainMenuButton() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

    private void GameInput_OnPauseStarted()
    {
        _pausePanel.SetActive(!_pausePanel.activeSelf);
        Time.timeScale = _pausePanel.activeSelf ? 0f : 1f;

        SoundManager.Instance.SetPauseState(_pausePanel.activeSelf);
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPauseStarted -= GameInput_OnPauseStarted;
    }
}
