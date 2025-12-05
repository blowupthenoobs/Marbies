using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NametagHoverScript : MonoBehaviour
{
    public GameObject target;
    [SerializeField] TMP_Text nametag;

    void Update()
    {
        if(target != null)
            transform.position = target.transform.position;
        transform.LookAt(Camera.main.transform.position);

        if(transform.position.y < RoomManagerScript.Instance.deathCutoffHeight)
            Destroy(gameObject);
    }

    public void ChangeNametag(string newText)
    {
        nametag.text = newText;
    }
}
