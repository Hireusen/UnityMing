using UnityEngine;
using System.Collections;

public class SceneEffect : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("페이드 설정")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private float _defaultFadeDuration = 0.25f;
    [SerializeField] private bool _useUnscaledTime = true;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Coroutine _fadeRoutine;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 씬 전환 설정을 초기화합니다.
    /// </summary>
    public void Initialize()
    {
        if (De.IsNull(_fadeGroup))
            return;
        _fadeGroup.alpha = 0f;
        _fadeGroup.blocksRaycasts = false;
        _fadeGroup.interactable = false;
        De.Print("씬 이펙트를 초기화했습니다.");
    }
    /// <summary>
    /// 지정한 투명 값으로 점차 페이드합니다.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <param name="blockRayCastsWhileFading"></param>
    /// <returns></returns>
    public IEnumerator CFadeTo(float targetAlpha, float duration = -1f, bool blockRayCastsWhileFading = true)
    {
        if (De.IsNull(_fadeGroup))
            yield break;
        // 기본값 설정
        if (duration < 0f)
            duration = _defaultFadeDuration;
        // 기존 페이드 기능 종료
        if(_fadeRoutine != null) {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }
        // 새로운 페이드 기능 시작
        _fadeRoutine = StartCoroutine(CFadeInternal(targetAlpha, duration, blockRayCastsWhileFading));
        yield return _fadeRoutine;
        _fadeRoutine = null;
    }
    /// <summary>
    /// Alpha 값을 보간합니다.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <param name="blockRayCastsWhileFading"></param>
    /// <returns></returns>
    private IEnumerator CFadeInternal(float targetAlpha, float duration, bool blockRayCastsWhileFading)
    {
        float startAlpha = _fadeGroup.alpha;
        _fadeGroup.blocksRaycasts = blockRayCastsWhileFading;
        _fadeGroup.interactable = false;
        // 지속 시간이 0일 경우 즉시 전환
        if(duration <= 0f) {
            _fadeGroup.alpha = targetAlpha;
            _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
            yield break;
        }
        // 보간
        float t = 0f;
        while (t < duration) {
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float lerp = Mathf.Clamp01(t / duration);
            _fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            yield return null;
        }
        _fadeGroup.alpha = targetAlpha;
        _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
    }
    #endregion
}
