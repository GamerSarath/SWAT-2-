using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;

    PlayerMovement_RealGame playerMovement;

    AnimatorManager animatorManager;

    public Vector2 cameraMovementInput;

    [SerializeField] Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;


    public float verticalCameraInput;
    public float horizontalCameraInput;

    public float movementAmount;

    public bool sprintInput;

    public bool jumpInput;

    public bool fireInput;

    public bool reloadInput;

    public bool scopeInput;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement_RealGame>();
    }
    private void OnEnable()
    {
       if(playerControls != null)
        {

        }
       else
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Fire.performed += i => fireInput = true;
            playerControls.PlayerActions.Fire.canceled += i => fireInput = false;
            playerControls.PlayerActions.Reloading.performed += i => reloadInput = true;
           // playerControls.PlayerActions.Reloading.canceled += i => reloadInput = false ;

            playerControls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerControls.PlayerActions.Scope.canceled += i => scopeInput = false;


        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HnadleAllInputs()
    {

        HandleMovement();
        HandleSprintInput();
        HandleJumpingInput();
    }
    private void HandleMovement()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        horizontalCameraInput = cameraMovementInput.x;
        verticalCameraInput = cameraMovementInput.y;

        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.ChangeAnimatorValues(0, movementAmount,playerMovement.isSprinting);
    }

    void HandleSprintInput()
    {
        if(sprintInput && movementAmount > 0.5)
        {
            playerMovement.isSprinting = true; 
        }
        else
        {
            playerMovement.isSprinting = false;
        }
        
    }

    void HandleJumpingInput()
    {
        if(jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }
}
