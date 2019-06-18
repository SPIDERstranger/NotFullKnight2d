using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnExit : StateMachineBehaviour
{
    public string[] OnExitMes;
   

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (string mes in OnExitMes)
        {
            animator.gameObject.SendMessage(mes);
        }
    }

    
}
