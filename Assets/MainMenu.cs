using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject title;
    public GameObject levels;
    public AudioSource clickSound;
    
    public void SetMainMenu()
    {
        mainMenu.SetActive(true);
        title.SetActive(true); 
        levels.SetActive(false);
        clickSound.Play();
    }

    public void SetLevels()
    {
        mainMenu.SetActive(false);
        title.SetActive(false);
        levels.SetActive(true);
        clickSound.Play();
    }

    public void LoadLevel(string sceneString)
    { 
        SceneManager.LoadScene(sceneString);
    }
}
