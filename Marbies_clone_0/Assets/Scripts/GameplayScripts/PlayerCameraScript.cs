using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] float followDist;
    [SerializeField] float viewHeight;
    [SerializeField] float sensitivity;
    [SerializeField] float maxRotX;
    [SerializeField] float minRotX;


    [SerializeField] float cameraCutoffHeight;
    Vector2 mouseMovement;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        var rot = transform.rotation.eulerAngles;

        //X spins vertically, Y horizontally, and Z tilts the camera
        rot.x -= mouseMovement.y;
        rot.y += mouseMovement.x * sensitivity;

        rot.x = Mathf.Clamp(rot.x, minRotX, maxRotX);

        transform.rotation = Quaternion.Euler(rot);

        if(rot.y < 0)
            rot.y = 360 + rot.y;

        var displacement = new Vector3(-Mathf.Cos((90-rot.y) * Mathf.Deg2Rad), viewHeight, -Mathf.Sin((90-rot.y) * Mathf.Deg2Rad)) * followDist;

        if(player != null)
        {
            if(player.transform.position.y + displacement.y > cameraCutoffHeight)
                transform.position = player.transform.position + displacement;
            else if(transform.position.y != cameraCutoffHeight)
            {
                var newPos = player.transform.position + displacement;
                newPos.y = cameraCutoffHeight;

                transform.position = newPos;
            }   
        }

        PlayerID.Instance.yRotation = rot.y;
    }
}
