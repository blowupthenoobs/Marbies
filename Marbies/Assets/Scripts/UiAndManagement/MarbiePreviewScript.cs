using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbiePreviewScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMarbieMaterial(Material texture)
    {
        gameObject.GetComponent<Renderer>().material = texture;
    }
}
