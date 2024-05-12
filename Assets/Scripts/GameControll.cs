using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//定数を定義
//Constants.BLANKのように使用
public static class Constants
{
    public const int BLANK = 0;
    public const int BLACK = 1;
    public const int WHITE = 2;
    public const int WALL  = 3;
}

/*
 ++++++++++
 +        +       11 12 ... 18         (-3.5 -2.5 -1.5 -0.5 0.5 1.5 2.5 3.5,  3.5)
 +        +       21 22 ... 28         (-3.5 -2.5 -1.5 -0.5 0.5 1.5 2.5 3.5,  2.5)
     ...                          →    
 +        +       81 82 ... 88         (-3.5 -2.5 -1.5 -0.5 0.5 1.5 2.5 3.5, -3.5)
 ++++++++++

*/



public class GameControll : MonoBehaviour
{
    public GameObject piecePrefab;  //駒のプレハブ
    //盤面用の配列を用意
    public static int[] ban = new int[100];  
    public static int MINE, COM;

    public GameObject[] allPieces;  //盤上の全ての駒
    public List<GameObject> flipPieces = new List<GameObject>(); //裏返そうとしている駒
    
    //GameDirectorクラス（スクリプト）を取得
    GameDirector directorComponent;
    //FlipActionクラス（スクリプト）を取得
    FlipAction flipActionComponent;

    //黒・白の数を表示
    public GameObject countTextObject;
    //どちらのターンなのかを表示
    public GameObject turnTextObject;

    //左からi番目、上からj番目の盤面に置く
    public void put_ban(int i, int j, int color)
    {
        //banを操作
        ban[j * 10 + i] = color;

        //黒または白の駒を置いたときは、プレハブを生成する
        switch(color)
        {
            case Constants.BLANK: return;
            case Constants.BLACK: break;
            case Constants.WHITE: break;
            case Constants.WALL: return;
        }

        float x = (float)(i - 4.5);
        float y = (float)(-j + 4.5);
        
        //プレハブをゲームオブジェクトとして生成
        GameObject piece = Instantiate(piecePrefab) as GameObject;
        piece.transform.position = new Vector3(x, y, 0);
        //Quaternion.Euler：オイラー角からクォータニオンへの変換
        if(color==Constants.BLACK)
            piece.transform.rotation = Quaternion.Euler(-90, -180 , 0);
        else //(color==Constants.WHITE)
            piece.transform.rotation = Quaternion.Euler(-90, 0 , 0);

    }

    //盤面の初期化
    void ban_init()
    {
        int i, j;
        //まず壁ですべて埋める
        for(i=0;i<100;i++)
        {
            ban[i] = Constants.WALL;
        }

        //内側の8*8の部分を、何もない状態にする
        for(i=1;i<9;i++)
        {
            for(j=1;j<9;j++)
            {
                ban[j * 10 + i] = Constants.BLANK;
            }
        }

        //中心の4マスに駒を配置
        put_ban(4, 4, Constants.WHITE);
        put_ban(4, 5, Constants.BLACK);
        put_ban(5, 4, Constants.BLACK);
        put_ban(5, 5, Constants.WHITE);

    }
    
    //左からi番目、上からj番目の駒をflipPiecesに登録する
    void registerToFlipPieces(int i, int j)
    {
        int I = 0, J = 0; //注目しているpieceが左からI番目、上からJ番目
        foreach(GameObject piece in allPieces)
        {
            //駒のx座標、y座標を確認（小数第二位を四捨五入）
            float x = Mathf.RoundToInt(piece.transform.position.x * 10.0f) / 10.0f;
            float y = Mathf.RoundToInt(piece.transform.position.y * 10.0f) / 10.0f;
            //盤の座標から、左からI番目、上からJ番目という情報に変換
            switch (x)
            {
                case (float)-3.5: I = 1; break;
                case (float)-2.5: I = 2; break;
                case (float)-1.5: I = 3; break;
                case (float)-0.5: I = 4; break;
                case (float)0.5: I = 5; break;
                case (float)1.5: I = 6; break;
                case (float)2.5: I = 7; break;
                case (float)3.5: I = 8; break;
                default: break;
            }
            switch (y)
            {
                case (float)-3.5: J = 8; break;
                case (float)-2.5: J = 7; break;
                case (float)-1.5: J = 6; break;
                case (float)-0.5: J = 5; break;
                case (float)0.5: J = 4; break;
                case (float)1.5: J = 3; break;
                case (float)2.5: J = 2; break;
                case (float)3.5: J = 1; break;
                default: break;
            }
            //左からi番目、上からj番目の駒が今まさに注目している駒のとき
            if (i==I && j==J)
            {
                //登録
                flipPieces.Add(piece);
            }
        }
    }

