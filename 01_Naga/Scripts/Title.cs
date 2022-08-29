using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [Header("フェード")] public FadeImage fade;
    [SerializeField] [Header("次のステージ名")]private string nextstage = "";

    private bool firstPush   = false;
    private bool goNextScene = false;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if (!firstPush)
            {
                fade.StartFadeOut();
                firstPush = true;
            }
        }

        if (!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene(nextstage);
            goNextScene = true;
        }
    }
 
}
