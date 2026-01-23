using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "name4mePls/player")]
public class PlayerID : ScriptableObject
{
    //Gameplay tools
    public float yRotation;


    //Actually saved stuffs
    public int materialIndex;
    public string playerName;

    public Account player;

    public void SetUpAccount()
    {
        if(player == null)
        {
            if(ES3.KeyExists("account"))
                player = ES3.Load<Account>("account");
            else
            {
                player = new Account();
                //give em any starting items
            }
        }
        
    }

    [System.Serializable]
    public class Account
    {
        public string accountName;
        // public Stats stats;
        public Settings settings;
        public List<string> achievements;

        public Account(string accountName, Settings settings, List<string> achievements)
        {
            this.accountName = accountName;
            this.settings = settings;
            this.achievements = achievements;
        }

        public Account()
        {
            this.accountName = "new account";
            // this.stats = new Stats();
            this.settings = new Settings();
            this.achievements = new List<string>();
        }
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
