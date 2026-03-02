using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbiePreviewScript : MonoBehaviour
{
    public float minSpeed;
    public Vector3 currentMovementDirection;
    public float currentMoveSpeed;
    public float[] startingSpeed;
    private bool beingDragged = false;

    void Start()
    {
        StartSpinning();
    }

    public void StartSpinning()
    {
        currentMovementDirection = (new Vector3(Random.Range(0, 2) - 1, Random.Range(0, 2) - 1, Random.Range(0, 2) - 1)).normalized;
        currentMoveSpeed = Random.Range(startingSpeed[0], startingSpeed[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if(!beingDragged)
            transform.rotation = Quaternion.Euler(transform.eulerAngles + (currentMovementDirection * currentMoveSpeed));
    }

    private void OnMouseDrag() //Needs to be revamped later
    {
        var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        var rot = transform.rotation.eulerAngles;
        var sensitivity = 10;

        rot.x += mouseMovement.y * sensitivity;
        rot.y -= mouseMovement.x * sensitivity;

        if(rot.y < 0)
            rot.y += 360;
        
        transform.rotation = Quaternion.Euler(rot);
        beingDragged = true;

        if(mouseMovement != new Vector2())
        {
            currentMovementDirection = mouseMovement.normalized;
            currentMoveSpeed = mouseMovement.magnitude;

            if(currentMoveSpeed < minSpeed)
                currentMoveSpeed = minSpeed;
        }
    }

    private void OnMouseUp()
    {
        beingDragged = false;
    }

    public void ChangeMarbieMaterial(Material texture)
    {
        gameObject.GetComponent<Renderer>().material = texture;
    }
}
