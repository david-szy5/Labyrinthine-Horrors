using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class uiManager_depr : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI endGameText;
    public Text restartText;
    // [SerializeField] private GameObject otherPanel;
    private bool isGameOver = false;
    private SendToGoogle sender;

    // Start is called before the first frame update
    void Start()
    {
        //Disables panel if active
        gameOverPanel.SetActive(false);
        restartText.gameObject.SetActive(false);
        GameObject manager = GameObject.Find("GameManager");
        sender = manager.GetComponent<SendToGoogle>();
    }

    // Update is called once per frame
    void Update()
    {
        //Trigger game over manually and check with bool so it isn't called multiple times
        if (Input.GetKeyDown(KeyCode.G) && !isGameOver)
        {
            isGameOver = true;

            //StartCoroutine(GameOverSequence(END_TYPE_QUIT));
        }

        //If game is over
        if (isGameOver)
        {
            //If R is hit, restart the current scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            //If Q is hit, quit the game
            if (Input.GetKeyDown(KeyCode.Q))
            {
                print("Application Quit");
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                print("back to level select");
                SceneManager.LoadScene("LevelSelectScene");
            }
        }


    }


    //controls game over canvas and there's a brief delay between main Game Over text and option to restart/quit text
   /* public IEnumerator GameOverSequence(int cause)
    {
        if (cause == END_TYPE_WIN)
        {
            endGameText.text = "You win!";
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            endGameText.text = "Game over!";
        }
        gameOverPanel.SetActive(true);
        // otherPanel.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        restartText.gameObject.SetActive(true);


        sender.setEndType(cause); //send end type as dead 
        sender.setCheckpoint(4);
        sender.setURL("https://docs.google.com/forms/u/0/d/e/1FAIpQLSeBZx3Kqt9QJ8PbwfhE2ht4mm7G_F0LtWbipezgfGKoec503A/formResponse");
        sender.Send();
        isGameOver = true;

    }*/
}
