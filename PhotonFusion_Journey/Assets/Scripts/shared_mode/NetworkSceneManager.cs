using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkObject
{
    private SceneRef _currentScene;
    private SceneRef _desiredScene;
    private bool _currentSceneOutdated;
    private IEnumerator _runningCoroutine;

    private Dictionary<Guid, NetworkObject> _sceneObjects;
    private static NetworkSceneManager _sceneManager;

    // LateUpdate(): 씬 업데이트 감지
    // 현재 로드된 씬이 NetworkRunner.CurrentScene에서 제공하는 씬과 다른지 확인
    protected virtual void LateUpdate()
    {
        if (!Runner)
            return;

        if (Runner.CurrentScene != _currentScene)
            _currentSceneOutdated = true;

        //씬이 현재 비동기적으로 로드 중인지 확인하고 완료될 때까지 새 로드를 연기하여 개체를 오버라이드 하거나 손실되지 않도록 하는 것입니다.
        if (!_currentSceneOutdated || _runningCoroutine != null)
        {
            // busy or up to date
            return;
        }

        var prevScene = _currentScene;
        _currentScene = Runner.CurrentScene;
    }

    // NetworkRunner가 씬 객체를 연결할 준비가 되었을 때 이를 알려 주는 IsReady()
    // LateUpdate에서 처리하는 것과 똑같은 것 같다.
    bool IsReady(NetworkRunner runner)
    {
        Assert.Check(Runner == runner);

        if (_runningCoroutine != null)
            return false;

        if (_currentSceneOutdated)
            return false;

        if (runner.CurrentScene != _currentScene)
            return false;

        return true;
    }
    /**
    현재 씬을 언로드합니다(현재 씬이 유효한 경우).
    유니티 LoadSceneAsync() 메소드를 호출합니다.
    대기 시간이 완료될 때까지 추가 실행을 수행합니다(씬 로드 완료).
    대기 시간이 완료되고 로드된 씬이 유효하지 않으면 오류가 발생하고 종료됩니다.
    로드된 씬에서 각 NetworkObject를 찾아 저장합니다.
    발견된 모든 NetworkObject 게임 객체를 비활성화합니다(나중에 연결되면 활성화됨).
    (멀티 피어 모드의 경우) 각 NetworkObject를 Runner Visibility 처리로 등록합니다.
    발견된 NetworkObject 컬렉션을 인수로 전달하는 Finished() delegate를 트리거 합니다.
    **/
}
