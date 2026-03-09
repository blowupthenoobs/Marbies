using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using Photon.Pun;

public class PickupableScript : MonoBehaviour
{
    [Header("Bobbing Variables")]
    [SerializeField] float bobbingPower = 0.1f; //Amount of allowed vertical change
    [SerializeField] float minBobSpeed = 0.2f;
    [SerializeField] float maxBobSpeed = 0.25f;
    protected List<Vector3> movementPoints = new List<Vector3>();
    protected int movementIndex;
    protected float currentBobbingSpeed; //speed by which it goes up and down
    protected Collider hitbox;

    protected GameObject Collector;

    [Header("PickUp Anim Variables")]
    [SerializeField] float[] animSpeed;
    float[] placeholderAnimSpeed = {30, 15, 30};
    [SerializeField] float pushDist = 2;

    private bool ranAwake;


    void Awake()
    {
        ranAwake = true;
        
        if(animSpeed.Length == 0)
            animSpeed = placeholderAnimSpeed;

        hitbox = gameObject.GetComponent<Collider>();    

        currentBobbingSpeed = Random.Range(minBobSpeed, maxBobSpeed);
        var higherPoint = transform.position + new Vector3(0, bobbingPower, 0);
        var lowerPoint = transform.position - new Vector3(0, bobbingPower, 0);
        movementPoints.Add(lowerPoint); movementPoints.Add(higherPoint);

        movementIndex = Random.Range(0, 2);
    }

    void Update()
    {
        if(!ranAwake)
            Awake();
        
        if(Collector == null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movementPoints[movementIndex], currentBobbingSpeed * Time.deltaTime);

            if(transform.position == movementPoints[movementIndex])
            {
                movementIndex *= -1;
                movementIndex++;
            }
        }
        else
        {
            if(movementIndex != 1)
                transform.position = Vector3.MoveTowards(transform.position, movementPoints[movementIndex] + Collector.transform.position, animSpeed[movementIndex] * Time.deltaTime);
            else
            {
                var localPos = transform.position - Collector.transform.position;
                var xDist = localPos.x + localPos.z;
                var yDist = localPos.y;

                //Use that to calculate current angle around player
                var currentAngle = Mathf.Atan2(yDist, xDist);

                currentAngle = Mathf.MoveTowards(currentAngle, 90 * Mathf.Deg2Rad, animSpeed[movementIndex] * Time.deltaTime);
                var newPos = new Vector3();
                newPos.y = Mathf.Sin(currentAngle);
                var horizontalDist = Mathf.Cos(currentAngle);
                newPos.x = horizontalDist * (localPos.x / xDist);
                newPos.z = horizontalDist * (localPos.z / xDist);

                transform.position = (newPos * pushDist) + Collector.transform.position;
            }
            
            if(transform.position == Collector.transform.position + movementPoints[movementIndex])
            {
                movementIndex++;

                if(movementIndex == movementPoints.Count)
                    OnPickUpEffect();
            }
        }
    }

    public virtual void OnPickUpEffect()
    {
        Debug.Log("Picked up item");
        Destroy(gameObject);
        PhotonView.Get(this).RPC("AllClientPickUpEffect", RpcTarget.All);
    }

    [PunRPC]
    public virtual void AllClientPickUpEffect()
    {
        Debug.Log("Play particles");
    }

    protected void PlayPickUpAnimation(GameObject claimer)
    {
        Collector = claimer;
        movementPoints.Clear();
        hitbox.enabled = false;

        var impactDirection = transform.position - claimer.transform.position;
        impactDirection.y = 0;
        movementPoints.Add(impactDirection.normalized * pushDist);

        movementPoints.Add(new Vector3(0, 1, 0) * pushDist);

        movementPoints.Add(new Vector3(0, 0, 0));

        //fly up in a curve above the player //x^2 + y^2 = r^2
        //fly down into the player

        movementIndex = 0;
    }

    protected void OnTriggerEnter(Collider collision)
    {
        PlayPickUpAnimation(collision.gameObject);
        // Destroy(gameObject);
    }
}
