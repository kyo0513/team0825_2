using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerCheck : MonoBehaviour
{
    [HideInInspector] public bool isOn = false;

	private string playerTag = "Player";

	#region//接触判定
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == playerTag)
		{
            //Debug.Log("メッセージ範囲に入った");
			isOn = true;
		}
	
    }

    private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == playerTag)
		{
			isOn = false;
		}
	}
    #endregion
}