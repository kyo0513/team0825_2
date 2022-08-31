using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("エフェクトがついた床を判定するか")]public bool checkPlatformGroud = true;
    private string groundTag    = "Ground";
    private string platformTag  = "GroundPlatform";
    private string moveFloorTag = "Move";
    private string fallFloorTag = "Fall";


    private bool isGround       = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;

    //接地判定を返すメソッド
　　//物理判定の更新毎に呼ぶ必要がある
    public bool IsGround()
    {    
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }
        else if (isGroundExit)
        {
            isGround = false;
        }

        isGroundEnter  = false;
        isGroundStay   = false;
        isGroundExit   = false;
        return isGround;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("触れた");
        if (collision.tag == groundTag)
        {
            //Debug.Log("接地");
            isGroundEnter = true;
        }
        //以下移動床用に追加
        else if(checkPlatformGroud && (collision.tag == platformTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag ))
        {
            isGroundEnter = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundStay = true;
        }
        ////以下移動床用に追加
        else if (checkPlatformGroud && (collision.tag == platformTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag))
        {
            isGroundStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundExit = true;
        }
        ////以下移動床用に追加
        else if (checkPlatformGroud && (collision.tag == platformTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag))
        {
            isGroundExit = true;
        }
    }
    
}
