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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementInputs = inputs.FindAction("Move");
    }

    public void GetPlayerData(PlayerID newData)
    {
        playerID = newData;
        PhotonView.Get(this).RPC("SpawnNameTag", RpcTarget.OthersBuffered);
        PhotonView.Get(this).RPC("SetMarbleMaterial", RpcTarget.AllBuffered, playerID.materialIndex);
    }

    public void ScorePoints()
    {
        PhotonView.Get(RoomManagerScript.Instance).RPC("EditScore", RpcTarget.All, NetworkingManagerScript.Instance.playerIndex, 1);
        Debug.Log("Gained Points");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerID != null)
        {
            moveInputs = movementInputs.ReadValue<Vector2>();

            var moveDirection = new Vector3();

            moveDirection.x = moveInputs.x * Mathf.Sin((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Sin(playerID.yRotation  * Mathf.Deg2Rad);
            moveDirection.z = moveInputs.x * Mathf.Cos((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Cos(playerID.yRotation  * Mathf.Deg2Rad);

            moveDirection.Normalize();

            rb.AddForce(moveDirection, ForceMode.Acceleration);

            if(transform.position.y < RoomManagerScript.Instance.deathCutoffHeight)
            {
                RoomManagerScript.Instance.SpawnPlayer();
                Destroy(gameObject);
            }
        }
        else if(transform.position.y < RoomManagerScript.Instance.deathCutoffHeight)
            Destroy(gameObject);
    }

    [PunRPC]
    public void SpawnNameTag()
    {
        Instantiate(NameTag, transform.position, Quaternion.identity).GetComponent<NametagHoverScript>().target = gameObject;
    }

    [PunRPC]
    public void SetMarbleMaterial(int index)
    {
        gameObject.GetComponent<Renderer>().material = RoomManagerScript.Instance.defaultMaterialList[index];
    }

    [PunRPC]
    public void GetCollisionForce(Vector3 impactForce)
    {
        rb.AddForce(impactForce, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerMarbleScript>() != null && playerID != null)
            collision.gameObject.GetComponent<PhotonView>().RPC("GetCollisionForce", RpcTarget.All, rb.velocity);
    }
}
