using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovementFPS : MonoBehaviour
{
    [Header("Movement")] 
    public float walkSpeed;
    public float sprintSpeed;
    private float movementSpeed;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
   // private bool readyToJump = true;

    // Camera bob
    public Transform _cam = null;
    public Transform _camHolder = null;
    public Transform _hand = null;
    public Transform _pistol = null;

    [SerializeField] private bool _enable = true;
    [SerializeField, Range(0, 0.01f)] private float _amplitude = 0.0035f;
    [SerializeField, Range(0, 100)] private float _frequency = 15.0f;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private Vector3 _startPosHand;
    private Vector3 _startPosPistol;

    // Footstep sounds
    private List<AudioSource> footstepWalk;
    private List<AudioSource> footstepRun;

    private FootstepWalk footstepWalkAudio;
    private FootstepRun footstepRunAudio;

    private bool isAudioPlaying = false;

    [Header("Crouching")] 
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybindings")] 
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("SlopeHandling")] 
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope = false;
    public bool wasCrouching = false;
    

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    };
    
    [Header("GroundCheck")] 
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 movementDirection;
    private Rigidbody rb;
    private PlayerObject playerObject;

    private bool paused;

    private void Awake()
    {
        _startPos = _cam.localPosition;
        _startPosHand = _hand.localPosition;
        _startPosPistol = _pistol.localPosition;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
        playerHeight = transform.parent.transform.localScale.y;
        playerObject = transform.GetComponent<PlayerObject>();
        paused = false;
        GameEvents.current.pauseTriggered += Paused;
        GameEvents.current.unpauseTriggered += Unpause;

        if (!_cam)
            _cam = playerObject.transform.parent.Find("CameraHolder").Find("PlayerCamera");

        if (!_hand)
            _hand = transform.parent.Find("CameraHolder").Find("PlayerCamera").Find("Hand");

        if (!_pistol)
            _pistol = transform.parent.Find("CameraHolder").Find("PlayerCamera").Find("Pistol");

        if (!footstepWalkAudio)
            footstepWalkAudio = GameObject.Find("FootstepWalk").GetComponent<FootstepWalk>();

        if (!footstepRunAudio)
            footstepRunAudio = GameObject.Find("FootstepRun").GetComponent<FootstepRun>();

        footstepWalk = footstepWalkAudio.getWalkAudio();
        footstepRun = footstepRunAudio.getRunAudio();
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        Vector3 posHand = Vector3.zero;

        if (state == MovementState.walking)
        {
            pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
            pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude;

            posHand.y += Mathf.Sin(Time.time * _frequency) * _amplitude / 8;
            posHand.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude / 15;
        }
        else if (state == MovementState.sprinting)
        {
            float freq = _frequency * 1.6f;
            float freqHand = _frequency * 1.2f;

            pos.y += Mathf.Sin(Time.time * freq) * _amplitude;
            pos.x += Mathf.Cos(Time.time * freq / 2) * _amplitude;

            posHand.y += Mathf.Sin(Time.time * freqHand) * _amplitude / 3;
            posHand.x += Mathf.Cos(Time.time * freqHand / 2) * _amplitude / 8;
        }

        if (!isAudioPlaying)
        {
            AudioSource footstep = null;
            int randomIndex = UnityEngine.Random.Range(0, Mathf.Min(footstepWalk.Count, footstepRun.Count));
            float period = 0.7f;

            if (state == MovementState.walking)
            {
                footstep = footstepWalk[randomIndex];
                period = 0.55f;
            }
            else if (state == MovementState.sprinting)
            {
                footstep = footstepRun[randomIndex];
                period = 0.35f;
            }

            if (footstep)
            {
                isAudioPlaying = true;
                footstep.Play();
                Invoke(nameof(FootstepFinished), period);
            }
        }

        _hand.localPosition += posHand;
        _pistol.localPosition += posHand;

        return pos;
    }

    private void FootstepFinished()
    {
        isAudioPlaying = false;
    }

    private void CheckMotion()
    {
        ResetPosition();
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (speed < _toggleSpeed) return;
        if (!grounded) return;

        PlayMotion(FootStepMotion());
    }

    private void ResetPosition()
    {
        if (_cam.localPosition == _startPos && _hand.localPosition == _startPosHand && _pistol.localPosition == _startPosPistol) return;
        if (!paused)
        {
            _cam.localPosition = Vector3.Lerp(_cam.localPosition, _startPos, 1 * Time.deltaTime);
            _hand.localPosition = Vector3.Lerp(_hand.localPosition, _startPosHand, 1 * Time.deltaTime);
            _pistol.localPosition = Vector3.Lerp(_pistol.localPosition, _startPosPistol, 1 * Time.deltaTime);
        }
    }
    private void PlayMotion(Vector3 motion)
    {
        _cam.localPosition += motion;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        // grounded check
        if (!paused) {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.2f);
            MyInput();
            SpeedControl();
            StateHandler();

            // apply drag if we are grounded
            rb.drag = 0;
            if (grounded)
            {
                rb.drag = groundDrag;
            }

            if (!_enable) return;

            CheckMotion();
        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        //if (Input.GetKey(jumpKey) && readyToJump && grounded)
        //{
        //    readyToJump = false;
        //    Jump();
        //    Invoke(nameof(ResetJump), jumpCooldown);
        //}

        // Crouching

        if (Input.GetKeyDown(crouchKey) && !playerObject.isPlayerHidden())
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(crouchKey) && !playerObject.isPlayerHidden())
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        else if (!playerObject.isPlayerHidden() && !Input.GetKeyDown(crouchKey) && !Input.GetKey(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;
        }
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    void MovePlayer()
    {
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // slope movement
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * (movementSpeed * 20f), ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

            }
        }
        else if (grounded)
        {
            rb.AddForce(movementDirection.normalized * (movementSpeed * 10f), ForceMode.Force);
        }
        else
        {
            rb.AddForce(movementDirection.normalized * (movementSpeed * 10f * airMultiplier), ForceMode.Force);
        }
        
        // turn off gravity while on a slope to avoid sliding down
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 newVelocity = flatVelocity.normalized * movementSpeed;
                rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        // Invoke(nameof(DownwardForce), 0.25f);
        //StartCoroutine(DownwardForce());
    }

    void DownwardForce()
    {
        rb.AddForce(transform.up * Physics.gravity.y * 5, ForceMode.Impulse);
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(movementDirection, slopeHit.normal).normalized;
    }

    void Paused()
    {
        paused = true;
        ResetPosition();
    }

    void Unpause()
    {
        paused = false;
        ResetPosition();
    }

    //void ResetJump()
    //{
    //    exitingSlope = false;
    //    readyToJump = true;
    //}
}
