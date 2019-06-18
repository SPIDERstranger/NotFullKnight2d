using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnEnter : StateMachineBehaviour
{
    public string[] OnEnterMes;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(string mes in OnEnterMes)
        {
            animator.gameObject.SendMessage(mes);
        }
    }

}
