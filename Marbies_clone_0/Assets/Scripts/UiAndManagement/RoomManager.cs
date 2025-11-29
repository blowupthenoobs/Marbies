using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;


public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject marbiePrefab;
    [SerializeField] GameObject PlayerCamera;
    [SerializeField] PlayerID playerID;

    [SerializeField] Transform[] spawnPoints;

    private void Awake()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
            SceneManager.LoadScene(0);

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

}