    //(a,b)に黒石を置いたとき、dirの方向にする処理
    //返り値は、その方向で返した石の数
    int Flip_line_black(int a, int b, int dir)
    {
        int p = b * 10 + a;
        int i = 0, n = p + dir;

        while (ban[n] == Constants.WHITE)
            n += dir;

        if (ban[n] != Constants.BLACK)
            return 0;

        n -= dir;
        while (n != p)
        {
            ban[n] = Constants.BLACK;
            //黒にした部分の駒を「裏返そうとしている駒たち」に登録する
            //nの値から、左からi番目、上からj番目という数に変換して引数へ
            registerToFlipPieces(n % 10, n / 10);
            n -= dir;
            i++;
        }
        return i;
    }

    //(a,b)に黒石を置いたとき、周りの適切な石を裏返す処理
    //返り値は、返したすべての石の数（0の時、(a,b)に黒石は置けない）
    public int Flip_black(int a, int b)
    {
        int i = 0, p = b * 10 + a;

        allPieces = GameObject.FindGameObjectsWithTag("Piece");

        i += Flip_line_black(a, b, -11);
        i += Flip_line_black(a, b, -10);
        i += Flip_line_black(a, b, -9);
        i += Flip_line_black(a, b, -1);
        i += Flip_line_black(a, b, 1);
        i += Flip_line_black(a, b, 9);
        i += Flip_line_black(a, b, 10);
        i += Flip_line_black(a, b, 11);

        //何か裏返したとき（選んだマスが、置いてもよいマスだったとき
        if (i > 0)
            ban[p] = Constants.BLACK;

        return i;
    }

    //(a,b)に白石を置いたとき、dirの方向にする処理
    //返り値は、その方向で返した石の数
    int Flip_line_white(int a, int b, int dir)
    {
        int p = b * 10 + a;
        int i = 0, n = p + dir;

        while (ban[n] == Constants.BLACK)
            n += dir;

        if (ban[n] != Constants.WHITE)
            return 0;

        n -= dir;
        while (n != p)
        {
            ban[n] = Constants.WHITE;
            //白にした部分の駒を「裏返そうとしている駒たち」に登録する
            //nの値から、左からi番目、上からj番目という数に変換して引数へ
            registerToFlipPieces(n % 10 , n / 10);
            n -= dir;
            i++;
        }
        return i;
    }

    //(a,b)に白石を置いたとき、周りの適切な石を裏返す処理
    //返り値は、返したすべての石の数（0の時、(a,b)に白石は置けない）
    public int Flip_white(int a, int b)
    {
        int i = 0, p = b * 10 + a;

        allPieces = GameObject.FindGameObjectsWithTag("Piece");

        i += Flip_line_white(a, b, -11);
        i += Flip_line_white(a, b, -10);
        i += Flip_line_white(a, b, -9);
        i += Flip_line_white(a, b, -1);
        i += Flip_line_white(a, b, 1);
        i += Flip_line_white(a, b, 9);
        i += Flip_line_white(a, b, 10);
        i += Flip_line_white(a, b, 11);

        //何か裏返したとき（選んだマスが、置いてもよいマスだったとき
        if (i > 0)
            ban[p] = Constants.WHITE;

        return i;
    }

