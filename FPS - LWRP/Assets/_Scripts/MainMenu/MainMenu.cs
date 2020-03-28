using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartGamePress()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
    public void OnOptionsPress()
    {
        //open options window
        //graphics/sound
    }
    
    public void OnQuitPress()
    {
        Application.Quit();
    }
    
    private void ResetWindow()
    {
        
    }
}
