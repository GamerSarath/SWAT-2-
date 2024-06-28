using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
public class ShootingController : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerMovement playerMovement;
    public Transform firePoint;
    public float fireRate = 0.1f;

    public float fireRange = 100f;
    public float fireDamage = 10;

    float nextFireTime = 0;

    public float maxAmmo;
    float currentAmmo;
    public float reloadTIme = 1.5f;


    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;
    public bool isReloading = false;
    public bool isScopeinput;

    AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip reloadSound;

    PhotonView view;

    [Header("Effects")]

   [SerializeField] GameObject muzzleFlash;

    [SerializeField] GameObject bloodParticle;

    int playerTeam;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
        view = GetComponent<PhotonView>();

        if(view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }
    private void Update()
    {
        if (!view.IsMine)
            return;
      if(isReloading || playerMovement.isSprinting)
        {   
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);            
            animator.SetBool("ShootWalk", false);
            return; 
        }
        isWalking = playerMovement.isMoving;
        isShootingInput = inputManager.fireInput;
        isScopeinput = inputManager.scopeInput;
        if (isWalking && isShootingInput)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Debug.Log("is shooting while walking");

                //Shooting();
                InvokeRepeating("Shooting", 0, 5f);
                animator.SetBool("ShootWalk", true);
            }
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", true);
            isShooting = true;
        }

        else if (isShootingInput)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f/fireRate;
                Debug.Log("is shooting");
                // Shooting();
                InvokeRepeating(nameof(Shooting), 0, 5f);

            }
            animator.SetBool("Shoot", true);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);

            isShooting = true   ;
        }
        else if(isScopeinput)
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", true);
            animator.SetBool("ShootWalk", false);
            isShooting = false;
        }
        else
        {
            CancelInvoke("Shooting") ;

            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);

            isShooting = false;
        }

        if(inputManager.reloadInput && currentAmmo < maxAmmo)
        {

            Reloading();
        }
      
    }
    void Shooting()
    {
        if ((currentAmmo > 0))
        {
            RaycastHit hit;
            bool result = Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange);
           // Debug.Log("Result is " + result);
            Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 10f, Color.red, 10f);
            if (result)
            {

                Debug.Log(hit.transform.name);  
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;

                PlayerMovement playerMoveDamage = hit.collider.GetComponent<PlayerMovement>();
                EnemyShooter enemy = hit.collider.GetComponent<EnemyShooter>();
                if (playerMoveDamage != null && playerMoveDamage.playerTeam != playerTeam)
                {
                    playerMoveDamage.ApplyDamage(fireDamage);
                    view.RPC(nameof(RPC_Shoot), RpcTarget.All, hitPoint, hitNormal);
                }
                if(enemy != null)
                {
                    enemy.ApplyDamage(fireDamage);
                    view.RPC(nameof(RPC_Shoot), RpcTarget.All, hitPoint, hitNormal);
                }

            }

            muzzleFlash.GetComponent<ParticleSystem>().Play();
            audioSource.PlayOneShot(shootingSound);
            currentAmmo--;
        }
        else
        {
            Reloading();
        }
       

    }

    [PunRPC]

    void RPC_Shoot(Vector3 hitPoint, Vector3 hitNormal)
    {
        GameObject blood = Instantiate(bloodParticle, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(blood.gameObject,.2f  );
    }


    void Reloading()
    {
        if(!isReloading && currentAmmo < maxAmmo)
        {
            if (isShootingInput && isWalking)
            {
                animator.SetTrigger("ShootReload"); 
            }
            else
            {
                animator.SetTrigger("Reload");
            }
            isReloading = true;
            audioSource.PlayOneShot(reloadSound);

            Invoke("FinishReloading", reloadTIme);
        }
        
    }

    private void FinishReloading()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        if (isShootingInput && isWalking)
        {
            animator.ResetTrigger("ShootReload");
        }
        else
        {
            animator.ResetTrigger("Reload");
        }

    }

    

    
}
