using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
public GameObject menuCanvas; 
public GameObject controlsCanvas; 

    // Start is called before the first frame update
    void Start()
    {
        menuCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
    }

    public void OpenTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenMazeScene()
    {
        SceneManager.LoadScene("Maze");
    }

    public void OpenEasyScene()
    {
        SceneManager.LoadScene("MazeEasy");
    }

    public void OpenHardScene()
    {
        SceneManager.LoadScene("CameronLevel");
    }

    public void OpenControls()
    {
        menuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
    }

    public void BackToLevelSelect()
    {
        menuCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
    }
}
