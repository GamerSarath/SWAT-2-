using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
public class EnemyNavigationScript : MonoBehaviour
{
    public static EnemyNavigationScript instance;
    public GameObject[] playerTransforms;
    [SerializeField] float fireRange;
    [SerializeField] float fireRate;
    float shootingDistance;
    float pathupdateDelay = 0.2f;
    float pathupdateDeadLine;
    NavMeshAgent agent;
    public int teamNumber;
    int playerCount;
    public Transform focusPlayer;
    Animator animator;

    public GameObject[] spawnAreas;
    public GameObject enemyDummy;
    PhotonView view;
    private void Awake()
    {
        instance = this;
        spawnAreas = GameObject.FindGameObjectsWithTag("EnemySpawnArea");
        view = GetComponent<PhotonView>();
        if(enemyDummy != null)
        {
            enemyDummy.SetActive(false);
        }

    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootingDistance = agent.stoppingDistance;
        view.RPC(nameof(RPC_SetFocusPlayer),RpcTarget.All);
        
        animator = GetComponent<Animator>();
        animator.SetBool("Running", true);
    }

    [PunRPC]
    void RPC_SetFocusPlayer()
    {
        playerTransforms =   GameObject.FindGameObjectsWithTag("Player");
        playerCount = PhotonNetwork.PlayerList.Length;
        int random = Random.Range(0, playerCount);
        Debug.Log("int is " + random + " transforms length is " + playerCount);
        focusPlayer = playerTransforms[0].transform;
    }
    private void Update()
    {
      if(playerTransforms.Length <= 0)
        {
            return;
        }

      if(agent.remainingDistance <=0)
        {
            animator.SetBool("Idle", true);
        }
      else
        {
            animator.SetBool("running", true);

        }

        bool inRange = Vector3.Distance(transform.position, focusPlayer.transform.position) <= shootingDistance;

        if(inRange)
        { 
            LookAtTarget();

         }
        else
        {
            LookAtTarget();

            UpdatePath();
        }
        animator.SetBool("Shooting", inRange);
    }

    void UpdatePath()
    {
        if(Time.time >= pathupdateDeadLine)
        {
            Debug.Log("UpdatingPath");
            pathupdateDeadLine = Time.time + pathupdateDelay;
           // agent.SetDestination(playerTransforms[0].position);
            
        }
    }

    public void LookAtTarget()
    {
        Vector3 lookPos = focusPlayer.transform.position  - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f); 
    }
    }
    
