using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("카탈로그 등록")]
    [SerializeField] private SceneCatalog _catalog;
    [SerializeField] private SceneEffect _effect;

    [Header("옵션")]
    [SerializeField] private bool _enableHotKeys = true;
    [SerializeField] private bool _dontDestroyOnLoad = true;
    [SerializeField] private float _fadeDuration = 1.5f;
    [SerializeField] private bool _minimumTimeUse = true;

    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private static SceneFlow _instance;
    private int _cursorIndex = 0;
    private bool _isLoading = false;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void SyncCursorToCurrentScene()
    {
        // 방어 코드
        List<SceneCatalog.SceneEntry> entries = _catalog.GetEntries();
        if (De.IsNull(entries)) return;
        if (De.IsTrue(entries.Count == 0)) return;
        // 현재 씬과 변수 매칭
        string currentName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < entries.Count; ++i) {
            if (entries[i].name == currentName) {
                _cursorIndex = i;
                De.Print($"커서 싱크를 맞추었습니다. [ {_cursorIndex} / {currentName} ]");
                return;
            }
        }
        De.Print("커서 싱크를 실패했습니다.", LogType.Assert);
    }
    public void LoadScene(SceneCatalog.ESceneID id, float mininumTime)
    {
        // 방어 코드
        if (De.IsFalse(_catalog.TryGetSceneName(id, out string name)))
            return;
        if (De.IsTrue(string.IsNullOrEmpty(name)))
            return;
        // 연출과 함께 로딩
        StartCoroutine(CLoadSceneWithEffect(id, name, mininumTime));
    }
    private IEnumerator CLoadSceneWithEffect(SceneCatalog.ESceneID id, string name, float mininumTime)
    {
        if (De.IsTrue(_isLoading))
            yield break;
        ;
        // 비동기 씬 로드
        float nextTime = Time.time + mininumTime;
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) {
            yield return null;
        }
        De.Print($"씬 로드율이 90%에 도달했습니다. ({name})");
        if (_minimumTimeUse) {
            while (Time.time < nextTime) {
                yield return null;
            }
            De.Print($"씬 최소 재생 시간이 끝났습니다. ({name})");
        }
        // 페이드 아웃
        _isLoading = true;
        if (_effect != null) {
            yield return _effect.CFadeTo(1f, _fadeDuration);
        }
        op.allowSceneActivation = true;
        while (op.progress < 1f) {
            yield return null;
        }
        // 페이드 인
        if (_effect != null) {
            yield return _effect.CFadeTo(0f, _fadeDuration);
        }
        // 새로운 씬으로 변수 맞추기
        SyncCursorToCurrentScene();
        De.Print($"씬 로드가 완료되었습니다. ({name})");
        _isLoading = false;
    }
    private void ReloadCurrent()
    {
        // 카탈로그에 현재 씬이 없을 경우
        string current = SceneManager.GetActiveScene().name;
        if (De.IsFalse(_catalog.TryGetSceneID(current, out SceneCatalog.ESceneID id)))
            return;
        // 리로드
        De.Print($"씬({name}) 리로드를 시도합니다.");
        LoadScene(id, 0f);
    }
    public void LoadNext(float mininumTime)
    {
        // 방어 코드
        List<SceneCatalog.SceneEntry> entries = _catalog.GetEntries();
        if (De.IsNull(entries)) return;
        if (De.IsTrue(entries.Count == 0)) return;
        // 다음 씬으로 이동
        _cursorIndex++;
        if (entries.Count <= _cursorIndex)
            _cursorIndex = 0;
        string nextSceneName = entries[_cursorIndex].name;
        if(De.IsFalse(_catalog.TryGetSceneID(nextSceneName, out SceneCatalog.ESceneID id)) )
            return;
        De.Print($"다음 씬으로 이동합니다. ({_cursorIndex} → {nextSceneName})");
        LoadScene(id, mininumTime);
    }
    public void LoadPrev(float mininumTime)
    {
        // 방어 코드
        List<SceneCatalog.SceneEntry> entries = _catalog.GetEntries();
        if (De.IsNull(entries)) return;
        if (De.IsTrue(entries.Count == 0)) return;
        // 이전 씬으로 이동
        _cursorIndex--;
        if (_cursorIndex < 0)
            _cursorIndex = entries.Count - 1;
        string nextSceneName = entries[_cursorIndex].name;
        if(De.IsFalse(_catalog.TryGetSceneID(nextSceneName, out SceneCatalog.ESceneID id)) )
            return;
        De.Print($"이전 씬으로 이동합니다. ({_cursorIndex} → {nextSceneName})");
        LoadScene(id, mininumTime);
    }
    private void HandleHotKeys()
    {
        // 숫자 입력으로 특정 씬으로 이동
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LoadScene(SceneCatalog.ESceneID.Manager, 0f);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            LoadScene(SceneCatalog.ESceneID.Title, 0f);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            LoadScene(SceneCatalog.ESceneID.Planet, 0f);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            LoadScene(SceneCatalog.ESceneID.GamePlay, 0f);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            LoadScene(SceneCatalog.ESceneID.GameEnd, 0f);
        // 대괄호 입력으로 앞 뒤 씬으로 이동
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
            LoadNext(0f);
        else if (Input.GetKeyDown(KeyCode.RightBracket))
            LoadPrev(0f);
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Awake()
    {
        if (De.IsNull(_catalog) || De.IsNull(_effect)) {
            Destroy(gameObject);
            return;
        }
        // 중복 인스턴스 방지
        if (_instance != null && _instance != this) {
            De.Print("기존 인스턴스가 있으므로 현재 오브젝트를 제거합니다.");
            Destroy(gameObject);
            return;
        }
        // 싱글톤
        _instance = this;
        if (_dontDestroyOnLoad) {
            DontDestroyOnLoad(this.gameObject);
        }
        // 변수 정리
        _catalog.BuildMaps();
        SyncCursorToCurrentScene();
    }
    private void Start()
    {
        if (_effect != null)
            _effect.Initialize();
    }
    private void Update()
    {
        if (!_enableHotKeys) return;
        if (_isLoading) return;
        if (De.IsNull(_catalog)) return;

        HandleHotKeys();
    }
    // 인스턴스 정리
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    #endregion
}
