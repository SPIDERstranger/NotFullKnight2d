using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderActorController : MonoBehaviour
{
    //输入模块
    private PlayerInput PI;

    private Animator anim;

    private Rigidbody2D rigid;

    private BoxCollider2D feet;

    private float gravity = 9.8f;
    private Vector2 velocity;
    public LayerMask GroundLayer;

    public float moveSpeed = 10f;
    [Range(0.5f,1)]
    public float airMoveSpeed;
    private bool Xdir = true;


    private int jumpCounter = 0;
    public int maxJumpCount;
    public float jumpVelocity = 15;
    public float maxJumpTime= 0.05f;
    private float jumpTimer = 0 ;
    private bool jumpState = false;



    private void Awake()
    {
        PI = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        feet = GetComponent<BoxCollider2D>();

    }


    private void Update()
    {
        velocity = rigid.velocity;
        LRMove();
        UDMove();
        AnimatorController();
        JumpFunc();
        rigid.velocity = velocity;
    }

    private void LRMove()
    {
        velocity.x = PI.Lright * moveSpeed;
        //如果在空中 velocity .y*airMoveSpeed;
    }

    private void UDMove()
    {
        velocity.y -= Time.deltaTime * gravity*4;
        if (feet.IsTouchingLayers(GroundLayer))
            velocity.y = 0;
    }

    private void AnimatorController()
    {
        anim.SetBool("run", (velocity.x > 1f || velocity.x < -1f));//设置移动
        if (velocity.x > 1f || velocity.x < -1f)//如果移动了
        {
            if (Xdir != (velocity.x > 0))//判断方向
            {
                anim.SetTrigger("rotate");
            }
        }
        transform.localScale = new Vector3(Xdir ? -1 : 1, 1, 1);
        anim.SetBool("jump", (velocity.y > 1f));
        anim.SetBool("fall", (velocity.y < -0.5f));
        anim.SetBool("isGround", feet.IsTouchingLayers(GroundLayer));
        anim.SetInteger("jumpCounter", jumpCounter);

    }

    private void JumpFunc()
    {

        if (jumpState)
        {
            if (jumpTimer < maxJumpTime)
            {
                velocity.y = jumpVelocity;
                jumpTimer += Time.deltaTime;
                Debug.Log("ininnininininin");
            }
            else
            {
                jumpTimer = 0;
                jumpState = false;
                Debug.Log("outoutoutoutoutou");
            }
        }
        if (PI.Jump)
        {
            if (PI.lastJump && jumpCounter < maxJumpCount)//按下跳跃键的时候
            {
                jumpCounter++;
                velocity.y = jumpVelocity;
                Debug.Log("aaaaaaa");
                jumpState = true;
            }
            else
            {
                jumpTimer = 0;
                jumpState = false;
                
            }
        }
    }




    #region FSM 接收的方法
    private void Rotate()
    {
        Xdir = !Xdir;
    }
    private void ResetJump()
    {
        jumpCounter = 0;
    }

    //private void RunAttackEnter()
    //{
    //    PI.inputEnable = false;
    //    rigid.velocity = Vector2.right * Xdir * rushVelocity;
    //}
    //private void RunAttackExit()
    //{
    //    PI.inputEnable = true;
    //}
    #endregion

}
