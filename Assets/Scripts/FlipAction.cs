using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAction : MonoBehaviour
{
    //左からi番目、上からj番目の駒を裏返す
    //GameObject[] flipPieces:今から裏返そうとしている駒たち
    public bool FlipPiece(List<GameObject> flipPieces, float presentTime, float startTime, float flipTimePerPiece)
    {
        // 裏返す駒の数
        int flipPiecesNum = flipPieces.Count;
        float endTime = startTime + flipTimePerPiece * flipPiecesNum;
        // すべての裏返しが終わったらtrueを返す
        if (presentTime >= endTime)
        {
            return true;
        }

        // 実際に裏返す駒の番号
        int flippingPieceIndex = Mathf.FloorToInt((presentTime - startTime) / flipTimePerPiece);
        // 裏返す駒の、裏返し始める時間を取得
        float startTimeOfFlippingPiece = startTime + flippingPieceIndex * flipTimePerPiece;
        // 裏返す駒の、裏返し終わる時間を取得
        float endTimeOfFlippingPiece = startTimeOfFlippingPiece + flipTimePerPiece;
        // 裏返す駒のオブジェクトを取得
        GameObject flippingPiece = flipPieces[flippingPieceIndex];

        //駒のrotationを取得する
        Quaternion rotation = flippingPiece.transform.rotation;
        //クォータニオンからオイラー角への変換
        Vector3 rotationAngles = rotation.eulerAngles;
        //y軸中心に追加で180度回転
        rotationAngles.y += 180.0f * Time.deltaTime / flipTimePerPiece;
        // もしも裏返し終わっていたら、裏返し終わりの角度にする
        if (presentTime >= endTimeOfFlippingPiece)
        {
            rotationAngles.y = Mathf.Round(rotationAngles.y / 10) * 10;
        }
        //オイラー角からクォータニオンへの変換
        rotation = Quaternion.Euler(rotationAngles);
        //駒を回転
        flippingPiece.transform.rotation = rotation;

        return false;
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
