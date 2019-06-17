using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    private PlayerInput PI;

    public LayerMask PlayerLaymask;
    public Vector2 boxSize;
    public float gravity = 9.8f;
    public float moveSpeed = 15;
    public float distance = 0.001f;
    private Vector3 velocity = Vector3.zero;

    private int dir;
    void Awake()
    {
        PI = this.GetComponent<PlayerInput>();

    }
    // Update is called once per frame
    void Update()
    {
        Control();
        CheckNextAndMove();
    }

    private void Control()
    {
        velocity.x = moveSpeed * PI.Lright;
    }

    private void CheckNextAndMove()
    {
        if (velocity.x > 0.001f || velocity.x < -0.001f)//有速度
        {
            dir = (velocity.x > 0) ? 1 : -1; // 获取方向
            float moveDistance = velocity.x * Time.deltaTime;
            RaycastHit2D hitinfo = Physics2D.BoxCast(this.transform.position, boxSize, 0, Vector2.right * dir, 5f, PlayerLaymask);
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
                    this.transform.position = new Vector3(tempXValue + -dir * (boxSize.x * 0.5f + distance), transform.position.y, transform.position.z);
                }
            }
            else
            {
                transform.position += Vector3.right * moveDistance;
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

    private void Jump()
    {
        if()
    }
}
