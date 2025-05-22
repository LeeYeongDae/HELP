using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Linq;

public class LobbyPlayerData : MonoBehaviour
{

    public static LobbyPlayerData Instance;

    public Lobby joinedLobby;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetPlayerNameById(string id)
    {
        var player = joinedLobby.Players.FirstOrDefault(p => p.Id == id);
        if (player != null && player.Data.ContainsKey("Name"))
        {
            return player.Data["Name"].Value;
        }
        return "NoName";
    }
}