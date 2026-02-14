using System.Collections.Generic;
using UnityEngine;
public enum ESound
{
    None = 0,
    BlockBuild_1 = 1,
    BlockBuild_2 = 2,
    BlockBuild_3 = 3,
    BlockDestory_1 = 4,
    BlockDestory_2 = 5,
    BlockDestory_3 = 6
}
/// <summary>
/// 사운드 매니저 오브젝트에 부착하는 C# 스크립트입니다.
/// 등록된 사운드 클립을 이름으로 검색하여 재생합니다.
/// </summary>
public class SoundAdmin : MonoBehaviour
{
    [System.Serializable]
    public struct ClipPair
    {
        public ESound name;
        public AudioClip clip;
    }
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("필수 요소 등록")]
    [SerializeField] private ClipPair[] _clipPairs;
    [SerializeField] private AudioSource _source;
    [Header("사용자 정의 설정")]
    [SerializeField] private float _volume = 1f;
    [SerializeField] private bool _usePitch = true; // 사운드 울리기
    [SerializeField] private Vector2 _pitchRange = new Vector2(0.95f, 1.05f);
    #endregion
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Dictionary<ESound, AudioClip> _clips;
    #endregion
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 인스펙터 유효성 검사
    public void Verification()
    {
        De.IsNull(_source);
        De.IsNull(_clipPairs);
    }
    // 외부에 전달할 데이터 생성
    public void DataBuilder()
    {
        // 딕셔너리로 변경하기
        int length = _clipPairs.Length;
        _clips = new Dictionary<ESound, AudioClip>(length);
        for (int i = 0; i < length; ++i) {
            if (_clipPairs[i].clip == null)
                continue;
            _clips.Add(_clipPairs[i].name, _clipPairs[i].clip);
        }
    }
    /// <summary>
    /// 지정한 사운드를 한 번 재생합니다.
    /// </summary>
    public void PlaySound(ESound sound)
    {
        if (!_clips.TryGetValue(sound, out AudioClip clip))
            return;
        if (_usePitch) {
            _source.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        } else {
            _source.pitch = 1f;
        }
        _source.PlayOneShot(clip, _volume);
    }
    /// <summary>
    /// 지정한 사운드를 볼륨을 지정하여 한 번 재생합니다.
    /// </summary>
    public void PlaySound(ESound sound, float volumeScale)
    {
        if (!_clips.TryGetValue(sound, out AudioClip clip))
            return;
        if (_usePitch) {
            _source.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        } else {
            _source.pitch = 1f;
        }
        _source.PlayOneShot(clip, volumeScale);
    }
    #endregion
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    #endregion
}
