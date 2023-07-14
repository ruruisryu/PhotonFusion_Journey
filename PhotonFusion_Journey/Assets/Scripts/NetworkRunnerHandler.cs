using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] NetworkRunner NetworkRunnerPrefab;
    protected NetworkRunner networkRunner;

    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();
        
        // 만약 이미 NetworkRunner가 씬 내에 존재한다면 우리는 새로운 NetworkRunner를 생성해줄 필요가 없다.
        if (networkRunnerInScene != null)
        {
            networkRunner = networkRunnerInScene;
        }
    }

    void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(NetworkRunnerPrefab);
            networkRunner.name = "Network Runner";

            if (SceneManager.GetActiveScene().name != "Main")
            {
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", 
                    NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            }
            Debug.Log("Server Network Runner Started.");
        }
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, NetAddress address,
        SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        if (sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Initialized = initialized,
            SceneManager = sceneManager,
        });
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("Join Lobby started.");
        
        string lobbyID = "OurLobbyID";
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.LogError($"Unable to join lobby {lobbyID}");
        }
        else
        {
            Debug.Log("Join Lobby Ok.");
        }
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        Debug.Log($"Create session {sessionName} scene {sceneName} build Index {SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}")}");
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(),
            SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"), null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");
        
        // 존재하는 게임에 client로서 참여
        // Scene을 그냥 GetActiveScene().buildIndex로 불러오는 이유: 어차피 호스트에 의해 정보가 덮어쓰여지기 떄문에 상관없다.
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, NetAddress.Any(),
            SceneManager.GetActiveScene().buildIndex, null);
    }
}
