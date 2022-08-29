using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//コメント//
//ゲームコントローラーからパネル更新全般をこちらに移動 08/28

public class LifePanel : MonoBehaviour
{
    public GameObject[] icons;
    [Header("ゲームコントローラーを設定")] public GameController   gameController;
    [Header("ライフパネルを設定")]   public LifePanel  lifepanel;
    [Header("タイマーパネルを設定")] public Text       timepanel;
    [Header("コインテキストを設定")] public Text       cointext;

    //時間表示関連
    private float second;
    private int   minute;
    private int   hour;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        UpdateLife((gameController.Life()));   //08/28

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
        //cointext.text    = "×" + coin.ToString("000");
        cointext.text = "x" + gameController.Coin().ToString("000");

        
    }

    public void UpdateLife(int life){
        //Debug.Log("ライフ処理");
        for(int i = 0; i< icons.Length;i++){
            if(i < life) icons[i].SetActive(true);
            else icons[i].SetActive(false);
        }
    }
}
