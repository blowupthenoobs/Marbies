using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEditor;
using TMPro;


public class LobbyManagerScript : MonoBehaviourPunCallbacks
{
    public static string roomToJoin;


    [SerializeField] string[] Modes; //Perhaps turn this into a matrix so that I can have the diff modes give diff maps?
    [SerializeField] TMP_Text SelectedModeName;
    private int selectedModeIndex;
    [SerializeField] SceneAsset[] Maps; //Perhaps turn this into a matrix so that I can have the diff modes give diff maps?
    [SerializeField] TMP_Text SelectedMapName;
    private int selectedMapIndex;

    [SerializeField] GameObject[] buttons;

    [SerializeField] TMP_Text PlayerCountDisplay;
    public int playerCount;
    public static bool isHost;

    public static bool ConnectedToLobby;
    
    void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);

        if(!ConnectedToLobby)
            PhotonNetwork.JoinOrCreateRoom(roomToJoin, null, null);
        ChangeMap(0);

        Cursor.lockState = CursorLockMode.None;

        if(!isHost)
        {
            foreach(var button in buttons)
            {
                button.SetActive(false);
            }
        }
    }

    public void ChangeMode(int amount)
    {
        selectedModeIndex += amount;

        if(selectedModeIndex > Maps.Length)
        {
            while(selectedModeIndex > Maps.Length)
            {
                selectedModeIndex -= Maps.Length;
            }
        }
        else if(selectedModeIndex < 0)
        {
            while(selectedModeIndex < 0)
            {
                selectedModeIndex += Maps.Length;
            }
        }

        SelectedMapName.text = Maps[selectedMapIndex].name;

        PhotonView.Get(this).RPC("SyncMapsAndMode", RpcTarget.Others, selectedMapIndex, selectedModeIndex);
    }

    public void ChangeMap(int amount)
    {
        selectedMapIndex += amount;

        if(selectedMapIndex > Maps.Length)
        {
            while(selectedMapIndex > Maps.Length)
            {
                selectedMapIndex -= Maps.Length;
            }
        }
        else if(selectedMapIndex < 0)
        {
            while(selectedMapIndex < 0)
            {
                selectedMapIndex += Maps.Length;
            }
        }

        SelectedMapName.text = Maps[selectedMapIndex].name;

        PhotonView.Get(this).RPC("SyncMapAndMode", RpcTarget.Others, selectedMapIndex, selectedModeIndex);
    }

    [PunRPC]
    public void SyncMapsAndMode(int map, int mode)
    {
        selectedMapIndex = map;
        selectedModeIndex = mode;

        SelectedMapName.text = Maps[selectedMapIndex].name;
    }

    public void PressStartButton()
    {
        PhotonView.Get(this).RPC("StartGame", RpcTarget.All, selectedMapIndex, selectedModeIndex);
    }

    [PunRPC]
    public void StartGame(int map, int mode)
    {
        SceneManager.LoadScene(Maps[map].name);
    }

    [PunRPC]
    public void RequestCurrentMapAndMode()
    {
        if(isHost)
            PhotonView.Get(this).RPC("SyncMapsAndMode", RpcTarget.Others, selectedMapIndex, selectedModeIndex);
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            isHost = true;
            foreach(var button in buttons)
            {
                button.SetActive(true);
            }
            NetworkingManagerScript.Instance.playerIndex = 0;
        }
        else
        {
            isHost = false;

            PhotonView.Get(this).RPC("RequestCurrentMapAndMode", RpcTarget.Others);
            PhotonView.Get(NetworkingManagerScript.Instance).RPC("RequestPlayerIndex", RpcTarget.Others);
        }

        ConnectedToLobby = true;
    }

    public void Update()
    {
        if(ConnectedToLobby)
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        PlayerCountDisplay.text = (playerCount + "/16");
    }

}
