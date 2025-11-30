using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public Material[] defaultMaterialList;
    [SerializeField] GameObject marbiePrefab;
    [SerializeField] GameObject PlayerCamera;
    [SerializeField] PlayerID playerID;

    [SerializeField] Transform[] spawnPoints;
    public InputActionAsset inputs;

    private void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);

        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        
        string roomName = PlayerPrefs.GetString("roomToJoinOrCreate");

        PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
    }

    private void SetNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = "put name here";
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        GameObject player = PhotonNetwork.Instantiate(marbiePrefab.name, spawnPos, Quaternion.identity);
        player.GetComponent<PlayerMarbleScript>().GetPlayerData(playerID);
        PlayerCamera.GetComponent<PlayerCameraScript>().player = player;
    }


    
    public override void OnEnable()
    {
        base.OnEnable();
        inputs.FindActionMap("MarbleControls").Enable();
    }

    public void DisableInputSystem()
    {
        inputs.FindActionMap("MarbleControls").Disable();
    }
}
