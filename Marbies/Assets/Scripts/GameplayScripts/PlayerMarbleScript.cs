using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMarbleScript : MonoBehaviour
{
    [SerializeField] PlayerID playerID;
    private Rigidbody rb;
    private Vector2 moveInputs;
    public InputActionAsset inputs;
    private InputAction movementInputs;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementInputs = inputs.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        moveInputs = movementInputs.ReadValue<Vector2>();

        var moveDirection = new Vector3();

        moveDirection.x = moveInputs.x * Mathf.Sin((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Sin(playerID.yRotation  * Mathf.Deg2Rad);
        moveDirection.z = moveInputs.x * Mathf.Cos((playerID.yRotation + 90)  * Mathf.Deg2Rad) + moveInputs.y * Mathf.Cos(playerID.yRotation  * Mathf.Deg2Rad);

        moveDirection.Normalize();

        rb.AddForce(moveDirection, ForceMode.Acceleration);
    }

    
    protected void OnEnable()
    {
        inputs.FindActionMap("MarbleControls").Enable();
    }

    protected void DisableInputSystem()
    {
        inputs.FindActionMap("MarbleControls").Disable();
    }
}
