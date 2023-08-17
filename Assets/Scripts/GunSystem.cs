using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;
//using EZCameraShake;

public class GunSystem : MonoBehaviour
{
    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot, tempBullets;
    public int numReloads;
    
    // bools
    private bool shooting, readyToShoot, reloading, aiming, fingerIsReset = true;
    
    // References
    public Camera FPSCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Rigidbody rb;
    public TMP_Text reloads_text;
    public TMP_Text bullets_text;
    public int numReloadsLeft;

    public GameObject muzzleFlash, bulletHoleGraphic, enemyHitGraphic;

    Statistics stats;
    
    // Graphics
    public float effectScaleBullet;
    public float effectScaleFlash;
    public Image reloadCircle;
    Camera playerCamera;
    float baseFOV;
    GameObject crosshair;
    Vector3 gunPosition;
    public List<Transform> finger = new List<Transform>();
    public Transform fingerPull;
    //public CameraShaker camShake;
    //ublic float roughness, fadeIn, fadeOut;
    
    // Audio
    AudioSource fire;
    AudioSource magIn;
    AudioSource magOut;
    AudioSource slideBack;
    AudioSource slideRelease;
    
    
    // Noise threshold items
    private float noiseLevel;
    public float maxNoiseLevel = 15.0f;
    public float noisePerShot = 1.0f;

    private void Start()
    {
        shooting = false;
        readyToShoot = true;
        reloading = false;
        // rb = GetComponent<Rigidbody>();
        bulletsLeft = magazineSize;

        fire = transform.Find("Fire").GetComponent<AudioSource>();
        magIn = transform.Find("MagIn").GetComponent<AudioSource>();
        magOut = transform.Find("MagOut").GetComponent<AudioSource>();
        slideRelease = transform.Find("SlideRelease").GetComponent<AudioSource>();
        slideBack = transform.Find("SlideBack").GetComponent<AudioSource>();

        playerCamera = transform.parent.GetComponent<Camera>();
        baseFOV = playerCamera.fieldOfView;
        crosshair = GameObject.Find("Canvas 2").transform.Find("Crosshair").gameObject;
        //gunPosition = transform;

        stats = GameObject.Find("GameManager").GetComponent<Statistics>();

        GameEvents.current.resetNoise += ResetNoiseLevel;

        noiseLevel = 0;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        // shooting input
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        
        // reloading
        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < magazineSize)
        {
            Reload();
        }
        
        // shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        else if (bulletsLeft == 0 && !reloading)
        {
            Reload();
        }

        // Aiming
        if (Input.GetKey(KeyCode.Mouse1))
        {
            playerCamera.fieldOfView = 55;
            crosshair.SetActive(false);
            aiming = true;
        }
        else
        {
            playerCamera.fieldOfView = baseFOV;
            crosshair.SetActive(true);

            if (aiming)
            {
                //transform.position += new Vector3(2, 0, -1);
                //transform.position = playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.nearClipPlane));
            }
            aiming = false;
        }

        // decrease noise while not shooting
        if (!shooting && !reloading && !GameEvents.current.GetEnemyChaseStatus() && noiseLevel > 0) {
            noiseLevel -= (noisePerShot*Time.deltaTime / 50);
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        float spread_mult = 1f;
        if (rb.velocity.magnitude > 0)
        {
            spread_mult = 1.5f;
        }

        float spread_ch = spread * spread_mult;
        float x = Random.Range(-spread_ch, spread_ch);
        float y = Random.Range(-spread_ch, spread_ch);

        Vector3 direction = FPSCam.transform.forward; //+ new Vector3(x, y, 0);

        if (Physics.Raycast(FPSCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            //Debug.Log(rayHit.collider.name);
            // graphics
            GameObject hitObject;
            GameObject muzzleFlashObj = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            muzzleFlashObj.transform.localScale *= effectScaleFlash;
            muzzleFlashObj.transform.parent = attackPoint.transform;
            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<BasicEnemyController>().TakeDamage(damage);
                hitObject = Instantiate(enemyHitGraphic, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
                stats.hitShot();
            }
            else
            {
                hitObject = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
            }

            hitObject.transform.localScale *= effectScaleBullet;
            hitObject.transform.parent = rayHit.transform;

            fire.Play();
            if (fingerIsReset)
            {
                foreach (var part in finger)
                    part.gameObject.SetActive(false);

                fingerPull.gameObject.SetActive(true);
                Invoke(nameof(ResetFinger), 0.1f);
                fingerIsReset = false;
            }
            
            

            bulletsLeft--;
            stats.fireShot();
            bullets_text.text = bulletsLeft.ToString();
            bulletsShot--;

            if (!IsInvoking(nameof(ResetShot)) && !readyToShoot) // !readyToShoot is just to be extra sure	
                Invoke(nameof(ResetShot), timeBetweenShooting);

            if (bulletsLeft > 0 && bulletsShot > 0)
                Invoke(nameof(Shoot), timeBetweenShots);

            if (!GameEvents.current.GetEnemyChaseStatus())
                IncreaseNoiseLevel();
        }
    }

    public void AddMagazine()
    {
        numReloadsLeft += 15;
        reloads_text.text = numReloadsLeft.ToString();
    }

    private void ResetFinger()
    {
        fingerPull.gameObject.SetActive(false);
        foreach (var part in finger)
            part.gameObject.SetActive(true);
        fingerIsReset = true;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (numReloadsLeft > 0) {
            reloading = true;
            crosshair.GetComponent<RawImage>().color = new Color32(255, 255, 255, 0);
            ReloadAnimation();
            magOut.Play();
            Invoke(nameof(ReloadFinished), reloadTime);
        }
    }

    private void ReloadFinished()
    {
        DisableReloadAnimation();
        crosshair.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
        if ((numReloadsLeft + bulletsLeft) < 15)
        {
            tempBullets = numReloadsLeft;
            numReloadsLeft = 0;
            bulletsLeft = tempBullets + bulletsLeft;
        }
        else
        {
            numReloadsLeft -= (magazineSize - bulletsLeft);
            bulletsLeft = magazineSize;
        }

        bullets_text.text = bulletsLeft.ToString();
        reloads_text.text = numReloadsLeft.ToString();
        magIn.Play();
        Invoke(nameof(playSlideRelease), magIn.clip.length);
    }

    private void ReloadAnimation()
    {
        reloadCircle.fillAmount = 1;
        reloadCircle.gameObject.SetActive(true);
        DOTween.To(() => reloadCircle.fillAmount, x => reloadCircle.fillAmount = x, 0, 1.35f);
    }

    private void DisableReloadAnimation()
    {
        reloadCircle.fillAmount = 1;
        reloadCircle.gameObject.SetActive(false);
    }

    private void playSlideRelease()
    {
        slideRelease.Play();
        reloading = false;
    }

    public float GetNoiseLevel()
    {
        return noiseLevel;
    }

    private void IncreaseNoiseLevel()
    {
        noiseLevel += noisePerShot;
        //Debug.Log("Noise level increased " + noiseLevel + " " + maxNoiseLevel);
        if (noiseLevel >= maxNoiseLevel)
        {
            //Debug.Log("over noise thresh");
            GameEvents.current.TriggerFlickerLights();
        }
    }

    public void ResetNoiseLevel()
    {
        noiseLevel = 0;
    }
    
    
}
