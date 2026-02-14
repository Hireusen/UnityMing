using UnityEngine;

/// <summary>
/// 유저 인터페이스 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// SelectedBatchBuilder가 구성한 배치 데이터를 DrawMeshInstanced로 그립니다.
/// </summary>
public class SelectedPainter : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private SelectedBatchBuilder _batchBuilder;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_mesh);
        De.IsNull(_batchBuilder);
    }

    public void RunAfterFrame()
    {
        UGraphic.DrawBatches(
            _mesh,
            _batchBuilder.BatchMap,
            _batchBuilder.ActiveKeys,
            _batchBuilder.MaterialCache
        );
    }
    #endregion
}
