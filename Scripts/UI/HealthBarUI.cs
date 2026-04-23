using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPre;

    public Transform barPoint;

    Image healthslider;

    Transform UIbar;

    Transform cam;

    CharactorStates curruntstate;

    private void Awake()
    {
        curruntstate = GetComponent<CharactorStates>();
        if (curruntstate != null)
            curruntstate.updatecurruntHPbar += updatehealthbar;
    }

    private void OnEnable()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
        if (healthUIPre == null)
            return;

        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            if (canvas.renderMode == RenderMode.WorldSpace)
                UIbar = Instantiate(healthUIPre, canvas.transform).transform;
        if (UIbar == null || UIbar.childCount == 0)
            return;
        healthslider = UIbar.GetChild(0).GetComponent<Image>();
        if (healthslider == null)
            return;
        UIbar.gameObject.SetActive(true);
    }

    private void updatehealthbar(int currenthealth, int maxhealth)
    {
        if (UIbar == null || healthslider == null)
            return;
        if (currenthealth <= 0)
            Destroy(UIbar.gameObject);
        else
            UIbar.gameObject.SetActive(true);
        float sliderPercent = (float)currenthealth / Mathf.Max(1, maxhealth);
        healthslider.fillAmount = Mathf.Clamp01(sliderPercent);
    }

    private void LateUpdate()
    {
        if (UIbar != null && barPoint != null && cam != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;
        }
    }
}
