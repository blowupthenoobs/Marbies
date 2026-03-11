using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMarbleScript : MonoBehaviour
{
    [SerializeField] GameObject NameTag;
    public PlayerID playerID;
    private Rigidbody rb;
    private Vector2 moveInputs;
    public InputActionAsset inputs;
    private InputAction movementInputs;
    private InputAction itemUseInput;
    private bool itemUsedThisPress;

    private GameObject lastPlayerCollision;

    private int heldPower;

    [Header("PowerUpVarss")]
    [SerializeField] float rocketPower;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementInputs = inputs.FindAction("Move");
        itemUseInput = inputs.FindAction("UseItem");

        PowerContainerScript.Instance.SetPowerUpImage(heldPower);
        PointItemsClusterSpawnerScript.players.Add(gameObject);
    }

    public void GetPlayerData(PlayerID newData)
    {
        playerID = newData;
        PhotonView.Get(this).RPC("SpawnNameTag", RpcTarget.OthersBuffered, playerID.player.accountName);
        PhotonView.Get(this).RPC("SetMarbleMaterial", RpcTarget.AllBuffered, PlayerID.Instance.player.materialIndex);
    }

    public void ScorePoints()
    {
        PhotonView.Get(RoomManagerScript.Instance).RPC("EditScore", RpcTarget.All, NetworkingManagerScript.Instance.playerIndex, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerID != null)
        {
            moveInputs = movementInputs.ReadValue<Vector2>();

            var moveDirection = GetMoveDirection();

            rb.AddForce(moveDirection, ForceMode.Acceleration);

            if(transform.position.y < RoomManagerScript.Instance.deathCutoffHeight)
            {
                lastPlayerCollision?.GetComponent<PhotonView>().RPC("OnPlayerKill", RpcTarget.Others);
                RoomManagerScript.Instance.SpawnPlayer();
                KillPlayer();
            }

            if(itemUseInput.IsPressed() && !itemUsedThisPress)
            {
                itemUsedThisPress = true;  
                UsePower();
            }
            else if(!itemUseInput.IsPressed())
                itemUsedThisPress = false;
        }
        else if(transform.position.y < RoomManagerScript.Instance.deathCutoffHeight)
            KillPlayer();
    }

    [PunRPC]
    public void SpawnNameTag(string name)
    {
        var tag = Instantiate(NameTag, transform.position, Quaternion.identity);
        tag.GetComponent<NametagHoverScript>().target = gameObject;
        tag.GetComponent<NametagHoverScript>().ChangeNametag(name);
    }

    [PunRPC]
    public void SetMarbleMaterial(int index)
    {
        gameObject.GetComponent<Renderer>().material = PlayerID.Instance.defaultMaterialList[index];
    }

    [PunRPC]
    public void OnPlayerKill()
    {
        if(playerID != null)
            Debug.Log("KilledPlayer");
    }

    private void KillPlayer()
    {
        PointItemsClusterSpawnerScript.players.Remove(gameObject);
        Destroy(gameObject);
    }

    [PunRPC]
    public void GetCollisionForce(Vector3 impactForce)
    {
        rb.AddForce(impactForce, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerMarbleScript>() != null && playerID != null)
        {    
            collision.gameObject.GetComponent<PhotonView>().RPC("GetCollisionForce", RpcTarget.All, rb.velocity);
            lastPlayerCollision = collision.gameObject;
            StartCoroutine("EndKillClaim");
        }
    }

    private IEnumerator EndKillClaim()
    {
        yield return new WaitForSeconds(10f);
        lastPlayerCollision = null;
    }

    public Vector3 GetMoveDirection()
    {
        var moveDirection = new Vector3();

        moveDirection.x = moveInputs.x * Mathf.Sin((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Sin(playerID.yRotation  * Mathf.Deg2Rad);
        moveDirection.z = moveInputs.x * Mathf.Cos((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Cos(playerID.yRotation  * Mathf.Deg2Rad);

        moveDirection.Normalize();

        return moveDirection;
    }

    public Vector3 GetForwardDirection()
    {
        var direction = new Vector3();

        direction.x = Mathf.Sin(playerID.yRotation  * Mathf.Deg2Rad);
        direction.z = Mathf.Cos(playerID.yRotation  * Mathf.Deg2Rad);

        direction.Normalize();

        return direction;
    }

#region PowerUps
    public void PickupPower(object power)
    {
        heldPower = (int)power;
        PowerContainerScript.Instance.SetPowerUpImage(heldPower);
    }

    public void UsePower()
    {
        switch(heldPower)
        {
            case 0:
                break;
            case 1:
                UseRocket();
                break;
            case 2:
                UseGrowthThing();
                break;
            default:
                Debug.Log(("no power set for: ", heldPower.ToString()));
                break;
        }
    }

    private void UseRocket()
    {
        rb.AddForce(GetForwardDirection() * rocketPower, ForceMode.Impulse);
        heldPower = 0;
    }

    private void UseGrowthThing()
    {
        
    }

#endregion PowerUps
}
