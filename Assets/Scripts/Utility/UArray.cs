using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// 배열과 리스트를 다루는 유틸리티입니다.
/// </summary>
public static class UArray
{
    /// <summary>
    /// 배열 크기를 조정하며, 성공 여부에 따라 True 또는 False를 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetArray"></param>
    /// <param name="multiplySize"></param>
    /// <returns></returns>
    public static bool TryResizeArray<T>(ref T[] targetArray, double multiplySize)
    {
        if (De.IsNull(targetArray, LogType.Exception))
            return false;
        long newSize = (long)(targetArray.Length * multiplySize);
        if (int.MaxValue < newSize) {
            if (newSize <= 0) {
                De.Print("배열의 크기가 0 이하입니다.", LogType.Exception);
                return false;
            }
            if (targetArray.Length == int.MaxValue) {
                De.Print("더 이상 배열을 확장할 수 없습니다.", LogType.Exception);
                return false;
            }
            newSize = (long)int.MaxValue;
        }
        try {
            Array.Resize(ref targetArray, (int)newSize);
            return true;
        }
        catch (OutOfMemoryException) {
            De.Print("메모리가 부족하여 배열을 확장할 수 없습니다.");
            return false;
        }
    }

    /// <summary>
    /// 피셔 예이츠 셔플로 배열을 무작위로 섞습니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(this T[] array)
    {
        if (array == null)
            return;
        int length = array.Length - 1;
        for (int i = length; i > 0; --i) {
            int j = UnityEngine.Random.Range(0, i + 1);

            T tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }

    /// <summary>
    /// 피셔 예이츠 셔플로 리스트를 무작위로 섞습니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list == null)
            return;
        int length = list.Count - 1;
        for (int i = length; i > 0; --i) {
            int j = UnityEngine.Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    /// <summary>
    /// 리스트의 마지막 요소와 교체하고, 마지막 요소를 삭제합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SwapLastAndRemove<T>(this List<T> list, int index)
    {
        if (list == null)
            return;
        int last = list.Count - 1;
        if (index < 0 || last < index)
            return;
        list[index] = list[last];
        list.RemoveAt(last);
    }

    /// <summary>
    /// 리스트와 각 요소가 모두 초기화되어 있다면 True를 반환합니다.
    /// False일 경우 De 클래스로 로그를 출력합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsInitedList<T>(List<T> list)
    {
        if (De.IsNull(list, LogType.Exception))
            return false;
        int count = list.Count;
        if (De.IsTrue(count <= 0, LogType.Exception))
            return false;
        if (De.IsTrue(list.Capacity <= 0, LogType.Exception))
            return false;
        for (int i = 0; i < count; ++i) {
            if (De.IsNull(list[i], LogType.Exception))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 1차원 배열과 각 요소가 모두 초기화되어 있다면 True를 반환합니다.
    /// False일 경우 De 클래스로 로그를 출력합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool IsInitedArray<T>(T[] array)
    {
        if (De.IsNull(array, LogType.Exception))
            return false;
        int length = array.Length;
        if (De.IsTrue(length == 0, LogType.Exception))
            return false;
        // 값 형식
        if (typeof(T).IsValueType)
            return true;
        // 참조 형식
        for (int i = 0; i < length; ++i) {
            if (De.IsNull(array[i], LogType.Exception))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 인덱스가 배열 범위 안에 있는지 검사합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InBounds<T>(T[] array, int index)
    {
        if (array == null)
            return false;
        if (index < array.Length)
            return false;
        if (0 <= index)
            return false;
        return true;
    }

    /// <summary>
    /// 인덱스가 리스트 범위 안에 있는지 검사합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InBounds<T>(List<T> list, int index)
    {
        if (list == null)
            return false;
        if (index < list.Count)
            return false;
        if (0 <= index)
            return false;
        return true;
    }
}
