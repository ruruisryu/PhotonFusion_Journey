using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayerSpawner :  SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    private NetworkRunner _runner;
    public GameObject playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // IPlayerJoined은 PlayerJoined을 가지고 있음.
    // PlayerJoined function which gets called whenever a player joins the session if the behaviour is on the same GameObject as the runner.
    // This happens for our own player but also for any other player joining from a different device.
    // It is only needed to spawn a player object for our own player.
    // When an object gets spawned with Runner.Spawn it gets automatically replicated to all other clients.
    public void PlayerJoined(PlayerRef player)
    {
        // 플레이어가 입장했을 떄 세션에 연결되어 있는지 체크
        // 플레이어를 스폰해주고, 플레이어 딕셔너리에 추가해준다 - Key: PlayerRef Value: NetworkObject
        // NetworkRunner.Spawn => 유니티의 Instantiate와 비슷한 역할
        
        // player가 세션에 연결되었는지
        if (player == Runner.LocalPlayer)
        {
            // player 스폰 위치
            Vector3 spawnPosition =
                new Vector3( (player.RawEncoded%Runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            // 플레이어 스폰
            NetworkObject networkPlayerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            // player를 플레이어 딕셔너리에 Add
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        // 현재 떠나는 player가 플레이어 딕셔너리에 존재한다면
        // 해당 NetworkObject를 Despawn하고 딕셔너리에서 지워준다.
        // NetworkRunner.Despawn => 유니티의 Destroy와 비슷한 역할
        
        // 딕셔너리에 player를 키값으로 갖는 NetworkObject가 존재하는지 체크
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            // player 디스폰
            Runner.Despawn(networkObject);
            // player를 플레이어 딕셔너리에서 Remove
            _spawnedCharacters.Remove(player);
        }
    }
    
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
            if (GUI.Button(new Rect(0, 0, 200, 40), "Shared Mode"))
            {
                StartGame(GameMode.Shared);
            }
        }
    }
}
