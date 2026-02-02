using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MainMenuScript : MonoBehaviourPunCallbacks
{
    public static MainMenuScript Instance;
    
    public Material[] defaultMaterialList;
    [SerializeField] UIGradient backgroundGradient;
    [SerializeField] Color darkColor;
    [SerializeField] Color middleColor;
    [SerializeField] Color lightColor;

    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject MatchMakingMenu;
    [SerializeField] GameObject MatchesContainer;
    [SerializeField] GameObject OpenRoomItem;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] GameObject CustomizationMenu;
    [SerializeField] TMP_InputField UsernameBox;

    //Decorations
    [SerializeField] GameObject SpinnyCube;
    [SerializeField] Transform[] SpinnyCubePos;
    private int spinnyCubeIndex;

    [SerializeField] GameObject PreviewMarbie;
    [SerializeField] Transform[] PreviewMarbiePos;
    private int previewMarbieIndex;

    [SerializeField] float decorationMoveSpeed;


    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private string hostedRoomName = "";
    public bool browsingMatches;

    //Customization variables
    private string currentUsername;
    private int currentMaterial;

    private IEnumerator Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // if(NetworkingManagerScript.Instance != null)
        //     Destroy(NetworkingManagerScript.Instance.gameObject);

        Cursor.lockState = CursorLockMode.None;
        OpenLobbyJoinMenu();
        OpenMainMenu();
        yield return null;

        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        LobbyManagerScript.ConnectedToLobby = false;

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
        PlayerID.Instance.SetUpAccount();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if(cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;

            //If you want to put a loading thing for the rooms, this is where it disappears
        }
        else
        {
            foreach(var room in roomList)
            {
                for(int i = 0; i < cachedRoomList.Count; i++)
                {
                    if(cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        if(room.RemovedFromList)
                            newList.Remove(newList[i]);
                        else
                            newList[i] = room;

                        cachedRoomList = newList;
                    }
                }
            }
        }

        UpdateRoomList();
    }

    private void UpdateRoomList()
    {
        while(MatchesContainer.transform.childCount > 1)
        {
            var oldRoomItem = MatchesContainer.transform.GetChild(1).gameObject;
            oldRoomItem.transform.parent = null;
            Destroy(oldRoomItem);
        }

        if(browsingMatches && cachedRoomList.Count > 0)
        {
            foreach(var room in cachedRoomList)
            {
                GameObject roomItem = Instantiate(OpenRoomItem, transform.position, transform.rotation);
                roomItem.transform.parent = MatchesContainer.transform;

                roomItem.GetComponent<RoomItemScript>().roomToJoin = room.Name;
                roomItem.transform.GetChild(0).GetComponent<TMP_Text>().text = room.Name;
                roomItem.transform.GetChild(1).GetComponent<TMP_Text>().text = room.PlayerCount + "/16";
            }
        }
        
    }

    public void ChangeHostedGameName(string newName)
    {
        hostedRoomName = newName;
    }

    public void JoinRoom(string roomToJoin)
    {
        LobbyManagerScript.roomToJoin = roomToJoin;

        NetworkingManagerScript.OpenScene("LobbyScene");
    }

    public void CreateRoom()
    {
        if(hostedRoomName == "")
            hostedRoomName = "room" + Random.Range(0, 1000);
        
        LobbyManagerScript.roomToJoin =  hostedRoomName;

        NetworkingManagerScript.OpenScene("LobbyScene");
    }


#region MenuNavigation
    public void OpenMainMenu()
    {
        MatchMakingMenu.SetActive(false);
        CustomizationMenu.SetActive(false);
        SettingsMenu.SetActive(false);

        backgroundGradient.m_color1 = darkColor;
        backgroundGradient.m_color2 = lightColor;
        backgroundGradient.gameObject.SetActive(false);
        backgroundGradient.gameObject.SetActive(true);

        spinnyCubeIndex = 0;
        previewMarbieIndex = 0;

        MainMenu.SetActive(true);
    }

    public void OpenMatchMaking()
    {
        MainMenu.SetActive(false);

        backgroundGradient.m_color1 = middleColor;
        backgroundGradient.m_color2 = middleColor;
        backgroundGradient.gameObject.SetActive(false);
        backgroundGradient.gameObject.SetActive(true);

        MatchMakingMenu.SetActive(true);
    }

    public void OpenLobbyJoinMenu()
    {
        MatchesContainer.transform.GetChild(0).gameObject.SetActive(false);
        browsingMatches = true;

        UpdateRoomList();
    }

    public void OpenHostingMenu()
    {
        while(MatchesContainer.transform.childCount > 1)
        {
            var oldRoomItem = MatchesContainer.transform.GetChild(1).gameObject;
            oldRoomItem.transform.SetParent(null);
            Destroy(oldRoomItem);
        }

        MatchesContainer.transform.GetChild(0).gameObject.SetActive(true);
        browsingMatches = false;
    }

    public void OpenCustomizationMenu()
    {
        MainMenu.SetActive(false);

        backgroundGradient.m_color1 = lightColor;
        backgroundGradient.m_color2 = lightColor;
        backgroundGradient.gameObject.SetActive(false);
        backgroundGradient.gameObject.SetActive(true);

        spinnyCubeIndex = 1;
        previewMarbieIndex = 1;

        CustomizationMenu.SetActive(true);
        LoadCurrentCustomizationValues();
    }

    public void OpenSettingsMenu()
    {
        MainMenu.SetActive(false);

        backgroundGradient.m_color1 = lightColor;
        backgroundGradient.m_color2 = darkColor;
        backgroundGradient.gameObject.SetActive(false);
        backgroundGradient.gameObject.SetActive(true);

        SettingsMenu.SetActive(true);
    }
#endregion MenuNavigation

#region CustomizationFunctions
    private void LoadCurrentCustomizationValues()
    {
        UsernameBox.text = PlayerID.Instance.player.accountName;
    }

    public void ChangeUsername(string newName)
    {
        currentUsername = newName;
    }

    public void CycleMarbleStyle(int change)
    {
        currentMaterial = (currentMaterial + change + PlayerID.Instance.GetMaximumMaterialIndex()) % PlayerID.Instance.GetMaximumMaterialIndex();
    }

    public void LockInChanges()
    {
        PlayerID.Instance.player.accountName = currentUsername;
        PlayerID.Instance.player.materialIndex = currentMaterial;    
    }

#endregion CustomizationFunctions
    void Update()
    {
        MoveDecorations();
    }
    
    public void MoveDecorations()
    {
        SpinnyCube.transform.position = Vector3.MoveTowards(SpinnyCube.transform.position, SpinnyCubePos[spinnyCubeIndex].position, decorationMoveSpeed * Time.deltaTime);
        PreviewMarbie.transform.position = Vector3.MoveTowards(PreviewMarbie.transform.position, PreviewMarbiePos[previewMarbieIndex].position, decorationMoveSpeed * Time.deltaTime);
    }
}
