using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class SessionListUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _sessionItemListPrefab;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList()
    {
        foreach (Transform child in _verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        _statusText.gameObject.SetActive(false);
    }


    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListUIItem addedSessionInfoUIItem =
            Instantiate(_sessionItemListPrefab, _verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();
        addedSessionInfoUIItem.SetInformation(sessionInfo);

        addedSessionInfoUIItem.OnJoinSession += AddedSessionINfoListUIItem_OnJoinSession;
    }
    
    // Join 버튼을 누르면 Invoke되는 함수 (Join에 대한 처리를 함)
    private void AddedSessionINfoListUIItem_OnJoinSession(SessionInfo sessionInfo)
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.JoinGame(sessionInfo);

        MainUIHandler mainUIHandler = FindObjectOfType<MainUIHandler>();
        mainUIHandler.OnJoiningServer();
    }

    public void OnNoSessionFound()
    {
        _statusText.text = "No game session found";
        _statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSession()
    {
        _statusText.text = "Looking for game session";
        _statusText.gameObject.SetActive(true);
    }
}
