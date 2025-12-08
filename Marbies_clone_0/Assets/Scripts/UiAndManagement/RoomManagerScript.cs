using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;


public class RoomManagerScript : MonoBehaviourPunCallbacks
{
    public static RoomManagerScript Instance;

    //Basic player variables
    public Material[] defaultMaterialList;
    [SerializeField] GameObject marbiePrefab;
    [SerializeField] GameObject PlayerCamera;
    [SerializeField] PlayerID playerID;

    [SerializeField] Transform[] spawnPoints;
    public InputActionAsset inputs;

    private bool isHost;

    //UI controls
    [SerializeField] GameObject ScoresContainer;
    [SerializeField] GameObject scoreTrackerPrefab;
    private Dictionary<int, GameObject> scoreTracker = new Dictionary<int, GameObject>();

    //Gameplay Stuff
    [SerializeField] GameObject pickableObject;

    public float deathCutoffHeight;
    public Dictionary<int, int> playerScores = new Dictionary<int, int>();


    // [System.Serializable] //Old idea, seeing if Dictionary by itself would work
    // public struct PlayerScoreContainer
    // {
    //     public int playerIndex;
    //     public int score;

    //     public PlayerScoreContainer(int playerIndex, int score)
    //     {
    //         this.playerIndex = playerIndex;
    //         this.score = score;
    //     }
    // }

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

        PhotonView.Get(this).RPC("AddScoreDictionaryEntry", RpcTarget.AllBuffered, NetworkingManagerScript.Instance.playerIndex, 0);
    }

    [PunRPC]
    public void AddScoreDictionaryEntry(int index, int startingScore)
    {
        playerScores.Add(index, startingScore);
        scoreTracker.Add(index, Instantiate(scoreTrackerPrefab, transform.position, Quaternion.identity));
        scoreTracker[index].transform.SetParent(ScoresContainer.transform);
        scoreTracker[index].GetComponent<ScoreDisplayerScript>().Initialize(index, "temp", playerScores[index]);
    }

    [PunRPC]
    public void EditScore(int index, int change)
    {
        playerScores[index] += change;
        scoreTracker[index].GetComponent<ScoreDisplayerScript>().UpdateScores(playerScores[index]);
    }

    public void ReorderScoreTrackers()
    {
        var orderedKeys = playerScores.OrderByDescending(entry => entry.Key).ToList();

        foreach(int key in orderedKeys)
        {
            
        }
        //transform.SetSeiblingIndex(num);
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
