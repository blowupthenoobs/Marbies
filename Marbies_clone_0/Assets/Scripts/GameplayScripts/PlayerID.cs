using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "name4mePls/player")]
public class PlayerID : ScriptableObject
{
    public Material[] defaultMaterialList;
    public static PlayerID Instance;
    //Gameplay tools
    public float yRotation;


    //Actually saved stuffs
    public Account player;

    public void SetUpAccount()
    {
        // if(Instance == null)
        //     Instance = this;
        // else
        //     Destroy(this);
        
        if(player == null)
        {
            Debug.Log("running");
            if(ES3.KeyExists("account"))
                player = ES3.Load<Account>("account");
            else
            {
                player = new Account();
                //give em any starting items
            }
        }
        
        defaultMaterialList = MainMenuScript.Instance.defaultMaterialList;
    }

    public void SaveAccountInfo()
    {
        ES3.Save("account", player);
    }

    public void OnApplicationQuit()
    {
        ES3.Save("account", player);
        player = null; //simply for the editor stuffs
    }

    [System.Serializable]
    public class Account
    {
        public string accountName;
        public int materialIndex;
        // public Stats stats;
        public Settings settings;
        public List<string> achievements;
        public List<Material> importedImages;

        public Account(string accountName, int materialIndex, Settings settings, List<string> achievements, List<Material> importedImages)
        {
            this.accountName = accountName;
            this.materialIndex = materialIndex;
            this.settings = settings;
            this.achievements = achievements;
            this.importedImages = importedImages;
        }

        public Account()
        {
            this.accountName = "new account";
            // this.stats = new Stats();
            this.settings = new Settings();
            this.achievements = new List<string>();
            this.importedImages = new List<Material>();
        }
    }

    public int GetMaximumMaterialIndex()
    {
        return (defaultMaterialList.Length + player.importedImages.Count);
    }

    public Material GetSelectedMaterial()
    {
        if(player.materialIndex > defaultMaterialList.Length)
            return defaultMaterialList[player.materialIndex];
        else
            return player.importedImages[player.materialIndex - defaultMaterialList.Length];
    }

    
    [System.Serializable]
    public struct Settings
    {
        public float masterVol;
        public float musicVol;
        public float sfxVol;

        public Settings(float masterVol = 100, float musicVol = 100, float sfxVol = 50)
        {
            this.masterVol = masterVol;
            this.musicVol = musicVol;
            this.sfxVol = sfxVol;
        }
    }
}
