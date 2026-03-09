using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoringObjectScript : PickupableScript
{
    public static int currentObjectCount;

    void Awake()
    {
        currentObjectCount++;
    }

    public override void OnPickUpEffect()
    {
        if(Collector.GetComponent<PlayerMarbleScript>().playerID != null)
            Collector.SendMessage("ScorePoints");
        Destroy(gameObject);
        PhotonView.Get(this).RPC("AllClientPickUpEffect", RpcTarget.All);
        currentObjectCount--;

        if(currentObjectCount == 0)
            RoomManagerScript.Instance.Invoke("SpawnPointPickups", 5);
    }
}
