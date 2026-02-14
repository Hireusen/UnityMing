using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 데이터를 담는 스크립터블 오브젝트입니다.
/// </summary>
public abstract class SO_Matter : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] protected SpriteInfo[] _sprite;
    [SerializeField] protected string _label;
    public string Label => _label;
    

    /// <summary>
    /// 스프라이트 구조체 정보를 반환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ref readonly SpriteInfo GetSpriteInfo(int index)
    {
         return ref _sprite[index];
    }

    /// <summary>
    /// 스프라이트 구조체 정보를 반환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ref readonly SpriteInfo[] GetSpriteInfoAll()
    {
         return ref _sprite;
    }

    /// <summary>
    /// 스프라이트 정보를 반환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite GetSprite(int index)
    {
        return _sprite[index].sprite;
    }

    /// <summary>
    /// 텍스처 정보를 반환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Texture2D GetTexture(int index)
    {
        return _sprite[index].sprite.texture;
    }

    public int SpriteCount => _sprite.Length;

    // 개발자가 값을 제대로 입력했는지 검사하고 조정합니다.
    protected virtual void OnValidate()
    {
        // 스프라이트 구조체 검증
        UArray.IsInitedArray(_sprite);
        int length = _sprite.Length;
        for (int i = 0; i < length; ++i) {
            var sp = _sprite[i];
            De.IsNull(sp.sprite);
            De.IsTrue(sp.type == ESpriteType.None);

            sp.offset.x = Mathf.Clamp(sp.offset.x, -10f, 10f);
            sp.offset.y = Mathf.Clamp(sp.offset.y, -10f, 10f);
            sp.offset.z = Mathf.Clamp(sp.offset.z, -0.0009f, 0f);
            _sprite[i] = sp;
        }
    }
}
