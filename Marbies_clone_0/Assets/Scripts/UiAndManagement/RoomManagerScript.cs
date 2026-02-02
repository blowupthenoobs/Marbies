using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using UnityEngine.UIElements.Experimental;
using System;
using TMPro;


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
    [SerializeField] TMP_Text timerDisplay;
    public float gameTimer = 600;

    private void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);
        else
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            // PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
            NetworkingManagerScript.Instance.AllPlayersReady += SpawnPlayer;
            NetworkingManagerScript.Instance.AllPlayersReady += () => PhotonView.Get(this).RPC("AddScoreDictionaryEntry", RpcTarget.AllBuffered, NetworkingManagerScript.Instance.playerIndex, "temp" + NetworkingManagerScript.Instance.playerIndex, 0);
            PhotonView.Get(NetworkingManagerScript.Instance).RPC("SignalJoin", RpcTarget.AllBuffered);

            isHost = LobbyManagerScript.isHost;

            if(isHost)
            {
                Invoke("SpawnCollectables", 5);
                Invoke("SpawnCollectables", 5);
                Invoke("SpawnCollectables", 5);
            }

            // PlayerID.Instance.player.materialIndex = UnityEngine.Random.Range(0, RoomManagerScript.Instance.defaultMaterialList.Length);

        }

    }

    void Update()
    {
        gameTimer -= Time.deltaTime;
        var minutes = ((int)gameTimer)/60;
        var seconds  = (int)(gameTimer - (minutes * 60));

        if(seconds >= 10)
            timerDisplay.text = minutes + ":" + seconds;
        else
            timerDisplay.text = minutes + ":0" + seconds;

    }

    [PunRPC]
    public void AddScoreDictionaryEntry(int index, string name, int startingScore)
    {
        playerScores.Add(index, startingScore);
        scoreTracker.Add(index, Instantiate(scoreTrackerPrefab, transform.position, Quaternion.identity));
        scoreTracker[index].transform.SetParent(ScoresContainer.transform);
        scoreTracker[index].GetComponent<ScoreDisplayerScript>().Initialize(index, name, playerScores[index]);
    }

    [PunRPC]
    public void EditScore(int index, int change)
    {
        playerScores[index] += change;
        scoreTracker[index].GetComponent<ScoreDisplayerScript>().UpdateScores(playerScores[index]);

        ReorderScoreTrackers();
    }

    public void ReorderScoreTrackers()
    {
        var orderedKeys = new List<int>(playerScores.Values).OrderByDescending(score => score);
        // .OrderByDescending(entry => entry.Key).ToList();

        foreach(int key in orderedKeys)
        {
            scoreTracker[playerScores.FirstOrDefault(x => x.Value == key).Key].transform.SetAsLastSibling();
        }
        //transform.SetSeiblingIndex(num);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPos = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;

        GameObject player = PhotonNetwork.Instantiate(marbiePrefab.name, spawnPos, Quaternion.identity);
        player.GetComponent<PlayerMarbleScript>().GetPlayerData(PlayerID.Instance);
        PlayerCamera.GetComponent<PlayerCameraScript>().player = player;
    }

    public void SpawnCollectables()
    {
        //using temp code atm
        PhotonNetwork.Instantiate(pickableObject.name, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
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
