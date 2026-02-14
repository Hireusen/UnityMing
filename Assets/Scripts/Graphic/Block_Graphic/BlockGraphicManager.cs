using UnityEngine;

/// <summary>
/// 블록 그래픽 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 블록 그래픽 스크립트를 호출합니다.
/// </summary>
public class BlockGraphicManager : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private BlockBatchBuilder _blockBatchBuilder;
    [SerializeField] private BlockPainter _blockPainter;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Verification()
    {
        De.IsNull(_blockBatchBuilder);
        De.IsNull(_blockPainter);
        _blockBatchBuilder.Verification();
        _blockPainter.Verification();
    }

    public void DataBuilder()
    {

    }

    public void Initialize()
    {
        _blockBatchBuilder.Initialize();
    }

    public void RunBeforeFrame()
    {

    }

    public void RunAfterFrame()
    {
        _blockBatchBuilder.RunAfterFrame();
        _blockPainter.RunAfterFrame();
    }
    #endregion
}