    //(a,b)に黒石を置いたとき、dirの方向は何枚駒が返る予定か
    //実際には裏返さないが、裏返すとしたら何枚裏返すかを返り値とする
    int Can_Flip_line_black(int a, int b, int dir)
    {
        int p = b * 10 + a;
        int i = 0, n = p + dir;

        //進んだ先が白の間は進み続ける
        while (ban[n] == Constants.WHITE)
        {
            n += dir;
            //並んでいる白駒の数をカウント
            i++;
        }

        //白が並んだ先が黒以外であれば、何も返せない
        if (ban[n] != Constants.BLACK)
            return 0;
        else //白が並んだ先に黒があれば、並んだ白の数を返す
            return i;
    }

    //仮に(a,b)に黒石を置いたとき、白は何枚裏返るか
    int Can_Flip_black(int a, int b)
    {
        int i = 0, p = b * 10 + a;
        
        i += Can_Flip_line_black(a, b, -11);
        i += Can_Flip_line_black(a, b, -10);
        i += Can_Flip_line_black(a, b, -9);
        i += Can_Flip_line_black(a, b, -1);
        i += Can_Flip_line_black(a, b, 1);
        i += Can_Flip_line_black(a, b, 9);
        i += Can_Flip_line_black(a, b, 10);
        i += Can_Flip_line_black(a, b, 11);
        
        return i;
    }

    //(a,b)に白石を置いたとき、dirの方向は何枚駒が返る予定か
    //実際には裏返さないが、裏返すとしたら何枚裏返すかを返り値とする
    int Can_Flip_line_white(int a, int b, int dir)
    {
        int p = b * 10 + a;
        int i = 0, n = p + dir;

        //進んだ先が黒の間は進み続ける
        while (ban[n] == Constants.BLACK)
        {
            n += dir;
            //並んでいる黒石の数をカウント
            i++;
        }

        //黒が並んだ先が白以外であれば、何も返せない
        if (ban[n] != Constants.WHITE)
            return 0;
        else //黒が並んだ先に白があれば、並んだ黒の数を返す
            return i;
    }

    //仮に(a,b)に白石を置いたとき、黒は何枚裏返るか
    int Can_Flip_white(int a, int b)
    {
        int i = 0, p = b * 10 + a;

        i += Can_Flip_line_white(a, b, -11);
        i += Can_Flip_line_white(a, b, -10);
        i += Can_Flip_line_white(a, b, -9);
        i += Can_Flip_line_white(a, b, -1);
        i += Can_Flip_line_white(a, b, 1);
        i += Can_Flip_line_white(a, b, 9);
        i += Can_Flip_line_white(a, b, 10);
        i += Can_Flip_line_white(a, b, 11);

        return i;
    }

    //今現在の盤において、プレイヤーは駒を置くことができるか？
    public bool Can_Player_put()
    {
        int i, j;

        //全てのマスについて調べる。
        for(i=0;i<10;i++)
        {
            for(j=0;j<10;j++)
            {
                //空きマスだけをチェック
                if(ban[j*10+i]==Constants.BLANK)
                {
                    if(MINE==Constants.BLACK)
                    {
                        //黒を置く場所が一つでもあるならtrueを返す
                        if (Can_Flip_black(i, j) > 0)
                            return true;
                    }
                    else //(MINE==WHITE)
                    {
                        //白を置く場所が一つでもあるならtrueを返す
                        if (Can_Flip_white(i, j) > 0)
                            return true;
                    }
                }
            }
        }
        //全てのマスを調べても置く場所がなかったのでfalse
        return false;
    }

    //今現在の盤において、プレイヤーは駒を置くことができるか？
    public bool Can_Com_put()
    {
        int i, j;

        //全てのマスについて調べる。
        for (i = 0; i < 10; i++)
        {
            for (j = 0; j < 10; j++)
            {
                //空きマスだけをチェック
                if (ban[j * 10 + i] == Constants.BLANK)
                {
                    if (COM == Constants.BLACK)
                    {
                        //黒を置く場所が一つでもあるならtrueを返す
                        if (Can_Flip_black(i, j) > 0)
                            return true;
                    }
                    else //(MINE==WHITE)
                    {
                        //白を置く場所が一つでもあるならtrueを返す
                        if (Can_Flip_white(i, j) > 0)
                            return true;
                    }
                }
            }
        }
        //全てのマスを調べても置く場所がなかったのでfalse
        return false;
    }

