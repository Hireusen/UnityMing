using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// F키 드래그 중 복사 영역을 노란색 테두리 사각형으로 표시합니다.
/// </summary>
public class CopyAreaPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("사용자 정의 설정")]
    [SerializeField] private Color _borderColor = new Color(1f, 0.9f, 0.3f, 0.7f);
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
        if (!player.copyActive) {
            _line.enabled = false;
            return;
        }

        _line.enabled = true;

        float left = player.copyMinX;
        float right = player.copyMaxX + 1f;
        float bottom = player.copyMinY;
        float top = player.copyMaxY + 1f;

        _line.SetPosition(0, new Vector3(left, bottom, 0f));
        _line.SetPosition(1, new Vector3(right, bottom, 0f));
        _line.SetPosition(2, new Vector3(right, top, 0f));
        _line.SetPosition(3, new Vector3(left, top, 0f));
    }
    #endregion
}
