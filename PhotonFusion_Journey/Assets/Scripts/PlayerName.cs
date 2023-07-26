using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerName_Text;
    private NetworkRunner _networkRunner;
    private PlayerRef _playerRef;

    private void Start()
    {
        _networkRunner = FindObjectOfType<NetworkRunner>();
        _playerRef = _networkRunner.LocalPlayer;
        PlayerName_Text.text = $"Player Name: {_playerRef.PlayerId}";
    }
}
