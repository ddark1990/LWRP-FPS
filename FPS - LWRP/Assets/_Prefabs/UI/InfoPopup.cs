using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    public float fadeStartTimer;

    public Text itemNameText;
    public Text itemAmountText;
    
    private float _timer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void InitializePopup(float fadeTimer, string info)
    {
        _timer = fadeTimer;
        itemNameText.text = info;
        itemAmountText.text = string.Empty;
    }
    public void InitializePopup(float fadeTimer, string itemName, int itemAmount)
    {
        _timer = fadeTimer;
        itemNameText.text = itemName;
        itemAmountText.text = "+" + itemAmount.ToString();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _animator.Play("PopOutInfo"); //removes self at the end of animation using animation event
        }
    }

    public void RemoveSelf()
    {
        Destroy(gameObject);
    }
}
