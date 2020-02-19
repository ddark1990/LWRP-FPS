using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
    
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if (!SceneManager.GetSceneByName("OptionsScene").isLoaded)
            SceneManager.LoadSceneAsync("OptionsScene", LoadSceneMode.Additive);
        else
            SceneManager.UnloadSceneAsync("OptionsScene");
    }
}
