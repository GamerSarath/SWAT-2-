using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;

    public Text blueteamText ;
    public Text redteamText;
    public int blueteamScore;
    public int redteamScore;

    public GameObject resultCanvas;
    public Text blueKills;
    public Text redKills;
    public Text resultText;
    public Text objectiveText;
    PhotonView view;
    public int maxKillsRequired;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        Instance = this;
        maxKillsRequired = 40;
        objectiveText.text = "OBJECTIVES - " + maxKillsRequired;
    }

    public void PlayerDied(int playerTeam)
    {
        bool isUpdated = false;
        if(!isUpdated)
        {
            if (playerTeam == 2)
            {
                blueteamScore++;
            }
            else if (playerTeam == 1)
            {
                redteamScore++;
            }
            isUpdated = true;
        }
        
        view.RPC(nameof(UpdateScores), RpcTarget.All, blueteamScore, redteamScore);
    }
    [PunRPC]
    
    void UpdateScores(int bluescore, int redscore)
    {
        blueteamScore = bluescore;
        redteamScore = redscore;
        TimeEnd(blueteamScore,redteamScore);
        if(bluescore >= maxKillsRequired)
        {
            resultCanvas.SetActive(true);
            blueKills.text = bluescore.ToString();
            redKills.text = redscore.ToString();
            resultText.text = "BLUE TEAM WINS";
        }
        if(redscore >= maxKillsRequired)
        {
            resultCanvas.SetActive(true);
            blueKills.text = bluescore.ToString();
            redKills.text = redscore.ToString();
            resultText.text = "RED TEAM WINS";
        }
        blueteamText.text = blueteamScore.ToString();
        redteamText.text = redteamScore.ToString();
    }

    public void TimeEnd(int bluescore, int redscore)
    {
        if(TimerScript.instance.timeRemaining <= 0)
        {
            resultCanvas.SetActive(true);
            blueKills.text = bluescore.ToString();
            redKills.text = redscore.ToString();
            resultText.text = "TIME ENDS";  
        }
        
    }
}
