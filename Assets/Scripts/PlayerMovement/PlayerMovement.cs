using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;
    Vector3 moveDirection;
    AnimatorManager animatorManager;
    Transform mainCam;
    Animator animator;
    Rigidbody playerRB;
    [SerializeField] PlayerControllerManager playerControllerManager;
    PhotonView view;
    public Slider healthbarSlider;
    public GameObject playerUI;

    const float maxHealth = 200f;
    public float currentHealth;
    public float raycastradius;
    public float movementSpeed = 2;
    public float rotationSPeed = 13;
    public float sprintSpeed = 5;
    public bool isSprinting;
    public bool isMoving;
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastheightOffscet = 0.5f;
    public LayerMask groundLayer;
    public bool isGrounded ;
    public float gravityIntensity = -50;
    public float jumpHeight;
    public bool isJumping;
    public int playerTeam;
    public void Awake()
    {
        currentHealth = maxHealth;
        inputManager = GetComponent<InputManager>();
        playerRB = GetComponent<Rigidbody>();
        mainCam = Camera.main.transform;
        playerManager = GetComponent<PlayerManager>();  
        animatorManager = GetComponent<AnimatorManager>();
        animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();
        healthbarSlider.minValue = 0;
        healthbarSlider.maxValue = maxHealth;
        healthbarSlider.value = currentHealth;
        
    }
    private void Start()
    {
        if(!view.IsMine)
        {
            Destroy(playerRB);
            Destroy(playerUI);
        }
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
          
    }
    
    public void HandleAllMovement()
    {
         HanldeFallingAndLanding();
        if (playerManager.isInteracting)
        {
            return; 
        }
        HandleMovement();
        HandleRotation();
    }


     void HandleMovement()
    {
        if (isJumping)
            return;
        
        moveDirection = new Vector3(mainCam.forward.x, 0f, mainCam.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + mainCam.right * inputManager.horizontalInput;
            
        moveDirection.Normalize();

        moveDirection.y = 0;

       if(isSprinting ) 
        {
            moveDirection *= sprintSpeed;
        }           
        else
        {
            if(inputManager.movementAmount >= 0.5f)
            {
                moveDirection    *= movementSpeed;
                isMoving = true;
            }

            if (inputManager.movementAmount <= 0f)
            {
                isMoving = false;
            }

        }

        Vector3 movementVelocity = moveDirection;
        playerRB.velocity = movementVelocity;


    }

     void HandleRotation()
    {
        if (isJumping)
            return;
        Vector3 targetDirection = Vector3.zero;

        targetDirection = mainCam.forward * inputManager.verticalInput;
        targetDirection = targetDirection +  mainCam.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        raycastradius = 2;
        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSPeed * Time.deltaTime);
        transform.rotation = playerRotation;

    }

    void HanldeFallingAndLanding()
    {
        RaycastHit hit;

        Vector3 raycastOrigin = transform.position;
        Vector3 targetPositiion;
        raycastOrigin.y = raycastOrigin.y + raycastheightOffscet;
        targetPositiion = transform.position;

        if(!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            { 
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer += Time.deltaTime;
            playerRB.AddForce(transform.forward *leapingVelocity);
            playerRB.AddForce(-Vector3.up * fallingVelocity * inAirTimer);

        }
        bool r = Physics.SphereCast(raycastOrigin, .2f  , -Vector3.up, out hit, groundLayer);
       // Debug.Log(r);
        if (r)
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
               // playerManager.isInteracting = false;
                //  animator.SetBool("isInteracting", false);
            }

            Vector3 raycasthitPoint = hit.point;
            targetPositiion.y = raycasthitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else 
        {
            isGrounded = false;
           
        }
        if(isGrounded && !isJumping)
        {

            if(playerManager.isInteracting ||  inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPositiion, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPositiion;
            }
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRB.velocity = playerVelocity;
            isJumping = false;
        }
    }

    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
    }
    public void ApplyDamage(float damageValue)
    {
        view.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damageValue);
    }

    [PunRPC]

    void RPC_TakeDamage(float damage)
    {
        if(!view.IsMine)
        {
            return;
        }
        currentHealth -= damage;
        healthbarSlider.value = currentHealth;      
        if(currentHealth <= 0)
        {
            Die();
        }
        Debug.Log("Damage taken" + damage);
        Debug.Log("Current Health is" + currentHealth   );

    }

    void Die()
    {
        playerControllerManager.Die();
        ScoreBoard.Instance.PlayerDied(playerTeam);
    }
}
        