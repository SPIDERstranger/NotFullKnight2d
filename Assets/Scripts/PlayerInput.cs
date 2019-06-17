using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("----- key设置 -----")]
    public string keyUp;
    public string keyDown;
    public string keyLeft;
    public string keyRight;
    public string keyJump;
    

    [Header("----- 方向信号 -----")]
    public float Lup;
    public float Lright;
    public bool Jump = false;
    public bool lastJump = false;
    public float smoothTime;

    [Header("----- 其他设置 -----")]
    public bool inputEnable = true;
    private float targetLup;
    private float targetLright;
    private float tempLup;
    private float tempLright;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputEnable)
        {
            targetLup = (Input.GetKey(keyUp) ? 1f : 0) - (Input.GetKey(keyDown) ? 1f : 0);
            targetLright = (Input.GetKey(keyRight) ? 1f : 0) - (Input.GetKey(keyLeft) ? 1f : 0);
            if(Input.GetKey(keyJump)!=lastJump)
            {
                Jump = true;
                lastJump = !lastJump;
            }// 获取jump 触发器型号
            else
            {
                Jump = false;
            }

        }
        else
        {
            targetLright = 0f;
            targetLup = 0f;
            Jump = false;
            lastJump = false;
        }
        Lup = Mathf.SmoothDamp(Lup,targetLup,ref tempLup,smoothTime);
        Lright = Mathf.SmoothDamp(Lright,targetLright,ref tempLright,smoothTime);

    }
}