    //相手のターンの処理
    //駒を置いたらtrueを返す
    public bool ComTurn()
    {
        int a, b;

        //1から10000までの乱数を発生させ、整数にした後8で割った余りを求める
        //つまりa, bは1から8までの値を取りうる
        a = Mathf.RoundToInt(Random.Range(1, 10000)) % 8 + 1;
        b = Mathf.RoundToInt(Random.Range(1, 10000)) % 8 + 1;

        if (ban[b * 10 + a] != Constants.BLANK)
        {
            return false;
        }

        if (COM == Constants.BLACK)
        {
            if (Flip_black(a, b) > 0)
            {
                //(i, j)に駒を置く
                put_ban(a, b, Constants.BLACK);
                //自分のターンを終わる
                return true;
            }
        }
        else
        { //(COM==WHITE)
            if (Flip_white(a, b) > 0)
            {
                //(i, j)に駒を置く
                put_ban(a, b, Constants.WHITE);
                //自分のターンを終わる
                return true;
            }
        }
        return false;
    }

    //盤面の黒石の数を数える
    int CountBlack()
    {
        int sum = 0;
        for(int i =0;i<100;i++)
        {
            if (ban[i] == Constants.BLACK)
                sum++;
        }
        return sum;
    }

    //盤面の白石の数を数える
    int CountWhite()
    {
        int sum = 0;
        for (int i = 0; i < 100; i++)
        {
            if (ban[i] == Constants.WHITE)
                sum++;
        }
        return sum;
    }

    //次のターンがどちらなのかを引数に受け取る
    //next=0:どちらでもない、next=1:プレイヤー、next=2:COM
    public void ChangeText(int next)
    {
        Text countText = countTextObject.GetComponent<Text>();
        Text turnText = turnTextObject.GetComponent<Text>();

        int black = CountBlack();
        int white = CountWhite();

        if (next == 0)
        {
            //ゲーム終了
            if (MINE == Constants.BLACK)
            {
                //ToString("00")で、必ず二桁で表示するようにする
                countText.text = "黒：" + black.ToString("00") +
                    "\n白：" + white.ToString("00");
                if (black > white)
                    turnText.text = "あなたの\n勝ちです！";
                else if (black < white)
                    turnText.text = "あなたの\n負けです。";
                else //(black===white)
                    turnText.text = "引き分け\nです。";
            }
            else //(MINE==WHITE)
            {
                //ToString("00")で、必ず二桁で表示するようにする
                countText.text = "黒：" + black.ToString("00") +
                    "\n白：" + white.ToString("00");
                if (black > white)
                    turnText.text = "あなたの\n負けです。";
                else if (black < white)
                    turnText.text = "あなたの\n勝ちです！";
                else //(black===white)
                    turnText.text = "引き分け\nです。";
            }
        }
        else if(next==1)
        {
            //次はプレイヤーのターン
            //ToString("00")で、必ず二桁で表示するようにする
            countText.text = "黒：" + black.ToString("00") +
                "\n白：" + white.ToString("00");
            turnText.text = "あなたの\nターン";
        }
        else //(next==2)
        {
            //次はCOMのターン
            //ToString("00")で、必ず二桁で表示するようにする
            countText.text = "黒：" + black.ToString("00") +
                "\n白：" + white.ToString("00");
            turnText.text = "相手の\nターン";
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //取得できなかった場合にはエラーを出す
        directorComponent = gameObject.GetComponent<GameDirector>();
        if (directorComponent == null)
        {
            Debug.LogError("directorComponent is not found.");
        }
        flipActionComponent = gameObject.GetComponent<FlipAction>();
        if (flipActionComponent == null)
        {
            Debug.LogError("flipActionComponent is not found.");
        }

        //ゲームの初期化
        //盤の初期化
        ban_init();

        //手動で色を設定
        MINE = Constants.BLACK;
        COM = Constants.WHITE;
        
        switch (MINE)
        {
            case Constants.BLACK: Debug.Log("You are BLACK."); break;
            case Constants.WHITE: Debug.Log("You are WHITE."); break;
        }

        //プレイヤーのターンを開始
        //StartCoroutine(MyTurn());
        directorComponent.mode = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
