using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.EventSystems;

public static class Tool
{
    #region 입력 & 좌표
    /// <summary>
    /// 화면 좌표를 월드 좌표(Z = 0)로 변환합니다.
    /// 카메라의 원근, 직교 여부에 상관없이 정확하게 동작합니다.
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public static Vector2 GetWorldPosition2D(this Camera camera, Vector3 screenPosition)
    {
        if (camera == null)
            return Vector2.zero;
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = camera.ScreenPointToRay(screenPosition);
        if (plane.Raycast(ray, out float distance)) {
            return (Vector2)ray.GetPoint(distance);
        }
        screenPosition.z = -camera.transform.position.z;
        return (Vector2)camera.ScreenToWorldPoint(screenPosition);
    }

    /// <summary>
    /// [오버로딩] 입력 좌표를 넣지 않으면 자동으로 현재 마우스 좌표를 사용합니다.
    /// (Legacy Input과 New Input System 모두 대응하기 위해 분리함)
    /// </summary>
    public static Vector2 GetMouseWorldPos(this Camera camera)
    {
#if ENABLE_INPUT_SYSTEM
        // 새로운 Input System 패키지를 쓴다면
        return GetWorldPosition2D(camera, UnityEngine.InputSystem.Mouse.current.position.ReadValue());
#else
        // 기존 Input Manager를 쓴다면
        return GetWorldPosition2D(camera, Input.mousePosition);
#endif
    }

