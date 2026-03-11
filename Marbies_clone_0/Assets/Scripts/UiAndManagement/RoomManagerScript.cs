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
using ExitGames.Client.Photon.StructWrapping;


public class RoomManagerScript : MonoBehaviourPunCallbacks
{
    public static RoomManagerScript Instance;

    [Header("Basic player variables")]
    [SerializeField] GameObject marbiePrefab;
    [SerializeField] GameObject PlayerCamera;

    [SerializeField] Transform[] spawnPoints;
    public InputActionAsset inputs;

    [HideInInspector] public bool isHost;
    private List<string> usedNames = new List<string>();

    [Header("UI Menus")]
    [SerializeField] GameObject GameUi;
    [SerializeField] GameObject EndGameScreen;

    [Header("UI controls")]
    [SerializeField] GameObject ScoresContainer;
    [SerializeField] GameObject scoreTrackerPrefab;
    private Dictionary<int, GameObject> scoreTracker = new Dictionary<int, GameObject>();
    public Dictionary<int, int> playerScores = new Dictionary<int, int>();
    [SerializeField] TMP_Text timerDisplay;
    [SerializeField] GameObject FinalScoreDisplaysContainer;
    [SerializeField] GameObject FinalScoringDisplayPrefab;

    [Header("Gameplay Stuff")]
    [SerializeField] GameObject[] pointSpawnerCluster;

    public float deathCutoffHeight;
    public float gameTimer = 90; //was originally 600, probably a good idea to make it lower

    private bool gameEnded = false;

    private void Awake()
    {
        Time.timeScale = 1;

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
            NetworkingManagerScript.Instance.AllPlayersReady += () => PhotonView.Get(this).RPC("AddScoreDictionaryEntry", RpcTarget.AllBuffered, NetworkingManagerScript.Instance.playerIndex, GetUniqueUsername(), 0);
            PhotonView.Get(NetworkingManagerScript.Instance).RPC("SignalJoin", RpcTarget.AllBuffered);

            isHost = LobbyManagerScript.isHost;

            if(isHost)
            {
                Invoke("SpawnPointPickups", 5);
            }

            SetNormalGameUI();
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
        
        if(isHost && gameTimer <= 0 && !gameEnded)
            PhotonView.Get(this).RPC("EndGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void AddScoreDictionaryEntry(int index, string name, int startingScore)
    {
        playerScores.Add(index, startingScore);
        scoreTracker.Add(index, Instantiate(scoreTrackerPrefab, transform.position, Quaternion.identity));
        scoreTracker[index].transform.SetParent(ScoresContainer.transform);
        scoreTracker[index].GetComponent<ScoreDisplayerScript>().Initialize(index, name, playerScores[index]);
        usedNames.Add(name);
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

    public void SpawnPointPickups()
    {
        if(isHost)
        {
            GameObject selected = null;
            float currentMaxDist = 0;
            foreach(GameObject spawner in pointSpawnerCluster)
            {
                if(currentMaxDist < spawner.GetComponent<PointItemsClusterSpawnerScript>().closestPlayerDist)
                {
                    currentMaxDist = spawner.GetComponent<PointItemsClusterSpawnerScript>().closestPlayerDist;
                    selected = spawner;
                }
            }

            selected.SendMessage("SpawnPickups");
        }
    }

    [PunRPC]
    public void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0;

        for(int i = 0; i < ScoresContainer.transform.childCount; i++)
        {
            var variables = ScoresContainer.transform.GetChild(i).GetComponent<ScoreDisplayerScript>();
            var tracker = Instantiate(FinalScoringDisplayPrefab, FinalScoreDisplaysContainer.transform.position, Quaternion.identity);
            tracker.GetComponent<FinalScoreDisplayScript>().InitializeDisplay((i + 1).ToString(), variables.playerName.text, variables.playerScore.text);
            tracker.transform.SetParent(FinalScoreDisplaysContainer.transform);
        }

        
        GameUi.SetActive(false);
        EndGameScreen.SetActive(true);
    }

    private void SetNormalGameUI()
    {
        EndGameScreen.SetActive(false);
        GameUi.SetActive(true);
    }

    private string GetUniqueUsername()
    {
        var originalName = PlayerID.Instance.player.accountName;
        if(!usedNames.Contains(originalName))
            return originalName;
        else
        {
            int num = -1;
            var modifiedName = originalName;
            while(usedNames.Contains(modifiedName))
            {
                num++;
                modifiedName = originalName + " (" + num.ToString() + ")";
            }

            return modifiedName;
        }
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
