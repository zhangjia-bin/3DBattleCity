using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


using Google.Protobuf;
using PlayerIn;

public class StartGame : MonoBehaviour
{
    public InputField field;
    public Button button;
    void Start()
    {
        //进去游戏告诉客户端有人加入
        button.onClick.AddListener(()=> {

            player player = new player();
            player.Playername = field.text;
            //生成的位置赋值还有生成的角度赋值.....
            PlayerModel.Ins.playername = field.text;
            //告诉其他客户端我上线
            NetManage.Ins.OnClientSendToSever(MessageID.C2S_BeginGame,player.ToByteArray());
            //跳转场景！！
            SceneManager.LoadScene(1);
        });
    }

    void Update()
    {
        
    }
}
