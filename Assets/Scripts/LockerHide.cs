using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LockerHide : MonoBehaviour
{
    float detectionRange;
    bool hasPressedKey;
    bool isClose;

    Rigidbody playerRB;
    public Transform player;
    Transform playerHolder;
    PlayerObject playerObject;
    public Collider playerCollider;
    Vector3 playerPosition;

    GameObject closedDoor;
    Transform lockerDoor;
    Transform lockerReference;
    Transform hidingSpot;
    private Transform respawnSpot;
    
    GameObject hidePrompt;
    RectTransform hidePromptTransform;
    Vector2 visiblePosition = new Vector2(0.5f, 200);
    Vector2 invisiblePosition = new Vector2(0.5f, -100);

    public Camera cam;
    Vector3 doorScreenPoint;
    bool promptInView = false;
    RectTransform lockerExit;

    public GameObject pistol;
    public GameObject hand;

    // Graphics
    private VolumeProfile postProc = null;
    private Vignette vignette = null;
    public GameObject hiddenIndicator;

    void Start()
    {
        playerHolder = player.Find("PlayerHolder");
        playerObject = playerHolder.GetComponent<PlayerObject>();
        playerRB = player.Find("PlayerHolder").GetComponent<Rigidbody>();

        if (!playerCollider)
            playerCollider = playerHolder.Find("PlayerObject").GetComponent<Collider>();

        hidePrompt = transform.Find("HidePrompt").gameObject;
        hidePromptTransform = hidePrompt.GetComponent<RectTransform>();
        hidePrompt.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        hidePrompt.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;

        hidingSpot = transform.Find("HidingSpot");
        respawnSpot = transform.Find("RespawnSpot");
        lockerDoor = transform.Find("Door");
        closedDoor = transform.Find("ClosedDoor").gameObject;
        lockerReference = transform.Find("Part6");
        lockerExit = GameObject.Find("Canvas 2").transform.Find("LockerExit").GetComponent<RectTransform>();

        if (!hiddenIndicator)
            hiddenIndicator = GameObject.Find("Canvas 2").transform.Find("Hidden").gameObject;

        detectionRange = 25f;
        hasPressedKey = false;

        if (!pistol)
            pistol = player.Find("CameraHolder").Find("PlayerCamera").Find("Pistol").gameObject;

        if (!hand)
            hand = player.Find("CameraHolder").Find("PlayerCamera").Find("Hand").gameObject;

        postProc = GameObject.Find("PostProcessing").GetComponent<Volume>().profile;
        if (postProc)
        {
            postProc.TryGet(out Vignette vig);
            vignette = vig;
        }
    }

    void Update()
    {
        doorScreenPoint = cam.WorldToViewportPoint(transform.position);
        promptInView = (Mathf.Clamp(doorScreenPoint.x, 0, 1) == doorScreenPoint.x && Mathf.Clamp(doorScreenPoint.y, 0, 1) == doorScreenPoint.y && doorScreenPoint.z > 0);
        playerPosition = playerRB.worldCenterOfMass;
        isClose = false;

        if (Vector3.Distance(playerPosition, transform.position) <= detectionRange && promptInView)
        {
            isClose = true;
        }
        if (isClose)
        {
            hidePrompt.SetActive(true);
            hidePrompt.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
            hidePrompt.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            hidePrompt.transform.LookAt(2 * transform.position - playerPosition + new Vector3(0, -5, 0));
        }
        else
        {
            hidePrompt.SetActive(false);
        }
        if (isClose && !playerObject.isPlayerHidden() && Input.GetKeyUp(KeyCode.E))
        {
            if (!hasPressedKey)
            {
                vignette.intensity.Override(0.7f);
                playerHolder.position = hidingSpot.position + new Vector3(0, 5, 0) - 1.5f * hidingSpot.transform.right;

                closedDoor.SetActive(true);
                lockerDoor.gameObject.SetActive(false);
                pistol.SetActive(false);
                hand.SetActive(false);
                hiddenIndicator.SetActive(true);

                lockerExit.gameObject.SetActive(true);
                playerObject.setPlayerConcealment(true);
                cam.transform.LookAt(respawnSpot.transform);
            }
        }
        else if (playerObject.isPlayerHidden() && Input.GetKeyUp(KeyCode.E) && (Vector3.Distance(playerPosition, transform.position) <= detectionRange))
        {
            if (!hasPressedKey)
            {
                vignette.intensity.Override(0.475f);
                playerHolder.position = respawnSpot.position + new Vector3(0, 2, 0);

                closedDoor.SetActive(false);
                lockerDoor.gameObject.SetActive(true);
                pistol.SetActive(true);
                hand.SetActive(true);
                hiddenIndicator.SetActive(false);

                lockerExit.gameObject.SetActive(false);
                playerObject.setPlayerConcealment(false);
            }
        }
    }
}
