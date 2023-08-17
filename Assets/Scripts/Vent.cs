using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Vent : MonoBehaviour
{
    RectTransform openVentPrompt;
    RectTransform closeVentPrompt;

    Camera cam;
    public PlayerObject player;
    Collider ventCollider;

    float distance;
    bool isOpened;

    Vector3 ventScreenPoint;
    Vector2 visiblePosition;
    Vector2 invisiblePosition;
    Vector3 closedVentPosition;
    Vector3 openedVentPosition;

    public PlayerObject playerObject;

    // Graphics
    private VolumeProfile postProc = null;
    private Vignette vignette = null;
    public GameObject hiddenIndicator;

    void Start()
    {
        cam = player.transform.parent.Find("CameraHolder").Find("PlayerCamera").GetComponent<Camera>();
        closeVentPrompt = GameObject.Find("Canvas 2").transform.Find("CloseVent").GetComponent<RectTransform>();
        openVentPrompt = GameObject.Find("Canvas 2").transform.Find("OpenVent").GetComponent<RectTransform>();

        closedVentPosition = transform.position;
        openedVentPosition = transform.position + 7f * transform.up;
        openVentPrompt.GetComponent<RectTransform>().anchoredPosition = invisiblePosition;
        closeVentPrompt.GetComponent<RectTransform>().anchoredPosition = invisiblePosition;

        ventCollider = transform.GetComponent<Collider>();

        if (!playerObject && GameObject.Find("PlayerFPS 1"))
            playerObject = GameObject.Find("PlayerFPS 1").transform.Find("PlayerHolder").GetComponent<PlayerObject>();  

        if (!playerObject && GameObject.Find("PlayerFPS"))
            playerObject = GameObject.Find("PlayerFPS").transform.Find("PlayerHolder").GetComponent<PlayerObject>();

        postProc = GameObject.Find("PostProcessing").GetComponent<Volume>().profile;
        if (postProc)
        {
            postProc.TryGet(out Vignette vig);
            vignette = vig;
        }

        if (!hiddenIndicator)
            hiddenIndicator = GameObject.Find("Canvas 2").transform.Find("Hidden").gameObject;
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 15)
        {
            if (!playerObject.getListOfVents().Contains(gameObject))
            {
                playerObject.setNearVent(gameObject);
            }

            if (!isOpened)
            {
                openVentPrompt.gameObject.SetActive(true);
                ventScreenPoint = cam.WorldToViewportPoint(transform.position);

                // Check whether the vent is in the player's view before allowing them to open it
                if (Mathf.Clamp(ventScreenPoint.x, -1f, 2f) == ventScreenPoint.x && Mathf.Clamp(ventScreenPoint.y, -1f, 2f) == ventScreenPoint.y && ventScreenPoint.z > 0)
                {
                    if (Input.GetKeyUp(KeyCode.E) && !isOpened)
                    {
                        transform.DOMove(openedVentPosition, 0.5f);
                        openVentPrompt.gameObject.SetActive(false);
                        isOpened = true;
                    }
                }
            }
            else
            {
                closeVentPrompt.gameObject.SetActive(true);
                if (Input.GetKeyUp(KeyCode.E) && isOpened)
                {
                    transform.DOMove(closedVentPosition, 0.5f);
                    closeVentPrompt.gameObject.SetActive(false);
                    isOpened = false;
                }
            }
        }
        else
        {
            if (playerObject.getListOfVents().Contains(gameObject))
            {
                playerObject.removeNearVent(gameObject);
            }

            if (!playerObject.isPlayerNearVent())
            {
                closeVentPrompt.gameObject.SetActive(false);
                openVentPrompt.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        player.setPlayerConcealment(true);
        vignette.intensity.Override(0.7f);
        hiddenIndicator.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        player.setPlayerConcealment(false);
        vignette.intensity.Override(0.475f);
        hiddenIndicator.SetActive(false);
    }
}
