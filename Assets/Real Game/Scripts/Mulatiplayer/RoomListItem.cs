using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;  

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomNameText;
    public RoomInfo info;

    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomNameText.text = roomInfo.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }

}
