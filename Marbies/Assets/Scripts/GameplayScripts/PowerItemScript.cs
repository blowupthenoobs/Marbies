using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PowerItemScript : PickupableScript
{
    public enum powerType {none = 0, rocket = 1, sizeUp = 2, hammer = 3};
    public powerType power;
    public override void OnPickUpEffect()
    {
        if(Collector.GetComponent<PlayerMarbleScript>().playerID != null)
            Collector.SendMessage("PickupPower", power);
        Destroy(gameObject);
        PhotonView.Get(this).RPC("AllClientPickUpEffect", RpcTarget.All);
    }

    
    public override void OnCollectorDeath()
    {
        Destroy(gameObject);
    }
}
