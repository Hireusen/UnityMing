using UnityEngine;
/// <summary>
/// 플레이어 로직 매니저 오브젝트에 부착하는 스크립트입니다.
/// 플레이어 데이터 생성과 업데이트를 담당합니다.
/// </summary>
public class PlayerDataBuilder : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("사용자 정의 설정")]
    [SerializeField] private float _buildRange = 20f;
    [SerializeField] private float _buildInterval = 0.06f;
    #endregion

    private PlayerSingle _player;
    private Camera _camera;

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    public void Initialize()
    {
        _player = GameData.ins.Player;
        _camera = Camera.main;
    }

    public void DataBuilder() {
        GameData game = GameData.ins;
        game.Player = new PlayerSingle(game.TileMap.Width, game.TileMap.Height, _buildRange, _buildInterval);
    }

    public void RunBeforeFrame()
    {
        _player.UpdateCursor(_camera);
    }
    #endregion
}
