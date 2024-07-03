using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
public class GameManager : MonoBehaviour
{
    public bool isMenuOpened;

    public GameObject menuCanvas;

   // public GameObject crossHairUI;
    private void Update()
    {       
        if(Input.GetKey(KeyCode.Escape) && !isMenuOpened)
        {
            isMenuOpened = true;
            Cursor.lockState = CursorLockMode.None;
            menuCanvas.SetActive(true);
            AudioListener.pause = true;
           // crossHairUI.SetActive(false);
        }
        else if(Input.GetKey(KeyCode.Escape) && isMenuOpened)
        {
            isMenuOpened = false;
            Cursor.lockState = CursorLockMode.Locked;
            menuCanvas.SetActive(false);
            AudioListener.pause = false;
            //crossHairUI.SetActive(true  );

        }
    }

    public void LeaveGame()
    {
        isMenuOpened = false;Debug.Log("Game LEft");
        Application.Quit();
    }
}
