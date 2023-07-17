using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // Player가 조인했을 경우 해당 플레이어를 처리해주는 함수
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 플레이어가 입장했을 떄 세션에 연결되어 있는지 체크
        // 플레이어를 스폰해주고, 플레이어 딕셔너리에 추가해준다 - Key: PlayerRef Value: NetworkObject
        // NetworkRunner.Spawn => 유니티의 Instantiate와 비슷한 역할
        
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined we are server. Spawning Player");
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
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkPlayer))
        {
            // player 디스폰
            runner.Despawn(networkPlayer);
            // player를 플레이어 딕셔너리에서 Remove
            _spawnedCharacters.Remove(player);
        }
    }
    void Start()
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnected to Server"); }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown"); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisConnected from Server"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest");}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
}
