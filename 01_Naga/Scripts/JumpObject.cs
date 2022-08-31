using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{
    private Enemy1 oc;
    private Animator anim;
    void Start()
    {
        
        oc = GetComponent<Enemy1>();
        
        anim = GetComponent<Animator>();
        if(oc == null || anim == null)
        {
            Debug.Log("ジャンプ台の設定が足りていません");
            Destroy(this);
        }
        
    }

    void Update()
    {
        if (oc.playerStepOn)
        {
            //Debug.Log("ジャンプ台に乗った");
            anim.SetTrigger("on");
            oc.playerStepOn = false;
        }
    }
}
