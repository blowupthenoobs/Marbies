using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PointItemsClusterSpawnerScript : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject[] pointPickupItem;

    public static List<GameObject> players = new List<GameObject>();
    [HideInInspector] public float closestPlayerDist;

    void Update()
    {
        closestPlayerDist = (players[0].transform.position - transform.position).magnitude;
        for(int i = 1; i < players.Count; i++)
        {
            if(players[i] == null)
            {
                players.RemoveAt(i);
                i--;    
            }
            if((players[i].transform.position - transform.position).magnitude < closestPlayerDist)
                closestPlayerDist = (players[i].transform.position - transform.position).magnitude;
        }
    }



    public void SpawnPickups()
    {
        List<Transform> currentSpawns = new List<Transform>(spawnPoints);
        int itemToSpawn = Random.Range((int)(spawnPoints.Length * .5), spawnPoints.Length);
        for(int i = 0; i < itemToSpawn; i++)
        {
            int index = Random.Range(0, currentSpawns.Count);
            PhotonNetwork.Instantiate(pointPickupItem[Random.Range(0, pointPickupItem.Length)].name, currentSpawns[index].position, Quaternion.identity);
            currentSpawns.RemoveAt(index);
        }
    }
}
