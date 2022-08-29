using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    bool isCliming;
    public float distance;
    public LayerMask laddarLayer;
    public void LateUpdate()
    {

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isCliming = false;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); //方向キーのx方向の入力
        float y = Input.GetAxisRaw("Vertical"); //方向キーのy方向の入力

        rb.velocity = new Vector2(x*speed, rb.velocity.y);

        //Ladder上昇
        //上に登れる条件を決定する
        //・はしごがある
        //Physics2D.RaycastHit2D.Raycast(どこから, どの方向, どれぐらいの距離で, 対象のLayer);
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position,Vector2.up, 2, laddarLayer );
        if(hitInfo.collider != null)
        {
            if(y>0)
            {
                //・Playerが上を押している
                isCliming = true;
            }
        }
        else
        {
            isCliming = false; 
        }
        
        
        

        //if 上に登れるなら
        if(isCliming)
        {
        //上にのぼる
        rb.velocity = new Vector2(rb.velocity.x, y*speed);
        rb.gravityScale = 0;
        }
        else
        {
        rb.gravityScale = 5;
        }
    }
    
}
