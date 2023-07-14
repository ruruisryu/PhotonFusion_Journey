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

    private void AddedSessionINfoListUIItem_OnJoinSession(SessionInfo obj)
    {
        
    }

    public void OnNOSessionFound()
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
