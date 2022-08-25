using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sample_move : MonoBehaviour
{
    [SerializeField] private string nextstage = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetMouseButtonDown(0)){
            //遷移先のシーン名を入れる
            SceneManager.LoadScene(nextstage);
        }
        
    }
}
