using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 目前已知bug 但懒得修改
 * 在rotate 状态下冲刺 会产生人物方向不正确
 * jump 和 冲刺后攻击 会产生奇异的bug
 */
public class ActorController : MonoBehaviour
{
    //输入模块
    private PlayerInput PI;

    private Animator anim;


    //private SpriteRenderer Sprite;
    //属性
    public LayerMask PlayerLaymask;
    public Vector2 boxSize;
    public float gravity = 9.8f;
    public bool gravityEnable = true;
    private bool animEnable = true;
    [Header("----- 移动 -----")]
    public float moveSpeed = 15;
    public float distance = 0.001f;
    public bool flipx = false;// false 为 向右  true  为向左
    [Header("----- 跳跃 -----")]
    public float jumpVelocity;
    public float maxJumpTime;
    public int jumpCount = 2; // 跳跃总数
    private int jumpCounter = 0;  //跳跃计数器
    private float tempJumpVelocity;
    private bool isGround = true;
    //todo 修改使用count 计算跳跃次数
    private bool jumpState = false;
    private float jumpTimer = 0;
    [Header("----- 冲刺 -----")]
    public float rushTime;
    public float rushColdTime = 0.4f;
    private float rushColdTimer = 0;
    public float rushVelocity;
    private bool rushState;
    [SerializeField]
    private int Xdir = -1;//左右方向  1 为右  -1 为左
    private int Ydir = 0;  //竖直方向  1 为上  -1 为下
    public Vector3 velocity = Vector3.zero;
    void Awake()
    {
        PI = this.GetComponent<PlayerInput>();
        anim = this.GetComponent<Animator>();
        //Sprite = this.GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        LRMove();
        UDMove();
        AnimatorController();
        RushFun();
        JumpFun();

        CheckNextAndMove();
    }

    private void LRMove()
    {
        if (!PI.inputEnable)
            return;
        velocity.x = moveSpeed * PI.Lright;
        if (velocity.x > 0.001f || velocity.x < -0.001f)//有速度
        {
            Xdir = (velocity.x >= 0) ? 1 : -1; // 获取方向
        }
    } // 获取输入模块数据
    private void UDMove()
    {
        Ydir = Mathf.RoundToInt(PI.Lup);

        if (!CheckIsGround())
        {
            isGround = false;
            if (!gravityEnable)
            {
                return;
            }
            velocity.y -= gravity * Time.deltaTime;
            velocity.x *= 0.8f;
        }
        else
        {
            isGround = true;
            velocity.y = 0;

        }
    } //获取输入模块数据

