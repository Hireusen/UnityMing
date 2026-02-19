using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy 디자인 블록에 빨간색 테두리를 매 프레임 그립니다.
/// LineRenderer 풀링으로 동적 개수를 처리합니다.
/// </summary>
public class DestroyMarkerPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("사용자 정의 설정")]
    [SerializeField] private Color _markerColor = new Color(1f, 0.2f, 0.2f, 0.6f);
    [SerializeField] private float _lineWidth = 0.06f;
    [SerializeField] private int _initialPoolSize = 16;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private List<LineRenderer> _pool;
    private Material _mat;
    private int _activeCount;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 외부 공개 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification() { }

    public void Initialize()
    {
        _mat = new Material(Shader.Find("Sprites/Default"));
        _pool = new List<LineRenderer>(_initialPoolSize);
        _activeCount = 0;
        for (int i = 0; i < _initialPoolSize; ++i)
            _pool.Add(CreateLine(i));
    }

    /// <summary>
    /// 매 프레임 Destroy 디자인을 순회하여 마커를 갱신합니다.
    /// </summary>
    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (game == null || game.Player == null) {
            HideAll();
            return;
        }

        var designs = game.Player.designs;
        BlockMap blockMap = game.Blocks;
        var blockSO = game.BlockDatabase;
        int used = 0;

        foreach (BuildOrder order in designs.Values) {
            if (order.type != EOrderType.Destroy)
                continue;

            // 블록 크기 조회
            float sizeX = 1f;
            float sizeY = 1f;
            if (blockSO.TryGetValue(order.id, out SO_Block so)) {
                sizeX = so.Size.x;
                sizeY = so.Size.y;
            }

            Vector2Int size = new Vector2Int((int)sizeX, (int)sizeY);
            Vector2 renderPos = blockMap.GetRenderPos(order.index, size, order.rotation);

            // 회전 시 시각적 크기 스왑
            float halfWidth, halfHeight;
            if (order.rotation == ERotation.Right || order.rotation == ERotation.Left) {
                halfHeight = sizeX * 0.5f;
                halfWidth = sizeY * 0.5f;
            } else {
                halfWidth = sizeX * 0.5f;
                halfHeight = sizeY * 0.5f;
            }

            // 사각형 그리기
            float z = Const.DEMOLISH_MARKER;
            LineRenderer line = GetLine(used);
            line.enabled = true;
            line.SetPosition(0, new Vector3(renderPos.x - halfWidth, renderPos.y - halfHeight, z));
            line.SetPosition(1, new Vector3(renderPos.x + halfWidth, renderPos.y - halfHeight, z));
            line.SetPosition(2, new Vector3(renderPos.x + halfWidth, renderPos.y + halfHeight, z));
            line.SetPosition(3, new Vector3(renderPos.x - halfWidth, renderPos.y + halfHeight, z));
            ++used;
        }

        // 미사용 비활성화
        for (int i = used; i < _activeCount; ++i) {
            if (i < _pool.Count)
                _pool[i].enabled = false;
        }
        _activeCount = used;
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private LineRenderer CreateLine(int idx)
    {
        GameObject go = new GameObject($"DestroyMarker_{idx}");
        go.transform.SetParent(transform);
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.loop = true;
        line.positionCount = 4;
        line.startWidth = _lineWidth;
        line.endWidth = _lineWidth;
        line.sortingOrder = 100;
        line.material = _mat;
        line.startColor = _markerColor;
        line.endColor = _markerColor;
        line.enabled = false;
        return line;
    }

    private LineRenderer GetLine(int index)
    {
        while (index >= _pool.Count)
            _pool.Add(CreateLine(_pool.Count));
        return _pool[index];
    }

    private void HideAll()
    {
        for (int i = 0; i < _activeCount; ++i) {
            if (i < _pool.Count)
                _pool[i].enabled = false;
        }
        _activeCount = 0;
    }
    #endregion
}
