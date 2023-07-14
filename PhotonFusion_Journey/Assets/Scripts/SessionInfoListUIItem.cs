using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class SessionInfoListUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionNameText;
    [SerializeField] private TextMeshProUGUI _playerCountText;
    [SerializeField] private Button _joinButton;

    private SessionInfo _sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo sessionInfo)
    {
        _sessionInfo = sessionInfo;

        _sessionNameText.text = sessionInfo.Name;
        _playerCountText.text = $"{sessionInfo.PlayerCount.ToString()} / {sessionInfo.MaxPlayers.ToString()}";

        bool isJoinButtonActive = true;
        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            isJoinButtonActive = false;
        }
        _joinButton.gameObject.SetActive(isJoinButtonActive);
    }

    public void OnClick()
    {
        // Invoke the join session events
        OnJoinSession?.Invoke(_sessionInfo);
    }
}
