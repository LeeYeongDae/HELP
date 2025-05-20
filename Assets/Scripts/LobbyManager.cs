using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using System.Linq;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance { get; private set; }
    public event Action<Lobby> joinLobbyEvent;
    public event Action leaveLobbyEvent;
    public event Action gameStartEvent;

    [SerializeField] private RelayManager relayManager;

    [SerializeField] private GameObject Parent_Main;

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
    [SerializeField] private TextMeshProUGUI InLobbyCodeTxt;
    [SerializeField] private GameObject Btn_ReadyPlayer;
    [SerializeField] private GameObject Btn_WaitPlayer;
    [SerializeField] private GameObject Btn_StartGame;

    [Header("Input Password")]
    [SerializeField] private GameObject Parent_InputPwd;
    [SerializeField] private TMP_InputField IF_InputPwd;
    [SerializeField] private Button Btn_Inputpwd;

    private string playerName;
    private Player PlayerData;
    public string joinedLobbyId;
    private Lobby joinedLobby;


    Dictionary<string, bool> readyStatus = new();
    public int readyPlayer = 0;

    public static List<Player> currentLobbyPlayers = new List<Player>();

    private async void Start()
    {
        Instance = this;

        Tgl_CLobbyPrivate.onValueChanged.AddListener(OnCLobbyPrivateTgl);

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Parent_Main.SetActive(true);
        Parent_CProfile.SetActive(false);
        Parent_LobbyList.SetActive(false);
        Parent_Clobby.SetActive(false);
        Parent_InLobby.SetActive(false);
        Parent_InputPwd.SetActive(false);

        StartLobbyPollingLoop();
    }

    public void OnCLobbyPrivateTgl(bool value)
    {
        IF_CLobbyPwd.interactable = value;
    }

    public void CreateProfile()
    {
        playerName = IF_CProfilenick.text;
        Parent_CProfile.SetActive(false);
        Parent_LobbyList.SetActive(true);

        PlayerDataObject playerDataObjName = new(PlayerDataObject.VisibilityOptions.Public, playerName);
        PlayerDataObject playerDataObjRole = new(PlayerDataObject.VisibilityOptions.Public, "Prisoner");
        PlayerDataObject playerDataObjReady = new(PlayerDataObject.VisibilityOptions.Public, "Wait");
        PlayerData = new Player(id: AuthenticationService.Instance.PlayerId, 
            data: new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjName }, { "Role", playerDataObjRole }, { "Ready", playerDataObjReady } });

        Debug.Log("player " + PlayerData.Id);
    }

    public async void CreateLobby()
    {
        int maxPlayers = 4;
        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = PlayerData,
            IsPrivate = Tgl_CLobbyPrivate.isOn
        };

        if (Tgl_CLobbyPrivate.isOn)
        {
            options.Password = IF_CLobbyPwd.text;
        }
        DataObject dataObjJoinCode = new DataObject(DataObject.VisibilityOptions.Public, string.Empty);
        options.Data = new Dictionary<string, DataObject> { { "JoinCode", dataObjJoinCode } };


        try
        {
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(IF_CLobbyName.text, maxPlayers, options);
            Parent_Clobby.SetActive(false);
            Parent_InLobby.SetActive(true);
            joinedLobbyId = createdLobby.Id;
            joinedLobby = createdLobby;
            UpdateLobbyInfo();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        joinLobbyEvent?.Invoke(createdLobby);
        LobbyHeartbeat(createdLobby);
    }

    public async void JoinLobby(string lobbyId, bool isPrivate)
    {
        var joinOption = new JoinLobbyByIdOptions { Player = PlayerData };
        if (isPrivate)
        {
            try
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinOption);
                //{ Player = PlayerData, Password = await InputPassword()});

                joinedLobbyId = lobbyId;


                OnceUpdateLobbyInfo(joinedLobby);
                Parent_LobbyList.SetActive(false);
                Parent_InLobby.SetActive(true);
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
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinOption);

                joinedLobbyId = lobbyId;


                OnceUpdateLobbyInfo(joinedLobby);
                Parent_LobbyList.SetActive(false);
                Parent_InLobby.SetActive(true);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        joinLobbyEvent?.Invoke(joinedLobby);
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

    public async void RefreshLobbies()
    {
        
        while (Application.isPlaying && Parent_LobbyList.activeInHierarchy)
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Transform transform in Parent_LobbyContent)
            {
                Destroy(transform.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results)
            {
                Transform newLobbytab = Instantiate(LobbyTab, Parent_LobbyContent);
                newLobbytab.GetComponent<JoinLobbyBtn>().lobby = lobby;
                newLobbytab.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbytab.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + " / " + lobby.MaxPlayers;
            }

            await Task.Delay(1000*15);
        }
    }

    public async void LobbyStart()
    {
        Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);
        currentLobbyPlayers = lobby.Players;
        string JoinCode = await relayManager.StartHostWithRelay(lobby.MaxPlayers);
        isJoined = true;
        await Lobbies.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions 
        { Data = new Dictionary<string, DataObject> { { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, JoinCode) } } });

        Parent_LobbyList.SetActive(false);
        Parent_InLobby.SetActive(false);

        gameStartEvent?.Invoke();
        if (Unity.Netcode.NetworkManager.Singleton.IsHost)
        {
            Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene("InGame", LoadSceneMode.Single);
        }
    }

    private bool isJoined = false;

    private async void UpdateLobbyInfo()
    {
        while (Application.isPlaying)
        {
            if (joinedLobby == null) return;

            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyId);

            if (!isJoined && lobby.Data["JoinCode"].Value != string.Empty)
            {
                await relayManager.StartClientWithRelay(lobby.Data["JoinCode"].Value);
                isJoined = true;
                Parent_InLobby.SetActive(false);
                return;
            }

            InLobbyNameTxt.text = lobby.Name;
            InLobbyCodeTxt.text = lobby.LobbyCode;

            foreach (Transform child in Parent_PlayerContent)
            {
                Destroy(child.gameObject);
            }


            foreach (Player player in lobby.Players)
            {
                if (player.Data.TryGetValue("Ready", out var readyData))
                {
                    readyStatus[player.Id] = readyData.Value == "Ready";
                }
            }
            foreach (Player player in lobby.Players)
            {
                Transform newPlayertab = Instantiate(PlayerTab, Parent_PlayerContent);
                newPlayertab.GetChild(0).gameObject.SetActive(lobby.HostId == player.Id);
                newPlayertab.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
                newPlayertab.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.Data["Ready"].Value; ;
            }

            readyPlayer = readyStatus.Values.Count(ready => ready);

            Btn_StartGame.SetActive(IsLobbyhost() && readyPlayer >= lobby.Players.Count);
            
        }
    }

    private void OnceUpdateLobbyInfo(Lobby lobby)
    {
        InLobbyNameTxt.text = lobby.Name;
        InLobbyCodeTxt.text = lobby.LobbyCode;

        foreach (Transform child in Parent_PlayerContent)
        {
            Destroy(child.gameObject);
        }

        readyStatus.Clear();
        foreach (Player player in lobby.Players)
        {
            if (player.Data.TryGetValue("Ready", out var readyData))
            {
                readyStatus[player.Id] = readyData.Value == "Ready";
            }
        }

        foreach (Player player in lobby.Players)
        {
            Transform newPlayerTab = Instantiate(PlayerTab, Parent_PlayerContent);
            newPlayerTab.GetChild(0).gameObject.SetActive(lobby.HostId == player.Id);
            newPlayerTab.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
            newPlayerTab.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.Data["Ready"].Value;
        }

        readyPlayer = readyStatus.Values.Count(ready => ready);
        Btn_StartGame.SetActive(IsLobbyhost() && readyPlayer >= lobby.Players.Count);
    }

    public async void LeaveLobby()
    {

        if (joinedLobby == null) return;
        try
        {
            MigrateHost();
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;

            leaveLobbyEvent?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"{e}");
        }
    
    }

    private async void MigrateHost()
    {
        if (!IsLobbyhost() || joinedLobby.Players.Count <= 1) return;
        try
        {
            joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"{e}");
        }
    }

    private bool IsLobbyhost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public void ExitCLobbyBtn()
    {
        LeaveLobby();
        Parent_Clobby.SetActive(false);
        Parent_LobbyList.SetActive(true);
        RefreshLobbies();
    }

    public async void ReadyPlayer(Player player)
    {
        try
        {
            string currentStatus = player.Data.ContainsKey("Ready") ? player.Data["Ready"].Value : "Wait";

            Debug.Log($"'{currentStatus}'의 상태 확인");

            string newReadyValue = currentStatus == "Ready" ? "Wait" : "Ready";

            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, player.Id, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                { "Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newReadyValue) }
            }
            });

            joinedLobby = await Lobbies.Instance.GetLobbyAsync(joinedLobby.Id);
            OnceUpdateLobbyInfo(joinedLobby);

            FindObjectOfType<InLobbyReadyBtn>()?.UpdateButtonUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"{e}");
        }
    }

    public Player GetPlayerById(string playerId)
    {
        if (joinedLobby == null || joinedLobby.Players == null) return null;

        return joinedLobby.Players.FirstOrDefault(p => p.Id == playerId);
    }

    private async void LobbyHeartbeat(Lobby lobby)
    {
            if(lobby == null)
            {
                return;
        }
        while (lobby != null)
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            joinedLobby = await Lobbies.Instance.GetLobbyAsync(lobby.Id);

            OnceUpdateLobbyInfo(joinedLobby);

            await Task.Delay(5 * 1000);    //기본 15초지만 테스트를 위해 5초로 조정
        }
    }

    private void StartLobbyPollingLoop()
    {
        StartCoroutine(LobbyPollingCoroutine());
    }

    private IEnumerator LobbyPollingCoroutine()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(joinedLobbyId))
            {
                UpdateLobbyInfo(); // 비동기 호출, UI 갱신 포함
            }

            yield return new WaitForSeconds(3f); // 3초마다 체크
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
