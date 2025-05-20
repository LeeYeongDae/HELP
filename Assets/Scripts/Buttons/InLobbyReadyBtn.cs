using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;

public class InLobbyReadyBtn : MonoBehaviour
{
    public GameObject readyButton;
    public GameObject waitButton;

    public string readyState;
    private string playerId => AuthenticationService.Instance.PlayerId;

    private void Start()
    {
        UpdateButtonUI();
    }

    public void InLobbyReadyBtnPressed()
    {
        Player player = LobbyManager.Instance.GetPlayerById(playerId);
        LobbyManager.Instance.ReadyPlayer(player);
    }
    public void UpdateButtonUI()
    {
        Player player = LobbyManager.Instance.GetPlayerById(playerId);

        if (player == null) return;

        readyState = player.Data["Ready"].Value;

        readyButton.SetActive(readyState != "Ready");
        waitButton.SetActive(readyState == "Ready");
    }
}
