using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;

    //ゲームオーバー・クリア関連
    [Header("ゲームオーバー")] public GameObject gameOverObj;
    [Header("フェード")]       public FadeImage fade;
    [Header("ゲームオーバー時に鳴らすSE")] public AudioClip gameOverSE;
    [Header("リトライ時に鳴らすSE")] public AudioClip retrySE;
    [Header("ステージクリアーSE")]   public AudioClip stageClearSE;
    [Header("ステージクリア")]       public GameObject stageClearObj;
    [Header("ステージクリア判定")]   public PlayerTriggerCheck stageClearTrigger;

    private Player p;

    //ゲームオーバー&リトライ処理
    private int nextStageNum;

    [SerializeField] private string retrystage = "";   //お試しシーン遷移用追加 08/28
    [SerializeField] private string nextstage  = "";   //お試しシーン遷移用追加 08/28
    private string movestage;

    private bool startFade     = false;
    private bool doGameOver    = false;
    private bool retryGame     = false;
    private bool doSceneChange = false;
    private bool doClear       = false;
 
    void Start()
    {
        //if (playerObj != null && continuePoint != null && continuePoint.Length > 0)
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && gameOverObj != null && fade != null && stageClearObj != null)
        {
            gameOverObj.SetActive(false);  //ゲームオーバーは初期はオフ
            stageClearObj.SetActive(false);
            playerObj.transform.position = continuePoint[0].transform.position;
            p = playerObj.GetComponent<Player>();
            if(p == null)
            {
                Debug.Log("プレイヤーじゃない物がアタッチされているよ！");
            }
        }
        else
        {
        Debug.Log("設定が足りてないよ！");
        }
    }
 
    void Update()
    {
        //ゲームオーバー時
        if (GameController.instance.isGameOver && !doGameOver)
        {
            gameOverObj.SetActive(true);
            //GameController.instance.PlaySE(gameOverSE);
            doGameOver = true;
        }
        //プレイヤーがやられた時の処理
        //if(p != null && p.IsContinueWaiting())
        else if (p != null && p.IsContinueWaiting() && !doGameOver)
        {
            if(continuePoint.Length > GameController.instance.continueNum)
            {
                playerObj.transform.position = 
                continuePoint[GameController.instance.continueNum].transform.position;
                p.ContinuePlayer();
            }
            else
            {
                Debug.Log("コンティニューポイントの設定が足りてないよ！");
            }
        }
        //ステージクリア処理追加 08/28
        else if (stageClearTrigger != null && stageClearTrigger.isOn && !doGameOver && !doClear)
        {
            StageClear();
            doClear = true;
        }

        //ステージを切り替える
        if (fade != null && startFade && !doSceneChange)
        {
            if (fade.IsFadeOutComplete())
            {
                //ゲームリトライ
                if (retryGame)
                {
                    GameController.instance.RetryGame();
                    movestage = retrystage;
                }
                //次のステージ
                else
                {
                    GameController.instance.stageNum = nextStageNum;
                    movestage = nextstage;
                }
                GameController.instance.isStageClear = false;
                //SceneManager.LoadScene("stage" + nextStageNum);
                SceneManager.LoadScene(movestage);
                doSceneChange = true;
            }
        }
    }

    /// </summary>
    public void Retry()
    {
        //GameController.instance.PlaySE(retrySE);
        ChangeScene(1);      //最初のステージに戻るので１
        retryGame = true;
    }

    // ステージを切り替えます。
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num)
    {
        if (fade != null)
        {
            nextStageNum = num;
            fade.StartFadeOut();
            startFade = true;
        }
    }

    public void StageClear()
    {
        GameController.instance.isStageClear = true;
        stageClearObj.SetActive(true);
        //GameController.instance.PlaySE(stageClearSE);
    }
  
}
