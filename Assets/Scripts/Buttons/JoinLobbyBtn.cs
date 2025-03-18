using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinLobbyBtn : MonoBehaviour
{
    public bool isPrivate;
    public string lobbyId;

    public void JoinLobbyBtnPressed()
    {
        LobbyManager.Instance.JoinLobby(lobbyId, isPrivate);
    }
}
