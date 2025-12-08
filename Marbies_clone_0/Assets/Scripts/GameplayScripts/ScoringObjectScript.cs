using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoringObjectScript : PickupableScript
{
    public override void OnPickUpEffect()
    {
        if(Collector.GetComponent<PlayerMarbleScript>().playerID != null)
            Collector.SendMessage("ScorePoints");
        Destroy(gameObject);
        PhotonView.Get(this).RPC("AllClientPickUpEffect", RpcTarget.All);
    }
}
