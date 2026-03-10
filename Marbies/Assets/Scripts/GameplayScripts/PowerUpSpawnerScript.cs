using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUpSpawnerScript : MonoBehaviour
{
    [SerializeField] GameObject powerUp;
    private GameObject spawnedPower;
    [SerializeField] float minCooldown;
    [SerializeField] float maxCoolDown;
    private float currentCooldown;
    private float elapsedCooldown;

    void Awake()
    {
        SpawnPower();
    }

    void Update()
    {
        if(RoomManagerScript.Instance.isHost && spawnedPower == null)
        {
            if(elapsedCooldown >= currentCooldown)
                SpawnPower();
            else
                elapsedCooldown += Time.deltaTime;
        }
    }

    public void SpawnPower()
    {
        spawnedPower = PhotonNetwork.Instantiate(powerUp.name, transform.position, Quaternion.identity);
        elapsedCooldown = 0;
        currentCooldown = (float)(Random.Range(minCooldown * 100, maxCoolDown * 100) * .01);
    }
}
