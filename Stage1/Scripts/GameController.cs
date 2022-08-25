using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerController player;
    public LifePanel        lifepanel;
    public Text             timepanel;

    //時間表示関連
    private float second;
    private int minute;
    private int hour;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ライフ処理
        lifepanel.UpdateLife(player.Life());

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


    }
}
