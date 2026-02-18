using System.Collections.Generic;
using System;
/// <summary>
/// 게임 데이터에서 호출하는 C# 스크립트입니다.
/// 런타임에서 사용하는 각종 데이터를 저장하거나 꺼낼 수 있는 클래스입니다. 
/// 배열을 필요한 만큼만 확장하여 데이터를 저장합니다.
/// </summary>
public class PoolManagement<T> where T : struct
{
    protected T[] _dataList;
    protected bool[] _isActive;
    protected Stack<int> _freeIndex;
    protected int _count;
    protected int _nextIndex;

    public PoolManagement(int capacity = byte.MaxValue)
    {
        Initialize(capacity);
    }

    /// <summary>
    /// 현재 사용중인 데이터 개수
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// 현재까지 데이터가 가장 많이 생성되었을 때의 개수
    /// </summary>
    public int Capacity => _nextIndex;

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    /// <summary>
    /// 새로운 데이터 등록
    /// 생성 실패했을 경우 -1을 반환
    public int Create(T data)
    {
        // 유닛 수 최대치 도달
        if (De.IsTrue(short.MaxValue < _nextIndex)) {
            return -1;
        }
        int newIndex;
        // 재사용할 인덱스가 있음
        if (0 < _freeIndex.Count) {
            newIndex = _freeIndex.Pop();
        }
        // 새로운 인덱스
        else {
            // 공간 부족
            if (_dataList.Length <= _nextIndex) {
                
                if (!UArray.TryResizeArray(ref _dataList, 1.5d)) { // 배열 확장 시도
                    return -1;
                }
                UArray.TryResizeArray(ref _isActive, 1.5d);
            }
            // 배열 확장 성공
            newIndex = _nextIndex++;
        }
        // 데이터 저장
        _count++;
        _dataList[newIndex] = data;
        _isActive[newIndex] = true;
        return newIndex;
    }

    /// <summary>
    /// 해당 인덱스를 풀에 반납 (데이터 삭제 처리)
    /// </summary>
    public void Destroy(int index)
    {
        _count--;
        _freeIndex.Push(index);
        _isActive[index] = false;
    }

    /// <summary>
    /// 해당 인덱스에 데이터가 존재할 경우
    /// </summary>
    public bool IsExist(int index)
    {
        return _isActive[index];
    }

    /// <summary>
    /// 수정할 수 없는 데이터를 반환
    /// </summary>
    public ref readonly T Read(int index) => ref _dataList[index];

    /// <summary>
    /// 수정 가능한 데이터를 반환
    /// </summary>
    public ref T GetRef(int index) => ref _dataList[index];

    /// <summary>
    /// 모든 인덱스를 풀에 반납
    /// </summary>
    public void Clear()
    {
        _count = 0;
        for (int i = 0; i < _nextIndex; ++i) {
            if (IsExist(i)) {
                Destroy(i);
            }
        }
    }

    /// <summary>
    /// 메모리를 새로 할당하여 클래스를 완전히 초기화
    /// </summary>
    public void Initialize(int capacity = byte.MaxValue)
    {
        _dataList = new T[capacity];
        _isActive = new bool[capacity];
        Array.Fill<bool>(_isActive, false);
        _freeIndex = new Stack<int>(capacity);
        _count = 0;
        _nextIndex = 0;
    }
    #endregion
}
