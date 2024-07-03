using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{

    Animator animator;
    int horizontalValue;
    int verticalValue;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalValue = Animator.StringToHash("Horizontal");
        verticalValue = Animator.StringToHash("Vertical");
    }
    public void ChangeAnimatorValues(float horizontalMovement, float verticalMovement, bool issprinting)
    {
        float snappedHorizontalMovement;
        float snappedVerticalMovement;

        #region Snapped Horizontal
        if(horizontalMovement > 0 &&  horizontalMovement < 0.55)
        {
            snappedHorizontalMovement = 0.5f;
        }
        else if(horizontalMovement > 0.55)
        {
            snappedHorizontalMovement = 1;
        }
        else if(horizontalMovement < 0 && horizontalMovement >=0.55f)
        {
            snappedHorizontalMovement = -0.55f;
        }
        else if(horizontalMovement < -0.55f)
        {
            snappedHorizontalMovement = -1;
        }
        else
        {
            snappedHorizontalMovement = 0;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55)
        {
            snappedVerticalMovement = 0.5f;
        }
        else if (verticalMovement > 0.55)
        {
            snappedVerticalMovement = 1;
        }
        else if (verticalMovement < 0 && verticalMovement >= 0.55f)
        {
            snappedVerticalMovement = -0.55f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVerticalMovement = -1;
        }
        else
        {
            snappedVerticalMovement = 0;
        }
        #endregion

        if(issprinting)
        {
            snappedHorizontalMovement = horizontalMovement;
            snappedVerticalMovement = 2;    

        }
        else
        {
            
        }
        animator.SetFloat(horizontalValue, snappedHorizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalValue, snappedVerticalMovement, 0.1f, Time.deltaTime);


    }

   public  void PlayTargetAnimation(string targetAnim, bool isinteracting)
    {
        animator.SetBool("isInteracting", isinteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }   
}
