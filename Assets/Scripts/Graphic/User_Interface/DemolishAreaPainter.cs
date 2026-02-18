using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 우클릭 드래그 중 철거 영역을 빨간 테두리 사각형으로 표시합니다.
/// LineRenderer를 사용하여 4개 꼭짓점을 잇는 닫힌 사각형을 그립니다.
/// </summary>
public class DemolishAreaPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("사용자 정의 설정")]
    [SerializeField] private Color _borderColor = new Color(1f, 0.2f, 0.2f, 0.7f);
    [SerializeField] private float _lineWidth = 0.08f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private LineRenderer _line;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification() { }

    public void Initialize()
    {
        _line = gameObject.AddComponent<LineRenderer>();
        _line.useWorldSpace = true;
        _line.loop = true;
        _line.positionCount = 4;
        _line.startWidth = _lineWidth;
        _line.endWidth = _lineWidth;
        _line.sortingOrder = 100;
        // 단색 머티리얼 (Sprites/Default는 유니티 내장)
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = _borderColor;
        _line.endColor = _borderColor;
        _line.enabled = false;
    }

    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (game == null || game.Player == null) {
            _line.enabled = false;
            return;
        }

        PlayerSingle player = game.Player;
        if (!player.demolishActive) {
            _line.enabled = false;
            return;
        }

        _line.enabled = true;

        // 그리드 좌표 → 월드 좌표
        // 셀 (minX, minY) ~ (maxX, maxY) 영역의 테두리
        // 셀의 왼쪽 아래가 정수 좌표이므로, 영역의 외곽은 +1
        float left = player.demolishMinX;
        float right = player.demolishMaxX + 1f;
        float bottom = player.demolishMinY;
        float top = player.demolishMaxY + 1f;
        float z = Const.DEMOLISH_SQUARE; // LineRenderer는 sortingOrder로 깊이 제어

        _line.SetPosition(0, new Vector3(left, bottom, z));
        _line.SetPosition(1, new Vector3(right, bottom, z));
        _line.SetPosition(2, new Vector3(right, top, z));
        _line.SetPosition(3, new Vector3(left, top, z));
    }
    #endregion
}
