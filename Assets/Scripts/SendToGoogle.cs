using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using static Constants;

public class SendToGoogle : MonoBehaviour
{
    // parameters
    public string URL;
    public int checkPoint;
    private float playerHealth;
    private float enemyHealth;
    public int endType;
    public int level;

    public long sessionID;
    private string playType;
    private long startTime;
    private long endTime;
    private double duration;


    public PlayerHealth player;
    Statistics stats;


    
    private void Awake()
    {
        // Assign sessionID to identify playtests
        sessionID = System.DateTime.Now.Ticks;
        startTime = System.DateTime.Now.Ticks;

        player = GameObject.Find("PlayerHolder").GetComponent<PlayerHealth>();
        stats = GameObject.Find("GameManager").GetComponent<Statistics>();
        playerHealth = player.playerHealth;


        Send();

        Debug.Log(getSessionID());

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public long getSessionID() {

        return sessionID;
    }

    public void setEndType(int _endType) {

        endType = _endType;
    }

    public void setCheckpoint(int _checkpoint) {

        checkPoint = _checkpoint;
    }

    public void setURL(string _url) {

        URL = _url;
    }

    public void Send()
    {
        playType = PLAY_TYPE;
        level = 1;
        // Assign variables
        switch (checkPoint)
        {
        case 1:
            playType = PLAY_TYPE;
            level = 1;
            playerHealth = 100;
            enemyHealth = 90;
            URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSd79UuAamE_nAmyktP6dH_YDp1TR2_cy2jyoep2v4MgTHAa6w/formResponse";
            break;
        case 2:
            break;
        case 3:
            break;
        case 4:
            playerHealth = player.playerHealth;

            endTime = System.DateTime.Now.Ticks;
            duration = Time.realtimeSinceStartup;
            URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeBZx3Kqt9QJ8PbwfhE2ht4mm7G_F0LtWbipezgfGKoec503A/formResponse";
            
            break;
        default:
            break;
        }
        StartCoroutine(Post(sessionID.ToString(),
        playType.ToString(), level.ToString(), startTime.ToString(), playerHealth.ToString(), enemyHealth.ToString(), endType.ToString(), duration.ToString(), endTime.ToString(), (stats.shotsFired).ToString(), (stats.shotsHit).ToString(), stats.numPickups.ToString(), stats.timeToKey.ToString(), stats.gateCollisions.ToString() ));
    }

    private IEnumerator Post(string _sessionID, string _playType, string _level, string _startTime, string _playerHealth, string _enemyHealth, string _endType, string _duration, string _endTime, string _shotsFired, string _shotsHit, string _numPickups, string _timeToKey, string _gateCollisions)
    {

        // "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeZBAO1rts-cj4Flm4humfusWbJQwFT2NheSeWHawRcga0_Pw/formResponse"
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        switch (checkPoint)
        {
        case 1:
            form.AddField("entry.1979416340", _sessionID);
            form.AddField("entry.1524091363", _playType);
            form.AddField("entry.1812210451", _level);
            form.AddField("entry.39221133", _startTime);
            break;
        case 2:
            form.AddField("entry.1979416340", _sessionID);
            form.AddField("entry.39221133", _playerHealth);
            break;
        case 3:
            form.AddField("entry.1979416340", _sessionID);
            form.AddField("entry.39221133", _enemyHealth);
            break;
        case 4:
            form.AddField("entry.1979416340", _sessionID);
            form.AddField("entry.1524091363", _endType);
            form.AddField("entry.1812210451", _duration);
            form.AddField("entry.39221133", _endTime);
            form.AddField("entry.727038464", _playerHealth);
            form.AddField("entry.1279140490", _enemyHealth);
            form.AddField("entry.1952312401", _shotsFired);
            form.AddField("entry.1925229536", _shotsHit);
            form.AddField("entry.2090936749", _numPickups);
            form.AddField("entry.549266650", _timeToKey);
            form.AddField("entry.1709908324", _gateCollisions);
            break;
        default:
            break;
        }


        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(getSessionID());
            }
        }


    }

}
