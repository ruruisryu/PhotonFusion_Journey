using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainUIHandler : MonoBehaviour
{
    [Header("Panels")] 
    [SerializeField] private GameObject playerDetailPanel;
    [SerializeField] private GameObject sessionBrowserPanel;
    [SerializeField] private GameObject createSessionPanel;
    [SerializeField] private GameObject statusPanel;

    [Header("Player Settings")]
    public TMP_InputField playerNameInputField;

    [Header("New Game Session")] 
    public TMP_InputField sessionNameInputField;
    
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            playerNameInputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    void HideAllPanels()
    {
        playerDetailPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
    }

    public void OnFindGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", playerNameInputField.text);
        PlayerPrefs.Save();

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();
        
        HideAllPanels();
        sessionBrowserPanel.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSession();
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.CreateGame(sessionNameInputField.text, "Lobby");
        
        HideAllPanels();
        statusPanel.SetActive(true);
    }

    public void OnJoiningServer()
    {
        HideAllPanels();
        statusPanel.gameObject.SetActive(true);
    }
}
