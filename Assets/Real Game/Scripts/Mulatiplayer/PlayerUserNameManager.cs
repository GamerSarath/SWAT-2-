using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUserNameManager : MonoBehaviour
{
    [SerializeField] InputField usernameInput   ;
    [SerializeField] Text errorText;

    private void Start()
    {
        if(PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username"); 
        }
    }
    public void PlayerUserNameInputValueChanged()
    {
        string username = usernameInput.text;

        if(!string.IsNullOrEmpty(username)  && username.Length <= 12)
        {
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString("username", username);
            errorText.text = "";
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        else
        {
            errorText.text = "Username must not be empty and should be less than 12 characters";
        }
    }
}
