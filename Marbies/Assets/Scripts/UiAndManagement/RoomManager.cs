using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;


public class RoomManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);

        string roomName = PlayerPrefs.GetString("roomToJoinOrCreate");

        PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
    }
}
