using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System ;
using Photon.Pun;

public class TimerScript : MonoBehaviour
{
    public static TimerScript instance; 
    public Text timerText;

    public float timeRemaining = 600f;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(timeRemaining >0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timeRemaining = 0;
            
            //End Game;
        }
    }
    private void UpdateTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text =  string.Format("{0:D2}:{1:D2}" , timeSpan.Minutes,timeSpan.Seconds);
    }
}
