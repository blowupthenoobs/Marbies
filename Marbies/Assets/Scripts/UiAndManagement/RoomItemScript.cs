using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemScript : MonoBehaviour
{
    public string roomToJoin;

    public void JoinRoom()
    {
        MainMenuScript.Instance.JoinRoom(roomToJoin);
    }
}
