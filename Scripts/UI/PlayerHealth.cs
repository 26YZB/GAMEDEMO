using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Text levelText;
    Image healthslider;
    Image expslider;
    CharactorStates charactorStates;

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            charactorStates = player.GetComponent<CharactorStates>();

        if (transform.childCount > 2)
            levelText = transform.GetChild(2).GetComponent<Text>();
        if (transform.childCount > 0 && transform.GetChild(0).childCount > 0)
            healthslider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (transform.childCount > 1 && transform.GetChild(1).childCount > 0)
            expslider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        if (charactorStates == null || charactorStates.charactorData == null)
            return;

        if (levelText != null)
            levelText.text = "Level " + charactorStates.charactorData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth()
    {
        if (healthslider == null)
            return;
        float max = Mathf.Max(1, charactorStates.MaxHealth);
        float sliderPercent = (float)charactorStates.CurruntHealth / max;
        healthslider.fillAmount = Mathf.Clamp01(sliderPercent);
    }

    void UpdateExp()
    {
        if (expslider == null)
            return;
        float need = Mathf.Max(1, charactorStates.charactorData.baseExp);
        float sliderPercent = (float)charactorStates.charactorData.currentExp / need;
        expslider.fillAmount = Mathf.Clamp01(sliderPercent);
    }
}
