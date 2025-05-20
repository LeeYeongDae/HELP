using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshLobbyBtn : MonoBehaviour
{
    public void RefreshLobbyBtnPressed()
    {
        LobbyManager.Instance.RefreshLobbies();
    }
}
