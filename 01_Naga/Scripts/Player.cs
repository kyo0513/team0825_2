using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//コメント//
//ObjectCollision  は　Enemy1　に置き換える

public class Player : MonoBehaviour
{
    //インスペクターで設定する
    [Header("速さ　初期値3")] public float speed;
    [Header("重力　初期値5")] public float gravity;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("高さ制限")] public float jumpHeight;
    [Header("ジャンプ制限時間")] public float jumpLimitTime;
    [Header("天井判定")]public GroundCheck head;
    [Header("接地判定")]public GroundCheck ground;
    [Header("踏みつけ判定の高さの割合(%)")] public float stepOnRate;
    [Header("ダッシュの速さ表現")]public AnimationCurve  dashCurve;
    [Header("ジャンプの速さ表現")]public AnimationCurve  jumpCurve;

    //音関連
    [Header("ジャンプする時に鳴らすSE")] public AudioClip jumpSE;
    [Header("やられた鳴らすSE")] public AudioClip downSE;
    [Header("コンティニュー時に鳴らすSE")] public AudioClip continueSE; 


     //プライベート変数
    private Animator anim   = null;
    private Rigidbody2D rb  = null;
    private CapsuleCollider2D capcol  = null;
    private SpriteRenderer sr  = null;
    private MoveFloor moveObj  = null;    //移動床用 08/28
    private bool isGround      = false;
    private bool isJump        = false;
    private bool isRun         = false;
    private bool isHead        = false; 
    private bool isDown        = false;
    private bool isOtherJump   = false;
    private bool isContinue    = false;
    private bool isClearMotion = false;
    private float jumpPos      = 0.0f;
    //private float jumpTime  = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float otherJumpSpeed  = 0.0f;
    //private float dashTime, jumpTime;
    private float dashTime     = 0.0f;
    private float jumpTime     = 0.0f;
    private string enemyTag    = "Enemy";
    private float beforeKey    = 0.0f;
    private float continueTime = 0.0f;
    private float blinkTime    = 0.0f;

    //ゲームオーバー処理追加 08/27
    private bool nonDownAnim    = false;

    //タグ関連置き場
    //落下エリア・ダメージエリア追加 08/27
    private string deadAreaTag  = "DeadArea";
    private string hitAreaTag   = "HitArea";
    private string moveFloorTag = "Move";
    private string fallFloorTag = "Fall";
    private string jumpStepTag  = "JumpStep";

    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim   = GetComponent<Animator>();
        rb     = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr     = GetComponent<SpriteRenderer>();
    }
    private void Update() 
    {
        if (isContinue)
        {
            //明滅　ついている時に戻る
            if (blinkTime > 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えているとき
            else if (blinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅　ついているとき
            else
            {
                sr.enabled = true;
            }
            //1秒たったら明滅終わり
            if (continueTime > 1.0f)
            {
                isContinue   = false;
                blinkTime    = 0.0f;
                continueTime = 0.0f;
                sr.enabled   = true;
            }
            else
            {
                blinkTime    += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        } 
    } 

    void FixedUpdate()
    {
        //if (!isDown)
        //if (!isDown && !GameController.instance.isGameOver)
        if (!isDown && !GameController.instance.isGameOver && !GameController.instance.isStageClear)
        {
            //接地判定を得る
            isGround = ground.IsGround();
            isHead   = head.IsGround();

            //各種座標軸の速度を求める
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            //アニメーションを適用
            SetAnimation();

            //キー入力されたら行動する
            //float horizontalKey = Input.GetAxis("Horizontal");
            //float xSpeed = 0.0f;
            //float ySpeed = -gravity;
            //float verticalKey = Input.GetAxis("Vertical");

            //移動速度を設定
            //移動床処理追加 08/28
            Vector2 addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                addVelocity = moveObj.GetVelocity();
            }
            //rb.velocity = new Vector2(xSpeed, ySpeed);
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
            //移動床処理追加終了 08/28
        }
        else
        {
            if (!isClearMotion && GameController.instance.isStageClear)
            {
                anim.Play("player_clear");
                isClearMotion = true;
            }

            rb.velocity = new Vector2(0, -gravity);
        }
    }

    /// Y成分で必要な計算をし、速度を返す。
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        //何かを踏んだ際のジャンプ
        if (isOtherJump)
        {
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (canHeight && canTime && !isHead)
            {
                //ySpeed   = jumpSpeed;
                ySpeed    = otherJumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime    = 0.0f;
            }
        }
        else if (isGround) //地面にいるとき
        {
            if (verticalKey > 0)
            {
                if (!isJump)
                {   
                    //GameController.instance.PlaySE(jumpSE);
                }
                ySpeed   = jumpSpeed;
                jumpPos  = transform.position.y; //ジャンプした位置を記録する
                isJump   = true;
                jumpTime = 0.0f;
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump) //ジャンプ中
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed    = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump   = false;
                jumpTime = 0.0f;
            }
        }
        if (isJump || isOtherJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        return ySpeed;
    }

    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed        = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isRun      = true;
            dashTime  += Time.deltaTime;
            xSpeed     = speed;
        }
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isRun      = true;
            dashTime  += Time.deltaTime;
            xSpeed     = -speed;
        }
        else
        {
            isRun    = false;
            xSpeed   = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        xSpeed   *= dashCurve.Evaluate(dashTime);
        beforeKey = horizontalKey;
        return xSpeed;
    }
    
    /// アニメーションを設定する
    private void SetAnimation()
    {
        //anim.SetBool("jump"     , isJump || isOtherJump);
        //anim.SetBool("ground"   , isGround);
        anim.SetBool("IsMoving" , isRun);
    }

    //コンテニューとダウンアニメーション関連
    public bool IsContinueWaiting() 
    { 
        //Debug.Log("判定1");
        //return IsDownAnimEnd();
        if (GameController.instance.isGameOver)
        {
            return false;
        }
        else
        {
            return IsDownAnimEnd() || nonDownAnim;
        } 
    }

    //ダウンアニメーション完了判定処理
    private bool IsDownAnimEnd() 
    {
        //Debug.Log("ダウンアニメーションの完了判定");
        if(isDown && anim != null) 
        {
            //Debug.Log("判定2"); 
            AnimatorStateInfo currentState =
                anim.GetCurrentAnimatorStateInfo(0);

            //if (currentState.IsName("player_down")) 
            if (currentState.IsName("Play_Down")) 
            { 
                //Debug.Log("判定2-1");
                if(currentState.normalizedTime >= 1) 
                { 
                    //Debug.Log("判定2-2");
                    return true; 
                } 
            } 
        } 
        return false; 
    }

    /// コンティニューする
    public void ContinuePlayer()
    {
        //GameController.instance.PlaySE(continueSE);
        //Debug.Log("判定3");
        isDown        = false;
        //anim.Play("player_stand");
        anim.Play("Play_Idle");
        isJump        = false;
        isOtherJump   = false;
        isRun         = false;
        isContinue    = true;
        nonDownAnim   = false;
    }

    //やられた時の処理
    private void ReceiveDamage(bool downAnim) 
    { 
        //if (isDown)
        if (isDown || GameController.instance.isStageClear)
        {     
            return;
        }
        else
        {
            if (downAnim)
            {
                anim.Play("Play_Down");
            }
            else
            {
                nonDownAnim = true;
            }
            isDown = true;
            //GameController.instance.PlaySE(downSE);
            GameController.instance.Sublife();
        }
    }


    //接地判定関連
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool enemy     = (collision.collider.tag == enemyTag);
        bool moveFloor = (collision.collider.tag == moveFloorTag);
        bool fallFloor = (collision.collider.tag == fallFloorTag);
        bool jumpStep  = (collision.collider.tag == jumpStepTag);

        //if (collision.collider.tag == enemyTag)
        //if (enemy || moveFloor || fallFloor)
        if (enemy || moveFloor || fallFloor || jumpStep)
        {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in collision.contacts)
            {
                if (p.point.y < judgePos)
                {
                    //if (enemy || fallFloor)
                    if (enemy || fallFloor || jumpStep)
                    {
                        Enemy1 o = collision.gameObject.GetComponent<Enemy1>();
                                        
                        if (o != null)
                        {
                            //if (enemy)
                            if (enemy || jumpStep)
                            {
                                otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                                otherJumpSpeed  = o.jumpSpeed;
                                o.playerStepOn  = true;             //踏んづけたものに対して踏んづけた事を通知する
                                jumpPos = transform.position.y;     //ジャンプした位置を記録する 
                                isOtherJump     = true;
                                isJump          = false;
                                jumpTime        = 0.0f;
                            }
                            else if(fallFloor)
                            {
                                o.playerStepOn  = true;
			                }
                        }
                        else
                        {
                            Debug.Log("ObjectCollisionが付いてないよ!");
                        }
                    }
                    else if(moveFloor)
		            {
                        moveObj = collision.gameObject.GetComponent<MoveFloor>();
                    }
                }
                else
                {
                    if (enemy)
                    {
                        ReceiveDamage(true);
                        break;
                    }
                    //Debug.Log("当たった");
                    //anim.Play("Play_Down");
                    //isDown = true;
                    //anim.Play("player_down");
                    //isDown = true;
                    //break;

                    /*
                    if(GameController.instance != null)
                    {
                        //anim.Play("play_Down");
                        GameController.instance.life--;
                        //スタン判定を追加する必要あり//     08/27

                        if(GameController.instance.life <= 0)
                        {
                            ReceiveDamage(true);
                            //Debug.Log("やられた判定");
                            //anim.Play("play_Down");
                            //isDown = true;
                            break;
                        }
                    }
                    */                    
                }
            }
        }

        //上記整理　下記不要?? 08/28-2
        //動く床用追記 08/28
        /*
        else if (collision.collider.tag == moveFloorTag)
        {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y -
                (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in collision.contacts)
            {
                //動く床に乗っている
                if (p.point.y < judgePos)
                {
                    moveObj = collision.gameObject.GetComponent<MoveFloor>();
                }
            }         
        }
        */
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == moveFloorTag)
        {
            //動く床から離れた
            moveObj = null;
        }
    }

    //ダメージ床　落下判定 08/27
    private void OnTriggerEnter2D(Collider2D collision)	
    {
        if(collision.tag == deadAreaTag)
	    {
            //nonDownAnim    = true; 08/30　なぜコメント化に？
            //isDown         = true; 08/30  なぜコメント化に？
            GameController.instance.Zerolife();
            //ReceiveDamage(false);
	    }
	    else if(collision.tag == hitAreaTag)
	    {
            ReceiveDamage(true);
	    }
    }

    /*
        //ジャンプ関連
        if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed   = jumpSpeed;
                jumpPos  = transform.position.y; //ジャンプした位置を記録する
                isJump   = true;
                jumpTime = 0.0f;
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed    = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump   = false;
                jumpTime = 0.0f;
            }
        }
        
        //歩き関連
        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetBool("IsMoving", true);
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetBool("IsMoving", true);
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            anim.SetBool("IsMoving", false);
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }
        beforeKey = horizontalKey;

         //アニメーションカーブを速度に適用
        xSpeed *= dashCurve.Evaluate(dashTime);
        if (isJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }


        anim.SetBool("IsJumping", isJump);
        //anim.SetBool("ground", isGround); //New
        //rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        rb.velocity = new Vector2(xSpeed, ySpeed);
    }
    */
}