    /// <summary>
    /// [유틸리티] 그리드(타일) 스냅 좌표를 반환합니다.
    /// 예: (1.2, 3.8) -> (1, 4) 또는 (1, 3) 
    /// </summary>
    /// <param name="cellSize">타일 크기 (기본 1)</param>
    public static Vector2Int GetGridPos(this Camera camera, float cellSize = 1.0f)
    {
        Vector2 worldPos = camera.GetMouseWorldPos();
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.y / cellSize)
        );
    }

    /// <summary>
    /// [유틸리티] 마우스가 UI 위에 있는지 확인합니다. (클릭 관통 방지)
    /// </summary>
    public static bool IsPointerOverUI()
    {
        // PC/모바일 모두 작동하는 표준 방식
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// 2D 게임에서 해당 카메라에서 마우스가 가리키는 월드 좌표를 반환합니다.
    /// </summary>
    public static Vector2 GetMousePos(Camera camera)
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = camera.transform.position.z;
        return (Vector2)camera.ScreenToWorldPoint(mouse);
    }

    /// <summary>
    /// 스크린 좌표를 2D 월드 좌표로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ScreenToWorld2D(Camera camera, Vector2 screenPos)
    {
        Vector3 pos = new Vector3(screenPos.x, screenPos.y, -camera.transform.position.z);
        return (Vector2)camera.ScreenToWorldPoint(pos);
    }

    /// <summary>
    /// 2D 월드 좌표를 스크린 좌표로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 WorldToScreen2D(Camera camera, Vector2 worldPos)
    {
        return (Vector2)camera.WorldToScreenPoint(new Vector3(worldPos.x, worldPos.y, 0f));
    }
    #endregion


    // ================================================================
    //  컴포넌트 · 게임오브젝트
    // ================================================================

    /// <summary>
    /// 지정한 타입의 컴포넌트를 가져오거나 없으면 추가합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    /// <summary>
    /// 부모 Transform의 모든 자식을 파괴합니다.
    /// </summary>
    public static void DestroyAllChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; --i) {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 부모 Transform의 모든 자식을 즉시 파괴합니다. (에디터 전용)
    /// </summary>
    public static void DestroyAllChildrenImmediate(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; --i) {
            UnityEngine.Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 게임오브젝트의 활성 상태가 다를 때만 SetActive를 호출합니다.
    /// 불필요한 SetActive 호출로 인한 오버헤드를 방지합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetActiveIfNeeded(GameObject go, bool active)
    {
        if (go == null) return;
        if (go.activeSelf != active)
            go.SetActive(active);
    }

    // ================================================================
    //  레이어 · 태그
    // ================================================================

    /// <summary>
    /// 레이어 이름으로 LayerMask 비트를 생성합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LayerToBit(string layerName)
    {
        return 1 << LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// 여러 레이어 이름으로 복합 LayerMask 비트를 생성합니다.
    /// </summary>
    public static int LayersToBit(params string[] layerNames)
    {
        int mask = 0;
        for (int i = 0; i < layerNames.Length; ++i)
            mask |= 1 << LayerMask.NameToLayer(layerNames[i]);
        return mask;
    }

    /// <summary>
    /// 대상 게임오브젝트가 지정한 레이어에 속하는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInLayer(GameObject go, int layerMask)
    {
        if (go == null) return false;
        return (layerMask & (1 << go.layer)) != 0;
    }

    // ================================================================
    //  색상
    // ================================================================

    /// <summary>
    /// 색상의 알파값만 변경한 복사본을 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    /// <summary>
    /// HEX 문자열(#RRGGBB 또는 RRGGBB)을 Color로 변환합니다.
    /// 실패하면 Color.white를 반환합니다.
    /// </summary>
    public static Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.white;
        if (hex[0] == '#')
            hex = hex.Substring(1);
        if (hex.Length < 6)
            return Color.white;
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            return color;
        return Color.white;
    }

    // ================================================================
    //  타이머 · 쿨다운
    // ================================================================

    /// <summary>
    /// 쿨다운이 끝났는지 검사합니다.
    /// Time.time 기준으로 nextTime보다 현재 시간이 크면 true를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCooldownReady(float nextTime)
    {
        return nextTime <= Time.time;
    }

    /// <summary>
    /// 쿨다운이 끝났는지 검사합니다.
    /// Time.unscaledTime 기준으로 nextTime보다 현재 시간이 크면 true를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCooldownReadyUnscaled(float nextTime)
    {
        return nextTime <= Time.unscaledTime;
    }

    /// <summary>
    /// 쿨다운을 소비합니다.
    /// 쿨다운이 끝났으면 nextTime을 갱신하고 true를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCooldown(ref float nextTime, float interval)
    {
        if (Time.time < nextTime)
            return false;
        nextTime = Time.time + interval;
        return true;
    }

    /// <summary>
    /// 스케일 영향을 받지 않는 쿨다운을 소비합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCooldownUnscaled(ref float nextTime, float interval)
    {
        if (Time.unscaledTime < nextTime)
            return false;
        nextTime = Time.unscaledTime + interval;
        return true;
    }

    // ================================================================
    //  확률 · 가중치 랜덤
    // ================================================================

    /// <summary>
    /// 0~1 범위의 확률 검사입니다. chance가 0.3이면 30% 확률로 true를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Chance(float chance)
    {
        return UnityEngine.Random.value < chance;
    }

    /// <summary>
    /// 가중치 배열에서 가중치 기반 랜덤 인덱스를 반환합니다.
    /// 모든 가중치가 0 이하이면 -1을 반환합니다.
    /// </summary>
    public static int WeightedRandom(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;
        float total = 0f;
        for (int i = 0; i < weights.Length; ++i) {
            if (weights[i] > 0f)
                total += weights[i];
        }
        if (total <= 0f) return -1;
        float random = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; ++i) {
            if (weights[i] <= 0f) continue;
            cumulative += weights[i];
            if (random < cumulative)
                return i;
        }
        return weights.Length - 1;
    }

    /// <summary>
    /// int 가중치 배열에서 가중치 기반 랜덤 인덱스를 반환합니다.
    /// 모든 가중치가 0 이하이면 -1을 반환합니다.
    /// </summary>
    public static int WeightedRandom(int[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;
        int total = 0;
        for (int i = 0; i < weights.Length; ++i) {
            if (weights[i] > 0)
                total += weights[i];
        }
        if (total <= 0) return -1;
        int random = UnityEngine.Random.Range(0, total);
        int cumulative = 0;
        for (int i = 0; i < weights.Length; ++i) {
            if (weights[i] <= 0) continue;
            cumulative += weights[i];
            if (random < cumulative)
                return i;
        }
        return weights.Length - 1;
    }

    // ================================================================
    //  문자열
    // ================================================================

    /// <summary>
    /// 큰 숫자를 축약 표기합니다. (1000 → 1K, 1500000 → 1.5M)
    /// </summary>
    public static string ToAbbreviated(long number)
    {
        if (number < 1000L)
            return number.ToString();
        if (number < 1000000L)
            return (number / 1000f).ToString("0.#") + "K";
        if (number < 1000000000L)
            return (number / 1000000f).ToString("0.#") + "M";
        return (number / 1000000000f).ToString("0.#") + "B";
    }

    /// <summary>
    /// 초를 MM:SS 형식의 문자열로 변환합니다.
    /// </summary>
    public static string ToTimeMMSS(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);
        return $"{min:D2}:{sec:D2}";
    }

    /// <summary>
    /// 초를 HH:MM:SS 형식의 문자열로 변환합니다.
    /// </summary>
    public static string ToTimeHHMMSS(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        int hour = (int)(seconds / 3600f);
        int min = (int)((seconds % 3600f) / 60f);
        int sec = (int)(seconds % 60f);
        return $"{hour:D2}:{min:D2}:{sec:D2}";
    }

    // ================================================================
    //  스프라이트렌더러 · UI
    // ================================================================

    /// <summary>
    /// SpriteRenderer의 알파값을 변경합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetSpriteAlpha(SpriteRenderer renderer, float alpha)
    {
        if (renderer == null) return;
        Color c = renderer.color;
        c.a = alpha;
        renderer.color = c;
    }

    /// <summary>
    /// CanvasGroup의 표시/숨김을 한 번에 처리합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetCanvasGroupVisible(CanvasGroup group, bool visible)
    {
        if (group == null) return;
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    // ================================================================
    //  2D 물리
    // ================================================================

    /// <summary>
    /// 2D 원형 범위 안에 있는 모든 콜라이더를 NonAlloc으로 검출합니다.
    /// 결과 배열은 내부에서 재사용됩니다. 즉시 사용하세요.
    /// </summary>
    private static readonly Collider2D[] _overlapBuffer = new Collider2D[64];

    public static int OverlapCircle2D(Vector2 center, float radius, int layerMask, Collider2D[] resultBuffer)
    {
        if (resultBuffer == null) return 0;
        return Physics2D.OverlapCircleNonAlloc(center, radius, resultBuffer, layerMask);
    }

    /// <summary>
    /// 2D 원형 범위 안에 있는 모든 콜라이더를 내부 버퍼로 검출합니다.
    /// 반환된 count만큼 GetOverlapBuffer()로 접근하세요.
    /// </summary>
    public static int OverlapCircle2D(Vector2 center, float radius, int layerMask)
    {
        return Physics2D.OverlapCircleNonAlloc(center, radius, _overlapBuffer, layerMask);
    }

    /// <summary>
    /// 내부 오버랩 버퍼를 반환합니다.
    /// OverlapCircle2D(center, radius, layerMask)와 함께 사용하세요.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Collider2D[] GetOverlapBuffer()
    {
        return _overlapBuffer;
    }

    // ================================================================
    //  애플리케이션
    // ================================================================

    public static void GameStop()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
