using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunToCover : IState
{
    IState currentState;
    EnemyNavigationScript enemy;
    GameObject coverarea;
    void Update()
    {
       // currentState.UpdateState();
    }

    public  RunToCover(EnemyNavigationScript _enemy, GameObject coverArea)
    {
        enemy = _enemy;
        coverarea = coverArea;
        OnEnter();
    }
    public void ChangeState(IState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
    
    public void OnEnter()
    {
       // Debug.Log("cover area is " + coverarea);
        Cover nextCover = this.coverarea.GetComponent<CoverArea>().GetRandomCover(enemy.transform.position); 
     //   Debug.Log("Next Cover is " +  nextCover);
        enemy.GetComponent<NavMeshAgent>().SetDestination(nextCover.transform.position);
    }
    public void UpdateState()
    {

    }

    public void OnHurt()
    {

    }

    public void OnExit()
    {

    }

    public Color Gizmos()
    {
        return Color.blue;
    }

    public void Tick()
    {

    }
}

