using UnityEngine;
using UnityEngine.UI;

public class PauseC : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseContainer; // Pause_Container
    [SerializeField] private GameObject greyFilter; // Grey_Filter
    [SerializeField] private GameObject sceneContent; // Scene_Content

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button quitButton;

    private bool isPaused;

    void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        SetPauseVisible(false);
        SetSettingsVisible(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        Time.timeScale = 0f;
        SetPauseVisible(true);
        SetSettingsVisible(false);
    }

    public void ContinueGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetPauseVisible(false);
        SetSettingsVisible(false);
    }

    public void OpenSettings()
    {
        if (!isPaused)
            PauseGame();

        SetPauseVisible(false);
        SetSettingsVisible(true);
    }

    public void CloseSettings()
    {
        if (!isPaused)
            PauseGame();

        SetSettingsVisible(false);
        SetPauseVisible(true);
    }

    public void QuitGame()
    {
        Application.Quit();

    }

    void SetPauseVisible(bool visible)
    {
        if (pauseContainer != null)
            pauseContainer.SetActive(visible);
        if (greyFilter != null)
            greyFilter.SetActive(visible || (sceneContent != null && sceneContent.activeSelf));
    }

    void SetSettingsVisible(bool visible)
    {
        if (sceneContent != null)
            sceneContent.SetActive(visible);
        if (greyFilter != null)
            greyFilter.SetActive(visible || (pauseContainer != null && pauseContainer.activeSelf));
    }

    void OnDisable()
    {
        if (isPaused)
            Time.timeScale = 1f;
    }
}
