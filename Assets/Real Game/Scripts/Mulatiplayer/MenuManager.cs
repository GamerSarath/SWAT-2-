using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance;
    public Menu[] menus;

    private void Awake()
    {
        instance = this;
    }

    public void OpenMenu(string menuName)
    {
        Debug.Log("Open Menu. MEnu Name is" + menuName);
        for(int i = 0; i < menus.Length; i++)
        {

            if (menus[i].name == menuName)
            {
                Debug.Log("Condition true" + menuName);

                menus[i].Open();
            }
            else if(menus[i].isOpen)
            {
                Debug.Log("Condition false");

                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu )
    {
        Debug.Log("Opening Menu");
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();    
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
