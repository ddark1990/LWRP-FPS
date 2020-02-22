using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score;
    public Text scoreText;


    private void OnEnable()
    {
        EventRelay.OnKilledAi += OnKilledAi;
    }

    private void OnDisable()
    {
        EventRelay.OnKilledAi -= OnKilledAi;
    }

    private void Awake()
    {
        if (!instance) instance = this;
    }

    private void Update()
    {
        scoreText.text = $"Score: {score}";
    }

    public void AddPoints(int amount)
    {
        score += amount;
    }

    //events 
    public void OnKilledAi(AiStateController ai)
    {
        PopupInfoController.Instance.AddPopUp("Killed " + ai.tag, 100);
        AddPoints(100);
    }

}
