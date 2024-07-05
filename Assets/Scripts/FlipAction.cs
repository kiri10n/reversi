using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAction : MonoBehaviour
{
    //左からi番目、上からj番目の駒を裏返す
    //GameObject[] flipPieces:今から裏返そうとしている駒たち
    public bool FlipPiece(List<GameObject> flipPieces, int frame, int startFrame, int flipFramePerPiece)
    {
        int flipPiecesNum = flipPieces.Count;
        int flippingPieceIndex = (frame - startFrame) / flipFramePerPiece;
        if (flippingPieceIndex >= flipPiecesNum) return true;
        int startFrameOfFlippingPiece = startFrame + flippingPieceIndex * flipFramePerPiece;
        int endFrameOfFlippingPiece = startFrameOfFlippingPiece + flipFramePerPiece;
        GameObject flippingPiece = flipPieces[flippingPieceIndex];
        //駒のrotationを取得する
        Quaternion rotation = flippingPiece.transform.rotation;
        //クォータニオンからオイラー角への変換
        Vector3 rotationAngles = rotation.eulerAngles;
        //y軸中心に追加で180度回転
        rotationAngles.y += 180.0f / flipFramePerPiece;
        //rotationAngles += new Vector3(0.0f, 180.0f * (frame - startFrameOfFlippingPiece) / flipFramePerPiece, 0.0f);
        if (frame >= endFrameOfFlippingPiece)
        {
            rotationAngles.y = Mathf.Round(rotationAngles.y / 10) * 10;
        }
        //オイラー角からクォータニオンへの変換
        rotation = Quaternion.Euler(rotationAngles);
        //駒を回転
        flippingPiece.transform.rotation = rotation;

        // //裏返すことになっている駒たちから、一つずつ取り出して裏返す
        // foreach(GameObject piece in flipPieces)
        // {
        //     //駒のrotationを取得する
        //     Quaternion rotation = piece.transform.rotation;
        //     //クォータニオンからオイラー角への変換
        //     Vector3 rotationAngles = rotation.eulerAngles;
        //     //y軸中心に追加で180度回転
        //     rotationAngles += new Vector3(0.0f, 180.0f, 0.0f);
        //     //オイラー角からクォータニオンへの変換
        //     rotation = Quaternion.Euler(rotationAngles);
        //     //駒を回転
        //     piece.transform.rotation = rotation;

        // }
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
