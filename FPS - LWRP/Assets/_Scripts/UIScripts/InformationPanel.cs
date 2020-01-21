using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationPanel : MonoBehaviour
{
    public GameObject InfoPanel;

    private void OnEnable()
    {
        InfoPanel.SetActive(false);
    }

    void Update()
    {
        ToggleInformationPanel();
    }

    private void ToggleInformationPanel()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            InfoPanel.SetActive(!InfoPanel.activeSelf);
        }
    }
}
