using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;



public class PlayerControllerManager : MonoBehaviourPunCallbacks
{
    PhotonView view;
    GameObject controller;
    GameObject[] enemyController;
    public int playerTeam;
    public int enemyTeam;
    Dictionary<int, int> playerTeams = new Dictionary<int, int>();
    bool  isStartingGame = false;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if(view.IsMine)
        {
            CreateController();
         
        }
    }


    void CreateController()
    {
      if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("Players team is " + playerTeam);
        }

        AssignPlayerToSpawnArea(playerTeam);
    }

    public void Die()
    {
       controller.gameObject.SetActive(false);
        CreateController(); 

    }

    void AssignPlayerToSpawnArea(int team)
    {
        GameObject spawnArea1 = GameObject.Find("SpawnArea1");
        GameObject spawnArea2 = GameObject.Find("SpawnArea2");
        GameObject[] botSpawnArea = new GameObject[EnemyNavigationScript.instance.spawnAreas.Length];

        for (int i = 0; i < botSpawnArea.Length;i++)
        {
            botSpawnArea[i]  = EnemyNavigationScript.instance.spawnAreas[Random.Range(0, botSpawnArea.Length)];
            enemyTeam = i % 2;
            if(enemyTeam == 0)
            {
               //PhotonNetwork.LocalPlayer.CustomProperties["Team"] = enemyTeam;
                playerTeams[i] = 1;
            }
            else
            {
                playerTeams[i] = 0;
            }
        }
        if(spawnArea1 == null || spawnArea2 == null)
        {
            Debug.LogError("No Spawn Areas Specified");
            return;
        }

        Transform spawnpoint = null;

        if(team == 1)
        {
            spawnpoint = spawnArea1.transform.GetChild(Random.Range(0, spawnArea1.transform.childCount));
        }
        else
        {
            spawnpoint = spawnArea2.transform.GetChild(Random.Range(0, spawnArea2.transform.childCount));
        }

        if(spawnpoint != null)
        {
            enemyController = new GameObject[10];
            if(!isStartingGame)
            {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { view.ViewID });
                Debug.Log("Instantiated Player Controller at spawn point ");

                for (int i = 0; i < EnemyNavigationScript.instance.spawnAreas.Length; i++)
                {
                    Debug.Log("Instantiating Enemies" + i + botSpawnArea.Length);
                    enemyController[i] = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyBot"), botSpawnArea[i].transform.position, botSpawnArea[i].transform.rotation, 0, new object[] { view.ViewID });
                    enemyController[i].GetComponent<EnemyShooter>().enemyController = enemyController[i];
                    if(spawnpoint.gameObject.tag == "Spawn1")
                    {
                        EnemyNavigationScript.instance.teamNumber = 2;
                    }
                    else
                    {
                        EnemyNavigationScript.instance.teamNumber = 1;
                    }
                }
                isStartingGame = true;
            }
            else
            {
                controller.transform.position = spawnpoint.transform.position;
                controller.gameObject.SetActive(true);

                controller.GetComponent<PlayerMovement>().Awake();
            }
            
        }
        else
        {
            Debug.LogError("No Available Spawn Point for team " + team);

        }
    }

    void AssignTeamsToPlayers()
    {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if(player.CustomProperties.ContainsKey("Team"))
            {
                int _team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = _team;

                AssignPlayerToSpawnArea(_team);
            }
        }
    }

    public override void OnPlayerEnteredRoom( Photon.Realtime.Player newPlayer)
    {
        AssignTeamsToPlayers();
    }
}
