using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))] //アニメーションで追加

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    //敵を踏んだ判定用
    private CapsuleCollider2D capcol = null;    //カプセルコライダー情報取得用
    [Header("踏みつけ判定の高さの割合(%)")] public float stepOnRate;  //インスペクターで設定
    private float otherJumpHeight    = 0.0f;    //敵を踏んだ場合の跳ね返り
    //敵を踏んだ判定用 -END

    [SerializeField] private Animator animator; //アニメーションで追加

    [SerializeField] private int moveSpeed;
    [SerializeField] private int jumpForce;

    private bool isMoving  = false;     //移動中判定
    private bool isJumping = false;     //ジャンプ中判定用
    private bool isFalling = false;     //落下中判定

    const int DefaultLife  = 3;         //ライフ初期値
    int life = DefaultLife;             //ライフ変動用

     //private MoveFloor moveObj = null;  //移動する乗り物に乗ったとき用　8/24


    


    // Start is called before the first frame update
    void Start()
    {
        capcol = GetComponent<CapsuleCollider2D>();         //カプセルコライダー情報の取得        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");     //移動量?

        isMoving  = horizontal != 0;                        //移動中
        isFalling = rb.velocity.y < -0.5f;                  //落下中

        //向き処理下がった場合はアニメーション反転
        if (isMoving)
        {
            Vector3 scale = gameObject.transform.localScale;
            //右向きから左向きになった時、その反対の時下記に一致する
            if(horizontal < 0 && scale.x > 0 || horizontal > 0 && scale.x < 0)
            {
                scale.x *= -1;
            }
            gameObject.transform.localScale = scale;
        }

        //スペースを押した＆落下中でなければジャンプ + ジャンプ中判定を追加
        //if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !(rb.velocity.y < -0.5f))
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isFalling)
        {
            Jump();
        }

        //左右の歩行処理
        //rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y);
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y); 

        //アニメーターパラメータ管理
        animator.SetBool("IsMoving",  isMoving );
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        
    }

    /*   ***8/24 動く床関連　一旦断念
    void FixedUpdate()                               
    {
        //float xSpeed = GetXSpeed();
        //float ySpeed = GetYSpeed();

        Vector2 addVelocity = Vector2.zero;
        if (moveObj != null)
        {
            Debug.Log("MoveTest");
            addVelocity = moveObj.GetVelocity();
            rb.velocity = new Vector2(3, 1) + addVelocity;
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
        } 
    }
    */

    void Jump()
    {
        isJumping = true;        //ジャンプ判定へ
        //ジャンプ力に力を加える
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void otherJump()
    {
        isJumping = false;
        //isJumping = true;        //ジャンプ判定へ
        rb.AddForce(Vector2.up * otherJumpHeight, ForceMode2D.Impulse);
    }

    //着地判定　他コライダー(stageのタグを持つものと)と接触したらジャンプ可になる
    //足元のIstriggerを使う処理はこちら
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            isJumping = false;
        }

        //**下記の判定へ**
        //敵接触時ダメージ判定　今はライフを減らすだけ
        /*
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("接触");
            life--;

        } 
        */
        if (collision.CompareTag("Fall")) //落ちる床に乗った時用 8/24
        {
            Enemy1 o = collision.gameObject.GetComponent<Enemy1>();
            isJumping = false;
            o.playerStepOn = true;
        }
        
        if(collision.CompareTag("Move"))  //動く床に乗った時用　8/24
        {            
            isJumping = false;
        } 
        
    }
    
    //体全体の当たり判定を使う処理はこちら
    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in other.contacts)
            {
                if (p.point.y < judgePos)
                {
                    Enemy1 o = other.gameObject.GetComponent<Enemy1>();
                    if (o != null)
                    {
                        otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                        o.playerStepOn  = true;             //踏んづけたものに対して踏んづけた事を通知する

                        otherJump();
                    }
                    else
                    {
                        Debug.Log("Enemy1が付いてないよ!");
                    }
                }
                else
                {
                    if(other.gameObject.tag == "Enemy"){ 
                        //ダメージ判定　現在はライフが減るだけでノックバックや無敵処理もなし
                        Debug.Log("接触");
                        life--;
                    }
                }
            }
        }
        else
        {
            /*  ***動く床関連 8/24一旦断念
            if (other.gameObject.tag == "Move"){
                isJumping = false;
                foreach (ContactPoint2D p in other.contacts){
                    //動く床に乗っている
                    float judgePos = transform.position.y - (capcol.size.y / 2f);  //物の高さも必要?
                    if (p.point.y < judgePos)
                    {
                        moveObj = other.gameObject.GetComponent<Movefllor>();
                    }
                }

            }
            */

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Move")
        {
            //動く床から離れた
            //moveObj = null;
        }
    }
    

    //ゲームコントローラーにプレイヤーのライフを返す
    public int Life()
    {
        return life;
    }


}
