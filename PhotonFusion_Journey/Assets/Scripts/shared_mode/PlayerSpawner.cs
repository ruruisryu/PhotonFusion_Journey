using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawner :  SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    // IPlayerJoined은 PlayerJoined을 가지고 있음.
    // PlayerJoined function which gets called whenever a player joins the session if the behaviour is on the same GameObject as the runner.
    // This happens for our own player but also for any other player joining from a different device.
    // It is only needed to spawn a player object for our own player.
    // When an object gets spawned with Runner.Spawn it gets automatically replicated to all other clients.
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }
}
