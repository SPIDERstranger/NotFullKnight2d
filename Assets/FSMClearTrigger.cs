using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMClearTrigger : StateMachineBehaviour
{
    public string[] ClearOnEnter;
    public string[] ClearOnExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(string mes in ClearOnEnter)
        {
            animator.ResetTrigger(mes);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (string mes in ClearOnExit)
        {
            animator.ResetTrigger(mes);
        }
    }


}
