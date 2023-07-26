using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class LobbyUIHandler : NetworkBehaviour
{
    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI _buttonReadyText;
    [SerializeField] private TextMeshProUGUI _countDownText;

    private bool isReady = false;
    
    void Start()
    {
        _countDownText.text = "";
    }

    void Update()
    {
        
    }

    public void OnReady()
    {
        
    }
}
