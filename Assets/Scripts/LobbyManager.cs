using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance { get; private set; }


    [Header("Create Lobby")]
    [SerializeField] private GameObject Parent_Clobby;
    [SerializeField] private TMP_InputField IF_CLobbyName;
    [SerializeField] private Toggle Tgl_CLobbyPrivate;
    [SerializeField] private TMP_InputField IF_CLobbyPwd;

    [Header("Lobby List")]
    [SerializeField] private GameObject Parent_LobbyList;
    [SerializeField] private Transform Parent_LobbyContent;
    [SerializeField] private Transform LobbyTab;

    [Header("Create Profile")]
    [SerializeField] private GameObject Parent_CProfile;
    [SerializeField] private TMP_InputField IF_CProfilenick;

    [Header("In Lobby")]
    [SerializeField] private GameObject Parent_InLobby;
    [SerializeField] private Transform PlayerTab;
    [SerializeField] private Transform Parent_PlayerContent;
    [SerializeField] private TextMeshProUGUI InLobbyNameTxt;
    [SerializeField] private GameObject Btn_StartGame;

    [Header("Input Password")]
    [SerializeField] private GameObject Parent_InputPwd;
    [SerializeField] private TMP_InputField IF_InputPwd;
    [SerializeField] private Button Btn_Inputpwd;


    private string playerName;
    private Player PlayerData;
    public string joinedLobbyId;

    private async void Start()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Parent_CProfile.SetActive(true);
        Parent_LobbyList.SetActive(false);
        Parent_Clobby.SetActive(false);
    }

    private async void UpdateLobbyInfo()
    {
        while(Application.isPlaying)
        {
            if (string.IsNullOrEmpty(joinedLobbyId))
            {
                return;
            }
            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);

            if(AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                Btn_StartGame.SetActive(true);
            }
            else
            {
                Btn_StartGame.SetActive(false);
            }

            InLobbyNameTxt.text = lobby.Name;

            foreach(Transform transform in Parent_PlayerContent)
            {
                Destroy(transform.gameObject);
            }
            foreach(Player player in lobby.Players)
            {
                Transform newPlayertab = Instantiate(PlayerTab, Parent_PlayerContent);
                newPlayertab.GetChild(0).gameObject.SetActive((lobby.HostId == player.Id) ? true : false);
                newPlayertab.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
            }
        }
    }

    public void CreateProfile()
    {
        playerName = IF_CProfilenick.text;
        Parent_CProfile.SetActive(false);
        Parent_LobbyList.SetActive(true);

        PlayerDataObject playerDataObjName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        PlayerDataObject playerDataObjRole = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Prisoner");

        PlayerData = new Player(id: AuthenticationService.Instance.PlayerId, 
            data: new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjName }, { "Role", playerDataObjRole } });
        RefreshLobbies();
    }

    public async void JoinLobby(string lobbyId, bool isPrivate)
    {
        if (isPrivate)
        {
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions 
                { Player = PlayerData, Password = await InputPassword()});

                joinedLobbyId = lobbyId;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = PlayerData });

                joinedLobbyId = lobbyId;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        Parent_LobbyList.SetActive(false);
        Parent_InLobby.SetActive(true);
        UpdateLobbyInfo();
    }

    private async Task<string> InputPassword()
    {
        bool waiting = true;
        Parent_InputPwd.SetActive(true);

        while (waiting)
        {
            Btn_Inputpwd.onClick.AddListener(() => waiting = false);
            await Task.Yield();
        }
        Parent_InputPwd.SetActive(false);
        return IF_InputPwd.text;
    }

    private async void RefreshLobbies()
    {
        
        while (Application.isPlaying && Parent_LobbyList.activeInHierarchy)
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Transform transform in Parent_LobbyContent)
            {
                Destroy(transform.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results)
            {
                Transform newLobbytab = Instantiate(LobbyTab, Parent_LobbyContent);
                newLobbytab.GetComponent<JoinLobbyBtn>().lobbyId = lobby.Id;
                newLobbytab.GetComponent<JoinLobbyBtn>().isPrivate = lobby.HasPassword;
                newLobbytab.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbytab.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + " / " + lobby.MaxPlayers;
            }

            await Task.Delay(1000);
        }
    }

    public void ExitCLobbyBtn()
    {
        Parent_Clobby.SetActive(false);
        Parent_LobbyList.SetActive(true);
        RefreshLobbies();
    }

    public async void CreateLobby()
    {
        int maxPlayers = 4;
        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Player = PlayerData;

        if(Tgl_CLobbyPrivate.isOn)
        {
            options.Password = IF_CLobbyPwd.text;
        }
        
        try
        {
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(IF_CLobbyName.text, maxPlayers, options);
            Parent_Clobby.SetActive(false);
            Parent_InLobby.SetActive(true);
            joinedLobbyId = createdLobby.Id;
            UpdateLobbyInfo();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        LobbyHeartbeat(createdLobby);
    }

    private async void LobbyHeartbeat(Lobby lobby)
    {
        while (true)
        {
            if(lobby == null)
            {
                return;
            }
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            await Task.Delay(10 * 1000);    //기본 15초지만 테스트를 위해 10초로 조정
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
