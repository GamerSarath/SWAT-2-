using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class UsernameTeamDisplay : MonoBehaviour
{
   public Text usernameText;
    public Text teamNameText;

    public PhotonView view;

    private void Start()
    {

        if(view.IsMine)
        {
            gameObject.SetActive(false);
        }
        usernameText.text = view.Owner.NickName;

        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            teamNameText.text = "Team " + view.Owner.CustomProperties["Team"].ToString();

        }

    }
}
