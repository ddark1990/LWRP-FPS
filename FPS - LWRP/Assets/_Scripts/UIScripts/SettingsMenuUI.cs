using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    public GameObject BackPanel;

    public Slider MouseSensSlider;
    public Slider UISoundEffectsSlider;
    public Slider MouseSmoothingSlider;

    private CanvasGroup backPanelCanvasGroup;
    private GoomerFPSController.FPSController fpsController;

    private void Start()
    {
        BackPanel.SetActive(false);
        backPanelCanvasGroup = BackPanel.GetComponent<CanvasGroup>();
        fpsController = FindObjectOfType<GoomerFPSController.FPSController>();
    }

    private void Update()
    {
        fpsController.MouseSensetivity = MouseSensSlider.value; //mouse settings
        //fpsController.MouseSmooth = MouseSmoothingSlider.value;

        InventoryManager.Instance.GetUIVolumeNormalized((int)UISoundEffectsSlider.value); //UI sound efffect volume

        if(Input.GetKeyDown(KeyCode.Escape) && BackPanel.activeSelf)
        {
            StartCoroutine(CloseBackPanel(.4f));
        }
    }

    public void OnOpenSettingsPress()
    {
        StartCoroutine(OpenBackPanel(.4f));
    }
    public void OnCloseSettingsPress()
    {
        StartCoroutine(CloseBackPanel(.4f));
    }

    private IEnumerator OpenBackPanel(float time)
    {
        var elapsedTime = 0f;

        BackPanel.SetActive(true);

        while(elapsedTime < time)
        {
            backPanelCanvasGroup.alpha = Mathf.Lerp(backPanelCanvasGroup.alpha, 1, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator CloseBackPanel(float time)
    {
        var elapsedTime = 0f;

        while (elapsedTime < time)
        {
            backPanelCanvasGroup.alpha = Mathf.Lerp(backPanelCanvasGroup.alpha, 0, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BackPanel.SetActive(false);
    }
}
