using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class JoinLobbyBtn : MonoBehaviour
{
    public Lobby lobby;

    public void JoinLobbyBtnPressed()
    {
        LobbyManager.Instance.JoinLobby(lobby.Id, lobby.HasPassword);
    }
}
