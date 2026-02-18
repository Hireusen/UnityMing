using static Const;
using UnityEngine;

/// <summary>
/// 건설/철거 완료 시 테두리 사각형이 확대되며 사라지는 이펙트를 표시합니다.
/// PlayerArchitect.OnBuildEffect / OnDestroyEffect 이벤트를 구독합니다.
/// </summary>
public class BuildEffectPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private PlayerArchitect _playerArchitect;

    [Header("사용자 정의 설정")]
    [SerializeField] private Color _buildColor = new Color(1f, 0.85f, 0.3f, 0.8f);
    [SerializeField] private Color _destroyColor = new Color(1f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _expandSize = 1.0f;
    [SerializeField] private float _lineWidth = 0.06f;
    [SerializeField] private int _poolSize = 12;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private struct EffectInstance
    {
        public LineRenderer line;
        public float startTime;
        public Vector2 center;
        public float startHalfW;
        public float startHalfH;
        public Color color;
        public bool active;
    }

    private EffectInstance[] _pool;
    private int _nextIndex;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 공개 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification() {
        De.IsNull(_playerArchitect);
    }

    /// <summary>
    /// LineRenderer 풀 생성 및 이벤트 구독
    /// </summary>
    public void Initialize()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        _pool = new EffectInstance[_poolSize];

        for (int i = 0; i < _poolSize; ++i) {
            GameObject go = new GameObject($"BuildEffect_{i}");
            go.transform.SetParent(transform);

            LineRenderer line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.loop = true;
            line.positionCount = 4;
            line.startWidth = _lineWidth;
            line.endWidth = _lineWidth;
            line.sortingOrder = 102;
            line.material = mat;
            line.enabled = false;

            _pool[i] = new EffectInstance { line = line, active = false };
        }
        _nextIndex = 0;

        _playerArchitect.OnBuildEffect += OnBuild;
        _playerArchitect.OnDestroyEffect += OnDestroyed;
    }

    /// <summary>
    /// 매 프레임 활성 이펙트의 크기·알파를 갱신합니다.
    /// </summary>
    public void RunAfterFrame()
    {
        float now = Time.time;
        float z = BUILD_EFFECT;

        for (int i = 0; i < _poolSize; ++i) {
            ref EffectInstance inst = ref _pool[i];
            if (!inst.active)
                continue;

            float elapsed = now - inst.startTime;
            if (elapsed >= _duration) {
                inst.active = false;
                inst.line.enabled = false;
                continue;
            }

            // 진행률
            float t = elapsed / _duration;

            // 크기 확장
            float hw = inst.startHalfW + _expandSize * t;
            float hh = inst.startHalfH + _expandSize * t;

            // 알파 감쇠
            Color c = inst.color;
            c.a = Mathf.Lerp(inst.color.a, 0f, t);
            inst.line.startColor = c;
            inst.line.endColor = c;

            float cx = inst.center.x;
            float cy = inst.center.y;
            inst.line.SetPosition(0, new Vector3(cx - hw, cy - hh, z));
            inst.line.SetPosition(1, new Vector3(cx + hw, cy - hh, z));
            inst.line.SetPosition(2, new Vector3(cx + hw, cy + hh, z));
            inst.line.SetPosition(3, new Vector3(cx - hw, cy + hh, z));
        }
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void OnBuild(Vector2 center, float sizeX, float sizeY)
    {
        Spawn(center, sizeX, sizeY, _buildColor);
    }

    private void OnDestroyed(Vector2 center, float sizeX, float sizeY)
    {
        Spawn(center, sizeX, sizeY, _destroyColor);
    }

    private void Spawn(Vector2 center, float sizeX, float sizeY, Color color)
    {
        ref EffectInstance inst = ref _pool[_nextIndex];
        inst.center = center;
        inst.startHalfW = sizeX * 0.5f;
        inst.startHalfH = sizeY * 0.5f;
        inst.startTime = Time.time;
        inst.color = color;
        inst.active = true;
        inst.line.enabled = true;
        inst.line.startColor = color;
        inst.line.endColor = color;
        _nextIndex = (_nextIndex + 1) % _poolSize;
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void OnDisable()
    {
        _playerArchitect.OnBuildEffect -= OnBuild;
        _playerArchitect.OnDestroyEffect -= OnDestroyed;
    }
    #endregion
}
