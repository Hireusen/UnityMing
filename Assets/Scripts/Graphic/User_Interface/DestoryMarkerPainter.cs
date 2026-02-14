using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// designs에서 EOrderType.Destroy인 블록에 빨간색 테두리를 매 프레임 그립니다.
/// LineRenderer 풀링으로 동적 개수를 처리합니다.
/// </summary>
public class DestroyMarkerPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("사용자 정의 설정")]
    [SerializeField] private Color _markerColor = new Color(1f, 0.2f, 0.2f, 0.6f);
    [SerializeField] private float _lineWidth = 0.06f;
    [SerializeField] private float _zDepth = -34f;
    [SerializeField] private int _initialPoolSize = 16;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private List<LineRenderer> _pool;
    private Material _mat;
    private int _activeCount;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification() { }

    public void Initialize()
    {
        _mat = new Material(Shader.Find("Sprites/Default"));
        _pool = new List<LineRenderer>(_initialPoolSize);
        _activeCount = 0;
        for (int i = 0; i < _initialPoolSize; ++i)
            _pool.Add(CreateLine(i));
    }

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

    public void RunAfterFrame()
    {
        GameData game = GameData.ins;
        if (game == null || game.Player == null) {
            HideAll();
            return;
        }

        var designs = game.Player.designs;
        BlockMap blockMap = game.Blocks;
        Dictionary<EBlock, SO_Block> soDb = game.BlockDatabase;

        int used = 0;

        foreach (var kvp in designs) {
            BuildOrder order = kvp.Value;
            if (order.type != EOrderType.Destroy)
                continue;

            int centerIndex = order.index;

            // 블록 크기 → 렌더링 영역 계산
            float sizeX = 1f;
            float sizeY = 1f;
            if (soDb.TryGetValue(order.id, out SO_Block so)) {
                sizeX = so.Size.x;
                sizeY = so.Size.y;
            }
            Vector2Int size = new Vector2Int((int)sizeX, (int)sizeY);
            Vector2 renderPos = blockMap.GetRenderPos(centerIndex, size, order.rotation);

            float cx = renderPos.x;
            float cy = renderPos.y;
            // 회전 시 시각적 크기 스왑
            float hw, hh;
            if (order.rotation == ERotation.Right || order.rotation == ERotation.Left) {
                hw = sizeY * 0.5f;
                hh = sizeX * 0.5f;
            } else {
                hw = sizeX * 0.5f;
                hh = sizeY * 0.5f;
            }

            float z = _zDepth;
            LineRenderer line = GetLine(used);
            line.enabled = true;
            line.SetPosition(0, new Vector3(cx - hw, cy - hh, z));
            line.SetPosition(1, new Vector3(cx + hw, cy - hh, z));
            line.SetPosition(2, new Vector3(cx + hw, cy + hh, z));
            line.SetPosition(3, new Vector3(cx - hw, cy + hh, z));
            ++used;
        }

        // 사용하지 않은 LineRenderer 비활성화
        for (int i = used; i < _activeCount; ++i) {
            if (i < _pool.Count)
                _pool[i].enabled = false;
        }
        _activeCount = used;
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
