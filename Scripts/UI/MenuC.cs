using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuC : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Main UI")]
    [SerializeField] private GameObject title;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject grey_filter;
    [SerializeField] private GameObject scene_content;
    [SerializeField] private Button closeButton;

    const string PrefKeyPrefix = "menu.settings.";

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
        if (playButton != null)
            playButton.onClick.AddListener(Play);
        if (settingButton != null)
            settingButton.onClick.AddListener(OpenSettings);
        if (exitButton != null)
            exitButton.onClick.AddListener(Exit);
    }

    void Start()
    {
        SetSettingsVisible(false);
        SetMainInteractable(true);
        ApplySavedSettingsToUI();
    }

    public void Play()
    {
        if (!string.IsNullOrWhiteSpace(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        SetSettingsVisible(true);
        SetMainInteractable(false);
        ApplySavedSettingsToUI();
    }

    public void CloseSettings()
    {
        SaveSettingsFromUI();
        SetSettingsVisible(false);
        SetMainInteractable(true);
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetSettingsVisible(bool visible)
    {
        if (grey_filter != null)
            grey_filter.SetActive(visible);
        if (scene_content != null)
            scene_content.SetActive(visible);
    }

    void SetMainInteractable(bool interactable)
    {
        if (title != null)
            title.SetActive(interactable);
        if (playButton != null)
            playButton.interactable = interactable;
        if (settingButton != null)
            settingButton.interactable = interactable;
        if (exitButton != null)
            exitButton.interactable = interactable;
    }

    void ApplySavedSettingsToUI()
    {
        if (scene_content == null)
            return;

        foreach (var s in scene_content.GetComponentsInChildren<Slider>(true))
        {
            string k = PrefKeyPrefix + "slider." + s.gameObject.name;
            if (PlayerPrefs.HasKey(k))
                s.value = PlayerPrefs.GetFloat(k);
        }

        foreach (var t in scene_content.GetComponentsInChildren<Toggle>(true))
        {
            string k = PrefKeyPrefix + "toggle." + t.gameObject.name;
            if (PlayerPrefs.HasKey(k))
                t.isOn = PlayerPrefs.GetInt(k) != 0;
        }

        foreach (var d in scene_content.GetComponentsInChildren<Dropdown>(true))
        {
            string k = PrefKeyPrefix + "dropdown." + d.gameObject.name;
            if (PlayerPrefs.HasKey(k))
                d.value = PlayerPrefs.GetInt(k);
        }
    }

    void SaveSettingsFromUI()
    {
        if (scene_content == null)
            return;

        foreach (var s in scene_content.GetComponentsInChildren<Slider>(true))
            PlayerPrefs.SetFloat(PrefKeyPrefix + "slider." + s.gameObject.name, s.value);

        foreach (var t in scene_content.GetComponentsInChildren<Toggle>(true))
            PlayerPrefs.SetInt(PrefKeyPrefix + "toggle." + t.gameObject.name, t.isOn ? 1 : 0);

        foreach (var d in scene_content.GetComponentsInChildren<Dropdown>(true))
            PlayerPrefs.SetInt(PrefKeyPrefix + "dropdown." + d.gameObject.name, d.value);

        PlayerPrefs.Save();
    }
}
