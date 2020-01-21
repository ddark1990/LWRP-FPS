using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputKeyManager InputKeyManager;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
