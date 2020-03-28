using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        if (!Instance) Instance = this;
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
    public void OnKilledAi(AiController ai)
    {
        AddPoints(100);
        PopupInfoController.Instance.AddPopUp("Killed " + ai.tag, 100);
    }

}
