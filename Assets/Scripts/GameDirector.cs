using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    //GameControllクラス（スクリプト）を取得
    GameControll controllComponent;
    //PieceGeneratorクラス（スクリプト）を取得
    PieceGenerator generatorComponent;
    //FlipActionクラス（スクリプト）を取得
    FlipAction flipActionComponent;

    bool loopFrag = false;
    bool endFrag = false; //ゲームの終了フラグ
    public int mode = 0;
    int next = 0; //next=0:どちらでもない、next=1:プレイヤー、next=2:COM
    float flipStartTime; //裏返りの開始時間
    float flipTimePerPiece = 0.3f; //裏返りの時間

    // Start is called before the first frame update
    void Start()
    {
        //取得できなかった場合にはエラーを出す
        controllComponent = gameObject.GetComponent<GameControll>();
        if (controllComponent == null)
        {
            Debug.LogError("controllComponent is not found.");
        }
        //取得できなかった場合にはエラーを出す
        generatorComponent = gameObject.GetComponent<PieceGenerator>();
        if (generatorComponent == null)
        {
            Debug.LogError("generatorComponent is not found.");
        }
        flipActionComponent = gameObject.GetComponent<FlipAction>();
        if (flipActionComponent == null)
        {
            Debug.LogError("flipActionComponent is not found.");
        }

        //ループの開始（Update関数の中）
        loopFrag = true;
    }

    // Update is called once per frame
    void Update()
    {
        //loopFragがfalseのときは、これより下の処理はしない
        if (!loopFrag) return;
        switch(mode)
        {
            case 0:
                //プレイヤーも相手も操作しない
                break;
            case 1:
                //プレイヤーのターン
                //プレイヤーが駒を置いたらmodeを一つ進める
                if(generatorComponent.Playing()){
                    flipStartTime = Time.time;
                    mode++;
                }
                break;
            case 2:
                //プレイヤーの手による、駒の裏返りの時間
                //裏返す駒を実際に裏返す
                if(flipActionComponent.FlipPiece(controllComponent.flipPieces, Time.time, flipStartTime, flipTimePerPiece)){
                    //裏返し終わったので、今から裏返そうとしている駒たちのリストの要素を削除
                    controllComponent.flipPieces.Clear();
                    //modeを一つ進める
                    mode++;
                }
                break;
            case 3:
                //詰み判定
                if (controllComponent.Can_Com_put())
                {
                    //COMが置く場所が一つでもあるなら次はCOMのターン
                    mode++;
                    next = 2;
                }
                else if (controllComponent.Can_Player_put())
                {
                    //COMは置けないがプレイヤーが置けるならプレイヤーのターン
                    mode = 1;
                    next = 1;
                }
                else
                {
                    //どちらも置けないならゲーム終了
                    endFrag = true;
                    next = 0;
                }
                //ついでにテキストを変更
                controllComponent.ChangeText(next);
                break;
            case 4:
                //相手のターン
                if(controllComponent.ComTurn()){
                    flipStartTime = Time.time;
                    mode++;
                }
                break;
            case 5:
                //相手の手による、駒の裏返りの時間
                //裏返す駒を実際に裏返す
                if(flipActionComponent.FlipPiece(controllComponent.flipPieces, Time.time, flipStartTime, flipTimePerPiece)){
                    //裏返し終わったので、今から裏返そうとしている駒たちのリストの要素を削除
                    controllComponent.flipPieces.Clear();
                    //modeを一つ進める
                    mode++;
                }
                break;
            case 6:
                //詰み判定
                if (controllComponent.Can_Player_put())
                {
                    //プレイヤーが置く場所が一つでもあるなら次はプレイヤーのターン
                    mode = 1;
                    next = 1;
                }
                else if (controllComponent.Can_Com_put())
                {
                    //プレイヤーは置けないがCOMが置けるならCOMのターン
                    mode = 4;
                    next = 2;
                }
                else
                {
                    //どちらも置けないならゲーム終了
                    endFrag = true;
                    next = 0;
                }
                //ついでにテキストを変更
                controllComponent.ChangeText(next);
                break;
        }
        //ゲーム終了
        if (endFrag)
        {
            loopFrag = false;
        }
    }
}
