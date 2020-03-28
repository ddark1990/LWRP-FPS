using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private WaitForSeconds _waitForSecond;
    
    private void Start()
    {
        _waitForSecond = new WaitForSeconds(2); //buffer time so you can somewhat see the progress bar 
        
        StartCoroutine(LoadAsyncOperation(2));
    }

    private IEnumerator LoadAsyncOperation(int sceneIndex)
    {
        yield return _waitForSecond;

        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneIndex);

        while (!gameLevel.isDone)
        {
            var progress = Mathf.Clamp01(gameLevel.progress / .9f);
            progressBar.fillAmount = progress;
            progressText.text = (progress * 100f).ToString("#") + "%";
            
            yield return null;
        }
        
    }
}
