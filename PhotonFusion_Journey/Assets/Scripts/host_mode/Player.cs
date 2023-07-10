using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        // 네트워크 캐릭터 컨트롤러 가져오기
        _cc = gameObject.GetComponent<NetworkCharacterControllerPrototype>();
    }

    // FixedUpdateNetwork 함수를 이용해 Fusion 시뮬레이션 루프에 참여할 수 있도록 함 
    // 각 틱에 올바른 입력이 적용되도록 하려면 FixedUpdateNetwork에서 입력을 적용해야함.
    public override void FixedUpdateNetwork()
    {
        // GetInput(): 한 틱에 대한 입력을 얻을 수 있는 메소드
        if (GetInput(out BasicSpawner.NetworkInputData data))
        {
            // direction을 normalize한 다음 캐릭터 컨트롤러를 호출해 실제 움직임을 적용
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }
    }
}
