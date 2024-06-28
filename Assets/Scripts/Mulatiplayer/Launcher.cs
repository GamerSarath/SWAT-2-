using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Unity.VisualScripting;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    public GameObject startButtton;

    int nextTeamNumber = 1;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {       
        Debug.Log("Connecting To Master");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        MenuManager.instance.OpenMenu("UserNameMenu");
        Debug.Log("Connected To Lobby");

    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        else
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.instance.OpenMenu("LoadingMenu");
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for(int i= 0; i < players.Length; i++)
        {
            int teamNumber = GetNextTeamNUmber();
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i],teamNumber);
        }
        startButtton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButtton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);   
        MenuManager.instance.OpenMenu("ErrorMenu");
        errorText.text = "CREEATING ROOM FAILED" + message;

    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loadingenu");

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

        MenuManager.instance.OpenMenu("Loadingenu");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        MenuManager.instance.OpenMenu("TitleMenu");             
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach(Transform t in roomListContent )
        {
            Destroy(t.gameObject);
        }

        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);

        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        int teamNumber = GetNextTeamNUmber();
        GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
        playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer, teamNumber);

    }


    int GetNextTeamNUmber()
    {
        int teanNumber = nextTeamNumber;
        nextTeamNumber = 3 - nextTeamNumber;
        return teanNumber;
    }

}
