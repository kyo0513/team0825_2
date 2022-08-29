using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //カメラはネコゲームから流用
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos=player.transform.position;
        transform.position=new Vector3(
            //transform.position.x,
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z
        );
        
    }
}
