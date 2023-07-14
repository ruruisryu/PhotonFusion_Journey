using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // Player가 조인했을 경우 해당 플레이어를 처리해주는 함수
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 플레이어가 입장했을 떄 세션에 연결되어 있는지 체크
        // 플레이어를 스폰해주고, 플레이어 딕셔너리에 추가해준다 - Key: PlayerRef Value: NetworkObject
        // NetworkRunner.Spawn => 유니티의 Instantiate와 비슷한 역할
        
        if (runner.IsServer)
        {
            // player 스폰 위치
            Vector3 spawnPosition =
                new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            // player 스폰
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // player를 플레이어 딕셔너리에 Add
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    // Player가 떠났을 경우 그 Player를 처리해주는 함수
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // 현재 떠나는 player가 플레이어 딕셔너리에 존재한다면
        // 해당 NetworkObject를 Despawn하고 딕셔너리에서 지워준다.
        // NetworkRunner.Despawn => 유니티의 Destroy와 비슷한 역할
        
        // 딕셔너리에 player를 키값으로 갖는 NetworkObject가 존재하는지 체크
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            // player 디스폰
            runner.Despawn(networkObject);
            // player를 플레이어 딕셔너리에서 Remove
            _spawnedCharacters.Remove(player);
        }
    }

    // 입력을 보유할 데이터 구조체 정의
    // 클라이언트는 사용자에게 즉각적인 피드백을 제공하기 위해 입력을 로컬에 바로 적용할 수 있지만 호스트에 의해 무시될 수 있는 로컬 예측일 뿐임
    // 호스트가 클라이언트로부터 입력을 수집해 네트워크 상태를 업데이트
    // Fusion은 입력을 압축하고 실제로 변경되는 데이터만 전송하므로 최적화에 너무 집착할 필요는 없다.
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 direction;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // 입력 구조 인스턴스화
        var data = new NetworkInputData();
        
        // 입력 구조 data 채우기
        if (Input.GetKey(KeyCode.W))
        {
            data.direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.direction += Vector3.right;
        }
        
        // 채워진 입력 구조를 Fusion에게 전달
        // 호스트와 이 클라이언트가 입력 권한을 가진 모든 객체에서 사용가능하도록 한다.
        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    
    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestGame",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }
    
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if(GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}
