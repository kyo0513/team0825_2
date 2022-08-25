using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{    
    [Header("これを踏んだ時のプレイヤーが跳ねる高さ")]public float boundHeight;

    [HideInInspector]public bool playerStepOn;
    
}
