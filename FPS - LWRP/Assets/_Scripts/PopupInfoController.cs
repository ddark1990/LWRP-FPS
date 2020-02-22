using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfoController : MonoBehaviour
{
    public static PopupInfoController Instance;
    
    public GameObject popup;
    public GameObject popupHolder;

    public float fadeTimer = 3;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void AddPopUp(string info)
    {
        var tempPopup = Instantiate(popup, popupHolder.transform);
        var infoPopup = tempPopup.GetComponent<InfoPopup>();
        
        infoPopup.InitializePopup(fadeTimer, info);
    }
    public void AddPopUp(string popupName, int popupAmount)
    {
        var tempPopup = Instantiate(popup, popupHolder.transform);
        var infoPopup = tempPopup.GetComponent<InfoPopup>();
        
        infoPopup.InitializePopup(fadeTimer, popupName, popupAmount);
    }
}
