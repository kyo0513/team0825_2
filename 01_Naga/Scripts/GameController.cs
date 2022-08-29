using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //ゲームコントローラー用
    public static GameController instance = null;
    //public int score;
    public int stageNum;                    //開始ステージ用
    public int continueNum;                 //コンテニューポイントがある場合

    //ライフ・コイン管理
    const int DefaultLife   = 3;            //ライフ初期値 08/27
    public int life         = DefaultLife;  //ライフ変動用 08/27
    //public PlayerController player;
    public int coin         = 0;

    //パネル処理をパネル側へ変更 08/28
    /*
    public LifePanel        lifepanel;
    public Text             timepanel;
    public Text             cointext;

    //時間表示関連
    private float second;
    private int   minute;
    private int   hour;
    */

    //ゲームオーバー関連
    [HideInInspector] public bool isGameOver   = false;

    //ゲームクリア関連
    [HideInInspector] public bool isStageClear = false;

    //BGM・SE関連
    //private AudioSource audioSource = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //以下パネル処理をパネル側処理へ変更 08/28
        /*
        //ライフ処理
        //lifepanel.UpdateLife(player.Life());
        lifepanel.UpdateLife(Life());

        //経過時間
        second += Time.deltaTime;

        if(minute > 60)
        {
            hour++;
            minute = 0;
        }
        if(second > 60f)
        {
            minute += 1;
            second = 0;
        }

        //timepanel.text = "" + Time.time;
        timepanel.text   = hour.ToString() + ":" + minute.ToString("00") + ":" + second.ToString("f2").PadLeft(5, '0');

        //コイン取得
        cointext.text    = "×" + coin.ToString("000");
        //以下パネル処理をパネル側処理へ変更　終了  08/28
        */

    }

    //ライフ管理をゲームコントローラーへ変更 08/27
    public int Life()
    {
        return life;
    }

    public int Coin()
    {
        return coin;
    }

    public void Addlife()
    {
        if(life <= 3)
        {
            ++life;
        }
    }

    public void Sublife()
    {
        if(life > 0)    //3回まで当たれる？3回目でOUT？
        {
            --life;
        }
        else
        {
            isGameOver = true;
        }
    }

    //即死エリア用 08/28
    public void Zerolife()
    {
        life = 0;
        isGameOver = true;
    }

    //ゲームリスタート
    public void RetryGame()
    {
        isGameOver  = false;
        life        = DefaultLife;
        coin        = 0; 
        stageNum    = 1;
        continueNum = 0;


    }

    /// SEを鳴らす
    /*
    public void PlaySE(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);

        }
        else
        {
            Debug.Log("オーディオソースが設定されていません");
        }
    }
    */

}
