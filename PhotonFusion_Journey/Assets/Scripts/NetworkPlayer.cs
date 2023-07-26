using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; set; }
    private HorizontalLayoutGroup _horizontalLayoutGroup;

    public override void Spawned()
    {
        // 내가 권한을 갖고 있다면 로컬 플레이어, 아니라면 다른 플레이어 (remote)
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned local player");
        }
        else
        {
            Debug.Log("Spawned remote player");
        }

        _horizontalLayoutGroup = FindObjectOfType<HorizontalLayoutGroup>();
        if (_horizontalLayoutGroup)
        {
            transform.parent = _horizontalLayoutGroup.transform;
        }
    }
}
