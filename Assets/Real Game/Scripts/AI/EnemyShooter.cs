using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemyShooter : MonoBehaviour
{

    public float currentHealth;
    public float maxHealth;
    public float fireDamage = 10;
    public Transform shootPoint;

    public Transform gunPoint;

    public LayerMask layerMask;

    public Vector3 spread = new Vector3(0.06f, 0.06f, 0.06f);

    public TrailRenderer bulletTrail;
    [SerializeField] GameObject bloodParticle;
    PhotonView view;
    public GameObject enemyController;
    Animator animator;
    bool scoreUpdated = false;

    private void OnEnable()
    {
        scoreUpdated = false;
    }
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    void Shoot()
    {
        Vector3 direction = GetDirection();

        if(Physics.Raycast(shootPoint.position, direction, out RaycastHit hitInfo, float.MaxValue, layerMask))
        {
            Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 1f);

            TrailRenderer trail = Instantiate(bulletTrail, gunPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail,hitInfo ));

            if(hitInfo.collider.GetComponent<PlayerMovement_RealGame>())
            {
                GiveDamage(fireDamage);
                view.RPC(nameof(RPC_BotShoot), RpcTarget.All, hitInfo.point, hitInfo.normal);

            }
        }
    }

    Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
        direction +=  new Vector3(Random.Range(-spread.x,spread.x), Random.Range(-spread.y, spread.y), Random.Range(-spread.z, spread.z ));
        direction.Normalize();
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0f;
        Vector3 startP = trail.transform.position;

        while (time < .1f)
        {
            trail.transform.position = Vector3.Lerp(startP, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Destroy(trail.gameObject, .1f );
    }

    [PunRPC]

    void RPC_BotShoot(Vector3 hitPoint, Vector3 hitNormal)
    {
        GameObject blood = Instantiate(bloodParticle, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(blood.gameObject, .2f);
    }

    [PunRPC]

    void RPC_EnemyTakeDamage(float damage)
    {
        if (!view.IsMine)
        {
            return;
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        // Debug.Log("Damage taken" + damage);
        // Debug.Log("Current Health is" + currentHealth);

    }
    [PunRPC]

    void RPC_EnemyGiveDamage(float damage)
    {
        if (!view.IsMine)
        {
            return;
        }
        EnemyNavigationScript.instance.focusPlayer.GetComponent< PlayerMovement_RealGame>(). currentHealth -= damage;
        EnemyNavigationScript.instance.focusPlayer.GetComponent<PlayerMovement_RealGame>().healthbarSlider.value = EnemyNavigationScript.instance.focusPlayer.GetComponent<PlayerMovement_RealGame>().currentHealth;
        if (EnemyNavigationScript.instance.focusPlayer.GetComponent<PlayerMovement_RealGame>().currentHealth <= 0)
        {
            EnemyNavigationScript.instance.focusPlayer.GetComponent<PlayerMovement_RealGame>().currentHealth = 0;
            EnemyNavigationScript.instance.focusPlayer.GetComponent<PlayerMovement_RealGame>().ApplyDamage(0);
        }
        // Debug.Log("Damage taken" + damage);
        // Debug.Log("Current Health is" + currentHealth);

    }
    public void GiveDamage(float damageValue)
    {
        view.RPC(nameof(RPC_EnemyGiveDamage), RpcTarget.All, damageValue);

    }
    public void ApplyDamage(float damageValue)
    {
        view.RPC(nameof(RPC_EnemyTakeDamage), RpcTarget.All, damageValue);
    }
    void Die()
    {
        view.RPC(nameof(RPC_Spawn), RpcTarget.All);

        
    }

    [PunRPC]

    void RPC_Spawn()
    {
        bool isSPawned = false;
        animator.SetBool("Die", true);
        gameObject.SetActive(false);
        GameObject botSpawnArea;
        GameObject enemyController;
        if(!isSPawned)
        {
            Debug.Log("Enemy Instantiated");

            if (!scoreUpdated)
            {
                ScoreBoard.Instance.PlayerDied(EnemyNavigationScript.instance.teamNumber);
                
                scoreUpdated = true;
            }
            botSpawnArea = EnemyNavigationScript.instance.spawnAreas[Random.Range(0, EnemyNavigationScript.instance.spawnAreas.Length)];
            gameObject.SetActive(true);
            gameObject.transform.position = botSpawnArea.transform.position;
            currentHealth = maxHealth;

           // enemyController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyBot"), , botSpawnArea.transform.rotation, 0, new object[] { view.ViewID });
            //nemyController.GetComponent<EnemyShooter>().enemyController = enemyController;
            isSPawned = true;
            
        }

    }
}



