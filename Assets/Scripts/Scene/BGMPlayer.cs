using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 매니저 씬 오브젝트에 부착하는 C# 스크립트입니다.
/// 현재 활성 씬을 인식하여 해당 씬에 등록된 배경음 중 하나를 랜덤 재생합니다.
/// 씬 전환 시 자동으로 다음 씬의 배경음으로 전환합니다.
/// </summary>
public class BGMPlayer : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip[] clips;
    }

    [Header("필수 요소 등록")]
    [SerializeField] private SceneBGM[] _sceneBGMs;

    [Header("사용자 정의 설정")]
    [SerializeField, Range(0f, 1f)] private float _volume = 0.5f;
    [SerializeField] private float _fadeSpeed = 1f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private AudioSource _source;
    private string _currentScene;
    private float _targetVolume;
    private bool _fading;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Awake()
    {
        // AudioSource 자동 생성
        _source = gameObject.AddComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = 0f;
        _currentScene = string.Empty;

        // 씬 전환 이벤트 구독
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Start()
    {
        // 초기 씬 확인
        string active = SceneManager.GetActiveScene().name;
        TryPlayForScene(active);
    }

    private void Update()
    {
        if (!_fading)
            return;

        _source.volume = Mathf.MoveTowards(_source.volume, _targetVolume, _fadeSpeed * Time.unscaledDeltaTime);
        if (Mathf.Approximately(_source.volume, _targetVolume))
            _fading = false;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        TryPlayForScene(newScene.name);
    }

    private void TryPlayForScene(string sceneName)
    {
        if (sceneName == _currentScene)
            return;
        _currentScene = sceneName;

        // 해당 씬의 BGM 목록 검색
        AudioClip[] clips = null;
        for (int i = 0; i < _sceneBGMs.Length; ++i) {
            if (_sceneBGMs[i].sceneName == sceneName) {
                clips = _sceneBGMs[i].clips;
                break;
            }
        }

        // 클립이 없으면 페이드 아웃
        if (clips == null || clips.Length <= 0) {
            FadeOut();
            return;
        }

        // 랜덤 선택
        AudioClip chosen = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (chosen == null) {
            FadeOut();
            return;
        }

        // 재생
        _source.clip = chosen;
        _source.Play();
        FadeIn();
    }

    private void FadeIn()
    {
        _targetVolume = _volume;
        _fading = true;
    }

    private void FadeOut()
    {
        _targetVolume = 0f;
        _fading = true;
    }

    /// <summary>
    /// 외부에서 볼륨을 변경할 때 사용합니다.
    /// </summary>
    public void SetVolume(float volume)
    {
        _volume = Mathf.Clamp01(volume);
        if (_source.isPlaying) {
            _targetVolume = _volume;
            _fading = true;
        }
    }
    #endregion
}
