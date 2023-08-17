using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EscapeDoor : MonoBehaviour
{
    float distance;
    Vector3 doorScreenPoint;
    bool isOpened = false;

    Camera cam;
    PlayerObject player;

    Transform lockedDoorPrompt;
    Transform openDoorPrompt;

    void Start()
    {
        player = GameObject.Find("PlayerFPS 1").transform.Find("PlayerHolder").GetComponent<PlayerObject>();
        cam = player.transform.parent.Find("CameraHolder").Find("PlayerCamera").GetComponent<Camera>();

        lockedDoorPrompt = transform.Find("LockedDoorPrompt");
        lockedDoorPrompt.gameObject.SetActive(false);
        openDoorPrompt = transform.Find("OpenDoorPrompt");
        openDoorPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 20 && !isOpened)
        {
            if (player.hasKey())
            {
                openDoorPrompt.gameObject.SetActive(true);
                doorScreenPoint = cam.WorldToViewportPoint(transform.position);

                // Check whether the door is in the player's view before allowing them to open it
                if (Mathf.Clamp(doorScreenPoint.x, 0, 1) == doorScreenPoint.x && Mathf.Clamp(doorScreenPoint.y, 0, 1) == doorScreenPoint.y && doorScreenPoint.z > 0)
                {
                    if (Input.GetKeyUp(KeyCode.E) && !isOpened)
                    {
                        transform.DOMove(transform.position - new Vector3(0, 0, 10), 1);
                        isOpened = true;
                    }
                }
            }
            else
            {
                lockedDoorPrompt.gameObject.SetActive(true);
            }
        }
        else
        {
            lockedDoorPrompt.gameObject.SetActive(false);
            openDoorPrompt.gameObject.SetActive(false);
        }
    }
}
