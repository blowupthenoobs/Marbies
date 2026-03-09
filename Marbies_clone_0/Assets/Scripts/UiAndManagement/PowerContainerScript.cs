using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PowerContainerScript : MonoBehaviour
{
    public static PowerContainerScript Instance;
    [SerializeField] Sprite emptySlot;
    [SerializeField] Sprite rocket;
    [SerializeField] Sprite growthThing;

    [SerializeField] UnityEngine.UI.Image powerIcon;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPowerUpImage(int power)
    {
        switch(power)
        {
            case 0:
                powerIcon.sprite = emptySlot;
                break;
            case 1:
                powerIcon.sprite = rocket;
                break;
            case 2:
                powerIcon.sprite = growthThing;
                break;
            default:
                Debug.Log("no image set for: " + power.ToString());
                break;
        }
    }
}
