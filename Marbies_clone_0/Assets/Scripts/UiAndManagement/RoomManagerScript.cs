using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class RoomManagerScript : MonoBehaviourPunCallbacks
{
    public static RoomManagerScript Instance;
    public Material[] defaultMaterialList;
    [SerializeField] GameObject marbiePrefab;
    [SerializeField] GameObject PlayerCamera;
    [SerializeField] PlayerID playerID;

    [SerializeField] Transform[] spawnPoints;
    public InputActionAsset inputs;

    private bool isHost;

    [SerializeField] GameObject pickableObject;

    public float deathCutoffHeight;


    private void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);

        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
        NetworkingManagerScript.Instance.AllPlayersReady += SpawnPlayer;
        PhotonView.Get(NetworkingManagerScript.Instance).RPC("SignalJoin", RpcTarget.AllBuffered);

        isHost = LobbyManagerScript.isHost;

        if(isHost)
        {
            Invoke("SpawnCollectables", 5);
            Invoke("SpawnCollectables", 5);
            Invoke("SpawnCollectables", 5);
        }

        playerID.materialIndex = Random.Range(0, RoomManagerScript.Instance.defaultMaterialList.Length);
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        GameObject player = PhotonNetwork.Instantiate(marbiePrefab.name, spawnPos, Quaternion.identity);
        player.GetComponent<PlayerMarbleScript>().GetPlayerData(playerID);
        PlayerCamera.GetComponent<PlayerCameraScript>().player = player;
    }

    public void SpawnCollectables()
    {
        //using temp code atm
        PhotonNetwork.Instantiate(pickableObject.name, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
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
