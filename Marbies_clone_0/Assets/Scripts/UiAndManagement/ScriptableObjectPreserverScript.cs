using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectPreserverScript : MonoBehaviour
{
    private static ScriptableObjectPreserverScript Instance;
    private List<object> objectsToPreserve = new List<object>();
    public ScriptableObject temp;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            PlayerID.Instance = (PlayerID)ScriptableObject.CreateInstance(typeof(PlayerID));
            
            objectsToPreserve.Add(PlayerID.Instance);
            DontDestroyOnLoad(this);

            temp = PlayerID.Instance;
        }
        else
            Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        PlayerID.Instance.OnApplicationQuit();
    }
}
