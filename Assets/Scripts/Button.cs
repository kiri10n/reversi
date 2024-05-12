using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    //Quitボタンをクリックしたときの処理
    public void OnClickQuit()
    {
        Application.Quit();
    }

    //Loadボタンをクリックしたときの処理
    public void OnClickLoad()
    {
        //シーンを再読み込み
        SceneManager.LoadScene("GameScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
