using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public Text playerNameText;
    public Text teamNumberText  ;

    public Player player;

    int team;

    public void SetUp(Player _player, int teamnumber)
    {
        player = _player;
        playerNameText.text = _player.NickName;
        team = teamnumber;
        teamNumberText.text ="Team " + team.ToString();

        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["Team"] = team;
        player.SetCustomProperties(customProps);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
