using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using static Constants;

public class uiManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject controlsText;
    public TMP_Text endGameText;
    public TMP_Text optionsText;

    private bool isGameOver = false;
    private bool isGamePaused = false;
    private SendToGoogle sender;

    // Start is called before the first frame update
    void Start()
    {
        _pauseMenu.SetActive(false);

        GameObject manager = GameObject.Find("GameManager");
        sender = manager.GetComponent<SendToGoogle>();
    }

    void Update () {

		//uses the p button to pause and unpause the game
		if(Input.GetKeyDown(KeyCode.P))
		{
			if(!isGamePaused)
			{
				PauseButton();
			} else {
				ResumeButton();
			}
		}

        //If paused
        if (isGamePaused || isGameOver)
        {
            //If R is hit, restart the current scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
            //If Q is hit, quit the game
            if (Input.GetKeyDown(KeyCode.Q))
            {
                print("Application Quit");
                if (isGamePaused) {
                    StartCoroutine(GameOverSequence(END_TYPE_QUIT));
                }
                QuitGame();
            }
        }
	}

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseButton()
    {
        Debug.Log("Pause pressed");
        isGamePaused = true;
        GameEvents.current.TriggerPause();
        controlsText.SetActive(true);
        _pauseMenu.SetActive(true);
    }

    public void ResumeButton()
    {
        GameEvents.current.TriggerUnpause();
        isGamePaused = false;
        _pauseMenu.SetActive(false);
    }

    //controls game over canvas and there's a brief delay between main Game Over text and option to restart/quit text
    public IEnumerator GameOverSequence(int cause)
    {
        Debug.Log("in sequence" + cause);
        controlsText.SetActive(false);
        if(cause != END_TYPE_QUIT) {
            if (cause == END_TYPE_WIN) {
                endGameText.text = "You win!";
                GameEvents.current.TriggerPause();
            } else {
                Debug.Log("Game Over!");
                endGameText.text = "Game over!";
                Time.timeScale = 0f; // funny float off in the distance
            }
            
            optionsText.text = "R = Restart | Q = Quit";
            _pauseMenu.SetActive(true);
        }
        // otherPanel.SetActive(false);
        


        sender.setEndType(cause); //send end type as dead 
        sender.setCheckpoint(4);
        sender.setURL("https://docs.google.com/forms/u/0/d/e/1FAIpQLSeBZx3Kqt9QJ8PbwfhE2ht4mm7G_F0LtWbipezgfGKoec503A/formResponse");
        sender.Send();
        isGameOver = true;
         yield return null;
    }

}
