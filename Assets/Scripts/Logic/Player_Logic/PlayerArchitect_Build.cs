using UnityEngine;
/// <summary>
/// 디자인 블록을 플레이어 로직에 따라 건설하기
/// </summary>
public partial class PlayerArchitect
{
    private void UpdateBuildState()
    {
        // 건설 상태
        if (Time.time < _player.nextBuildTime) {
            _player.buildState = EBuildState.Build;
            return;
        }
        _player.buildState = EBuildState.None;
    }
    private void TryDesignToBlock()
    {
        // 건설 쿨타임 체크
        if (Time.time < _player.nextBuildTime)
            return;
        _player.nextBuildTime = Time.time + _player.buildInterval;
        // 거리 내에 있는 블록 가져오기
        BuildOrder target = new BuildOrder(-1, EOrderType.None, EBlock.None, ERotation.None);
        Vector2 playerPos = _player.pos;
        float buildRange = _player.buildRange;
        foreach (var item in _designs.Values) {
            if (item.type != EOrderType.Build) {
                De.Print("빌드가 아니다.");
                continue;
            }
            target.index = item.index;
            Vector2 targetPos = UGrid.IndexToWorldV(target.index, _width);
            if (UMath.IsWithinDistance(playerPos, targetPos, buildRange)) {
                De.Print("거리 내 발견 성공");
                target.id = item.id;
                target.rotate = item.rotate;
                break;
            } else {
                De.Print("거리 내 발견 실패 ...");
            }
        }
        De.Print("찾았나?" + _player.buildState);
        if (target.id == EBlock.None)
            return;
        // 건설
        De.Print("건설하자!" + _player.buildState);
        _blockMap.TryPlace(target.index, target.id, target.rotate);
        _designs.Remove(target.index);
    }
}
