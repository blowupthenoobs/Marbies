using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Events;
using Photon.Realtime;


public class NetworkingManagerScript : MonoBehaviourPunCallbacks
{
    public static NetworkingManagerScript Instance;
    public UnityAction AllPlayersReady;

    private int playersJoined;
    public string nickName = "put random string here";
    void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [PunRPC]
    public void SignalJoin()
    {
        playersJoined++;
        Debug.Log(playersJoined);

        if(playersJoined == PhotonNetwork.CurrentRoom.PlayerCount)
            PhotonView.Get(this).RPC("OnAllPlayersReady", RpcTarget.All);
    }

    [PunRPC]
    public void OnAllPlayersReady()
    {
        AllPlayersReady?.Invoke();
        AllPlayersReady = null;   
    }

    private void SetNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = "put name here";
    }

    public static void OpenScene(string scene)
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        
        SceneManager.LoadScene(scene);
    }
}
