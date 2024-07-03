using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyBrain : MonoBehaviour
{
    private StateMachine stateMachine;

    public GameObject coverAreas;
    RunToCover runToCover;
    private void Start()
    {
        coverAreas = GameObject.FindGameObjectWithTag("CoverAreasParent");
        stateMachine = new StateMachine();
        runToCover = new RunToCover(EnemyNavigationScript.instance, coverAreas); 

        void At(IState from,  IState to, Func<bool> condition ) => stateMachine.AddTransition( from, to, condition );   
        void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition( to, condition );
    }

    private void Update()
    {
        stateMachine.Tick();
    }
}
