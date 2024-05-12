using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    //gameControllのGameControllクラス（スクリプト）を取得
    GameControll controllComponent;
    //gameDirectorのGameDirectorクラス（スクリプト）を取得
    GameDirector directorComponent;

    int i, j, k;


    //マウスの座標に対応した駒を置く座標を返す
    //0が返った時は置けない（盤外）
    double MouseToBan(double a)
    {
        if (a >= -4 && a <= -3)
            return -3.5;
        else if (a >= -3 && a <= -2)
            return -2.5;
        else if (a >= -2 && a <= -1)
            return -1.5;
        else if (a >= -1 && a <= 0)
            return -0.5;
        else if (a >= 0 && a <= 1)
            return 0.5;
        else if (a >= 1 && a <= 2)
            return 1.5;
        else if (a >= 2 && a <= 3)
            return 2.5;
        else if (a >= 3 && a <= 4)
            return 3.5;
        else
            return 0.0;
    }

    //盤の座標から、左からi番目、上からj番目という情報に変換
    void BanToIJ(float x, float y)
    {
        switch (x)
        {
            case (float)-3.5: i = 1; break;
            case (float)-2.5: i = 2; break;
            case (float)-1.5: i = 3; break;
            case (float)-0.5: i = 4; break;
            case (float) 0.5: i = 5; break;
            case (float) 1.5: i = 6; break;
            case (float) 2.5: i = 7; break;
            case (float) 3.5: i = 8; break;
            default: break;
        }
        switch (y)
        {
            case (float)-3.5: j = 8; break;
            case (float)-2.5: j = 7; break;
            case (float)-1.5: j = 6; break;
            case (float)-0.5: j = 5; break;
            case (float) 0.5: j = 4; break;
            case (float) 1.5: j = 3; break;
            case (float) 2.5: j = 2; break;
            case (float) 3.5: j = 1; break;
            default: break;
        }
    }

    //プレイヤーが操作
    //プレイヤーが駒を置いたらtrueを返す
    public bool Playing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                float x = (float)MouseToBan(hit.point.x);
                float y = (float)MouseToBan(hit.point.y);

                //i,jを用意
                BanToIJ(x, y);

                //盤外が選択されたわけでないとき
                if (x != 0.0 && y != 0.0)
                {
                    //駒があるところ、もしくは壁のところに置こうとしたとき
                    if (GameControll.ban[j * 10 + i] != Constants.BLANK)
                        Debug.Log("そこには置けません。");
                    else if (GameControll.MINE == Constants.BLACK)
                    {
                        if (controllComponent.Flip_black(i, j) > 0)
                        {
                            //(i, j)に駒を置く
                            controllComponent.put_ban(i, j, Constants.BLACK);
                            //自分のターンを終わる
                            return true;
                        }
                        else Debug.Log("そこには置けません。");
                    }
                    else
                    { //(MINE==WHITE)
                        if (controllComponent.Flip_white(i, j) > 0)
                        {
                            //(i, j)に駒を置く
                            controllComponent.put_ban(i, j, Constants.WHITE);
                            //自分のターンを終わる
                            return true;
                        }
                        else Debug.Log("そこには置けません。");
                    }
                }
            }
        }
        return false;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        //取得できなかった場合にはエラーを出す
        controllComponent = gameObject.GetComponent<GameControll>();
        if (controllComponent == null)
        {
            Debug.LogError("gameComponent is not found.");
        }

        //取得できなかった場合にはエラーを出す
        directorComponent = gameObject.GetComponent<GameDirector>();
        if (directorComponent == null)
        {
            Debug.LogError("directorComponent is not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