    private void AnimatorController() // todo 优化这一段代码 使其更加简洁
    {
        if (!animEnable)
            return;
        anim.SetFloat("runSpeed", Mathf.Abs(velocity.x));
        if (PI.rush && !rushState && rushColdTimer <= 0)
            anim.SetTrigger("rush");
        if (!isGround)
        {
            flipx = !(velocity.x > 0);
            if (jumpCounter == 0)
            {
                jumpCounter = 1;
            }
        }
        if (flipx == (Xdir > 0)) // 方向不等时
        {
            anim.SetTrigger("rotate");
        }
        else
        {
            //Sprite.flipX = !flipx;
            this.transform.localScale = new Vector3(!flipx?-1:1, 1, 1);

        }

        if (velocity.x > 1f || velocity.x < -1f)
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);
        }
        anim.SetBool("jump", (velocity.y > 1f));
        anim.SetBool("fall", (velocity.y < -0.5f));

        anim.SetInteger("jumpCounter", jumpCounter);
        anim.SetBool("isGround", isGround);
        anim.SetInteger("Ydir", Ydir);
        if(PI.attack)
            anim.SetTrigger("attack");


    }



    private void CheckNextAndMove()
    {
        if (velocity.x > 0.001f || velocity.x < -0.001f)//有速度
        {
            float moveDistance = velocity.x * Time.deltaTime;
            RaycastHit2D hitinfo = Physics2D.BoxCast(this.transform.position, boxSize, 0, Vector2.right * Xdir, 5f, PlayerLaymask);
            if (hitinfo.collider != null)//前方碰撞体
            {
                float tempXValue = (float)System.Math.Round(hitinfo.point.x, 1);
                float tempDistance = Mathf.Abs(this.transform.position.x - tempXValue);
                if (tempDistance > distance + boxSize.x * 0.5f)
                {
                    transform.position += Vector3.right * moveDistance;
                }
                else //距离过近时
                {
                    this.transform.position = new Vector3(tempXValue + -Xdir * (boxSize.x * 0.5f + distance), transform.position.y, transform.position.z);
                }
            }
            else
            {
                transform.position += Vector3.right * moveDistance;
            }
        }

        if (velocity.y > 0.01f || velocity.y < -0.01f)
        {
            int dir = (velocity.y > 0) ? 1 : -1;
            float moveDistance = velocity.y * Time.deltaTime;
            RaycastHit2D hitinfo = Physics2D.BoxCast(this.transform.position, boxSize, 0, Vector2.up * dir, 5f, PlayerLaymask);
            if (hitinfo.collider != null)//前方碰撞体
            {
                float tempYValue = (float)System.Math.Round(hitinfo.point.y, 1);
                float tempDistance = Mathf.Abs(this.transform.position.y - tempYValue);
                if (tempDistance > distance + boxSize.y * 0.5f)
                {
                    transform.position += Vector3.up * moveDistance;
                }
                else //距离过近时
                {
                    this.transform.position = new Vector3(transform.position.x, tempYValue + -dir * (boxSize.y * 0.5f + distance), transform.position.z);
                    velocity.y = 0;
                    jumpTimer = maxJumpTime;
                }
            }
            else
            {
                transform.position += Vector3.up * moveDistance;
            }

        }
    }

    private bool CheckIsGround()
    {
        RaycastHit2D hitinfo = Physics2D.BoxCast(this.transform.position, boxSize, 0, Vector3.down, 5f, PlayerLaymask);
        if (hitinfo.collider != null)
        {
            float tempDistance = this.transform.position.y - hitinfo.point.y;
            if (tempDistance > boxSize.y * 0.5f + distance)
                return false;
            else
                return true;
        }
        return false;
    }

    private void JumpFun() // todo 待修改 使用更加的合理的信号判断
    {
        if (PI.Jump) //按键变化信号
        {
            if (PI.lastJump && jumpCounter < jumpCount)//按键 down 时
            {//进入跳跃状态
                jumpState = true;
                velocity.y = jumpVelocity * 0.5f;
                jumpTimer = 0;
                jumpCounter++;
            }
            else //按键  up  时
            {
                jumpState = false;
                jumpTimer = 0;
            }
        }
        else if (jumpState && PI.inputEnable) // 按住 按键 时
        {
            if (jumpTimer > maxJumpTime) // 超出跳跃时间时 
            {
                jumpTimer = 0;
                jumpState = false;
            }
            else
            {
                velocity.y = Mathf.SmoothDamp(velocity.y, jumpVelocity, ref tempJumpVelocity, Time.deltaTime);
                jumpTimer += Time.deltaTime;
            }
        }
    }

    private void RushFun()
    {
        rushColdTimer -= Time.deltaTime;
        if (PI.rush && !rushState && rushColdTimer <= 0)
        {
            StartCoroutine(RushMove(rushTime));
        }
    }

    IEnumerator RushMove(float rushTime)
    {
        PI.inputEnable = false;
        gravityEnable = false;
        //animEnable = false ;
        rushState = true;
        velocity.y = 0;
        velocity.x = Xdir * rushVelocity;

        yield return new WaitForSeconds(rushTime);

        rushColdTimer = rushColdTime;
        PI.inputEnable = true;
        gravityEnable = true;
        rushState = false;
        //animEnable = true;
    }

    private void AttackFun()
    {
            //todo 检测 攻击对象
    }

        
    #region FSM 接收的方法
    private void Rotate()
    {
        flipx = !flipx;
    }
    private void ResetJump()
    {
        jumpCounter = 0;
    }

    private void RunAttackEnter()
    {
        PI.inputEnable = false;
        velocity.x = Xdir * rushVelocity;
    }
    private void RunAttackExit()
    {
        PI.inputEnable = true;
    }
    #endregion
}
