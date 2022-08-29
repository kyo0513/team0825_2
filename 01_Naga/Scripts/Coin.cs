using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("加算するスコア")]   public int mycoin;
    [Header("プレイヤーの判定")] public PlayerTriggerCheck playerCheck;

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが判定内に入ったら
        if (playerCheck.isOn)
        {
            if(GameController.instance != null)
            {
                GameController.instance.coin += mycoin;
                Destroy(this.gameObject);
            }
        }
    }  
}
