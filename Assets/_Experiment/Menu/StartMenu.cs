using System.Collections;
using System.Collections.Generic;
using SLNet;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void OnButton_Connect()
    {
        NetworkController.Instance.Connect("127.0.0.1", 8888, "SLikeNet");
    }
    
    public void OnButton_Ready()
    {
        var obj = new GameReadyRes()
        {
            Ready = true,
        };

        NetworkController.Instance.SendObjectRaw((byte)DefaultMessageIDTypes.ID_USER_PLAYER_READY_REQ, obj);
    }
}
