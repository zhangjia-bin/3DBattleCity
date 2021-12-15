using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Singleton<PlayerModel>
{
    public string playername;
    public string id;
    //销毁的id人，从集合里移除
    public int index;
}
