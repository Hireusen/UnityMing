using System;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 격자 좌표 변환 및 검증하는 유틸리티입니다.
/// </summary>
public class UGrid
{
    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환하여 반환합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int WorldToGrid(Vector2 pos)
    {
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        return new Vector2Int((int)pos.x, (int)pos.y);
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환하여 반환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int x, int y) WorldToGrid(float x, float y)
    {
        int nX = Mathf.FloorToInt(x);
        int nY = Mathf.FloorToInt(y);
        return (nX, nY);
    }

    /// <summary>
    /// 월드 좌표를 격자 배열에서 사용할 인덱스로 변환합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WorldToIndex(Vector2 pos, int width)
    {
        int nX = Mathf.FloorToInt(pos.x);
        int nY = Mathf.FloorToInt(pos.y);
        return GridToIndex(nX, nY, width);
    }

    /// <summary>
    /// 월드 좌표를 격자 배열에서 사용할 인덱스로 변환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WorldToIndex(float x, float y, int width)
    {
        x = Mathf.FloorToInt(x);
        y = Mathf.FloorToInt(y);
        return GridToIndex(x, y, width);
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환하여 중심점을 반환합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GridToWorld(Vector2 pos)
    {
        pos.x += 0.5f;
        pos.y += 0.5f;
        return pos;
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환하여 중심점을 반환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float x, float y) GridToWorld(float x, float y)
    {
        x += 0.5f;
        y += 0.5f;
        return (x, y);
    }

    /// <summary>
    /// 그리드 좌표 성분을 월드 좌표로 변환하여 중심점을 반환합니다.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GridToWorld(float coordinate)
    {
        return coordinate + 0.5f;
    }

    /// <summary>
    /// 좌표를 격자 배열에서 사용할 인덱스로 변환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GridToIndex(int x, int y, int width)
    {
        return (y * width) + x;
    }

    /// <summary>
    /// 좌표를 격자 배열에서 사용할 인덱스로 변환합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GridToIndex(float x, float y, int width)
    {
        int gridX, gridY;
        (gridX, gridY) = WorldToGrid(x, y);
        return (gridY * width) + gridX;
    }

    /// <summary>
    /// 좌표를 격자 배열에서 사용할 인덱스로 변환합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GridToIndex(Vector2 pos, int width)
    {
        pos = WorldToGrid(pos);
        return ((int)pos.y * width) + (int)pos.x;
    }

    /// <summary>
    /// 인덱스를 격자 배열 좌표로 변환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int x, int y) IndexToGrid(int index, int width)
    {
        int x = index % width;
        int y = index / width;
        return (x, y);
    }

    /// <summary>
    /// 인덱스를 격자 배열 좌표로 변환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 IndexToGridV(int index, int width)
    {
        int x = index % width;
        int y = index / width;
        return new Vector2(x, y);
    }

    /// <summary>
    /// 인덱스를 월드 좌표로 변환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float x, float y) IndexToWorld(int index, int width)
    {
        (int x, int y) = IndexToGrid(index, width);
        float fX = x + 0.5f;
        float fY = y + 0.5f;
        return (fX, fY);
    }

    /// <summary>
    /// 인덱스를 월드 좌표로 변환합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 IndexToWorldV(int index, int width)
    {
        (int x, int y) = IndexToGrid(index, width);
        return new Vector2(x + 0.5f, y + 0.5f);
    }

    /// <summary>
    /// 좌표가 맵 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InMap(int x, int y, int width, int height)
    {
        if (x < 0) return false;
        if (y < 0) return false;
        if (width <= x) return false;
        if (height <= y) return false;
        return true;
    }

    /// <summary>
    /// 좌표가 맵 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InMap(float x, float y, int width, int height)
    {
        if (x < 0f) return false;
        if (y < 0f) return false;
        if ((float)width <= x) return false;
        if ((float)height <= y) return false;
        return true;
    }

    /// <summary>
    /// 좌표가 맵 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InMap(int index, int width, int height)
    {
        if (index < 0) return false;
        if (width * height < index) return false;
        return true;
    }

    /// <summary>
    /// 좌표가 맵 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InMap(Vector2 pos, int width, int height)
    {
        if (pos.x < 0f) return false;
        if (pos.y < 0f) return false;
        if ((float)width <= pos.x) return false;
        if ((float)height <= pos.y) return false;
        return true;
    }

    /// <summary>
    /// 실수 좌표를 맵 범위 안으로 클램프합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float x, float y) ClampMap(float x, float y, int width, int height)
    {
        x = Mathf.Clamp(x, 0.001f, (float)width - 0.001f);
        y = Mathf.Clamp(y, 0.001f, (float)height - 0.001f);
        return (x, y);
    }

    /// <summary>
    /// 실수 좌표를 맵 범위 안으로 클램프합니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ClampMap(Vector2 pos, int width, int height)
    {
        pos.x = Mathf.Clamp(pos.x, 0.001f, (float)width - 0.001f);
        pos.y = Mathf.Clamp(pos.y, 0.001f, (float)height - 0.001f);
        return pos;
    }

    /// <summary>
    /// 정수 좌표를 맵 범위 안으로 클램프합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int x, int y) ClampMap(int x, int y, int width, int height)
    {
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        return (x, y);
    }

    /// <summary>
    /// 회전 열거형을 시계 방향으로 90도 회전하여 반환합니다.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static ERotation ClockwisERotation(ERotation rotation)
    {
        if (rotation == ERotation.Up)
            return ERotation.Right;
        if (rotation == ERotation.Right)
            return ERotation.Down;
        if (rotation == ERotation.Down)
            return ERotation.Left;
        if (rotation == ERotation.Left)
            return ERotation.Up;
        return ERotation.None;
    }

    /// <summary>
    /// 회전 열거형을 반시계 방향으로 90도 회전하여 반환합니다.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static ERotation CounterClockwisERotation(ERotation rotation)
    {
        if (rotation == ERotation.Up)
            return ERotation.Left;
        if (rotation == ERotation.Left)
            return ERotation.Down;
        if (rotation == ERotation.Down)
            return ERotation.Right;
        if (rotation == ERotation.Right)
            return ERotation.Up;
        return ERotation.None;
    }

    /// <summary>
    /// 블록의 방향을 고려하여 블록이 점유한 셀을 순환할 수 있는 변수를 반환받습니다.
    /// 기본값 : 왼쪽 아래
    /// </summary>
    /// <param name="index"></param>
    /// <param name="size"></param>
    /// <param name="dir"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int startX, int endX, int startY, int endY)
        GetLoopBlock(int index, Vector2 size, ERotation rotate, int width, int height)
    {
        (int x, int y) = IndexToGrid(index, width);
        int sizeX = (int)size.x;
        int sizeY = (int)size.y;
        int halfX = sizeX / 2;
        int halfY = sizeY / 2;

        int startX = x - halfX;
        int endX = x + halfX;
        int startY = y - halfY;
        int endY = y + halfY;

        // 가로 길이 짝수
        if ((sizeX & 1) == 0) {
            if (rotate == ERotation.Right) startX++;
            else endX--;
        }
        // 세로 길이 짝수
        if ((sizeY & 1) == 0) {
            if (rotate == ERotation.Up) startY++;
            else endY--;
        }
        // 인덱스 벗어나지 않도록
        (startX, startY) = ClampMap(startX, startY, width, height);
        (endX, endY) = ClampMap(endX, endY, width, height);
        // 완료
        return (startX, endX, startY, endY);
    }

    /// <summary>
    /// 유닛이 점유한 셀을 순환할 수 있는 변수를 반환받습니다.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int startX, int endX, int startY, int endY)
        GetLoopUnit(Vector2 pos, Vector2 size, int width, int height)
    {
        float halfX = size.x * 0.5f;
        float halfY = size.y * 0.5f;
        int startX = (int)(pos.x - halfX);
        int endX = (int)(pos.x + halfX);
        int startY = (int)(pos.y - halfY);
        int endY = (int)(pos.y + halfY);
        (startX, startY) = ClampMap(startX, startY, width, height);
        (endX, endY) = ClampMap(endX, endY, width, height);
        // 완료
        return (startX, endX, startY, endY);
    }

    /// <summary>
    /// 좌표가 특정 그리드 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InGrid(float x, float y, int gridX, int gridY)
    {
        float left = (float)gridX;
        float right = (float)gridX + 1f;
        float down = (float)gridY;
        float up = (float)gridY + 1f;
        if (x < left) return false;
        if (y < down) return false;
        if (right <= x) return false;
        if (up <= y) return false;
        return true;
    }

    /// <summary>
    /// 좌표가 특정 그리드 안에 있는지 검사합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InGrid(Vector2 pos, int gridX, int gridY)
    {
        float left = (float)gridX;
        float right = (float)gridX + 1f;
        float down = (float)gridY;
        float up = (float)gridY + 1f;
        if (pos.x < left) return false;
        if (pos.y < down) return false;
        if (right <= pos.x) return false;
        if (up <= pos.y) return false;
        return true;
    }

    /// <summary>
    /// 정확한 값의 방향 벡터를 열거형으로 변환합니다.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static ERotation DirToRotate(Vector2 dir)
    {
        if (dir == Vector2.up)
            return ERotation.Up;
        else if (dir == Vector2.right)
            return ERotation.Right;
        else if (dir == Vector2.down)
            return ERotation.Down;
        else if (dir == Vector2.left)
            return ERotation.Left;
        De.Print("존재하지 않는 방향이 감지되었습니다. GetDirection4가 제대로 일 안하는 듯", LogType.Assert);
        return ERotation.None;
    }

    /// <summary>
    /// 4방향 인접 셀을 순회하며 액션(x, y)을 실행합니다.
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForeachAdjacentCells4(int gridX, int gridY, int width, int height, Action<int, int> action)
    {
        if (1 <= gridX) action(gridX - 1, gridY);
        if (1 <= gridY) action(gridX, gridY - 1);
        if (gridX < width - 1) action(gridX + 1, gridY);
        if (gridY < height - 1) action(gridX, gridY + 1);
    }

    /// <summary>
    /// 8방향 인접 셀을 순회하며 액션(x, y)을 실행합니다.
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForeachAdjacentCells8(int gridX, int gridY, int width, int height, Action<int, int> action)
    {
        if (1 <= gridX) {
            int leftX = gridX - 1;
            action(leftX, gridY); // 왼쪽
            if (1 <= gridY) action(leftX, gridY - 1); // 왼쪽 위
            if (gridY < height - 1) action(leftX, gridY + 1); // 왼쪽 아래
        }
        if (gridX < width - 1) {
            int rightX = gridX + 1;
            action(rightX, gridY); // 오른쪽
            if (1 <= gridY) action(rightX, gridY - 1); // 오른쪽 위
            if (gridY < height - 1) action(rightX, gridY + 1); // 오른쪽 아래
        }
        // 위, 아래
        if (1 <= gridY) action(gridX, gridY - 1);
        if (gridY < height - 1) action(gridX, gridY + 1);
    }

    /// <summary>
    /// 두 좌표를 받아서 점유한 블록을 점유할 수 있도록 변수를 반환합니다.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int minX, int minY, int maxX, int maxY) GetForeachPos(Vector2 startPos, Vector2 endPos)
    {
        Vector2 startGrid = WorldToGrid(startPos);
        Vector2 endGrid = WorldToGrid(endPos);
        (startGrid, endGrid) = UMath.SortNumericSize(startGrid, endGrid);
        int minX = (int)startGrid.x;
        int minY = (int)startGrid.y;
        int maxX = (int)endGrid.x;
        int maxY = (int)endGrid.y;
        return (minX, minY, maxX, maxY);
    }

    /// <summary>
    /// ERotation을 각도(도)로 변환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RotationToAngle(ERotation rotation)
    {
        switch (rotation) {
            case ERotation.Up: return 0f;
            case ERotation.Right: return -90f;
            case ERotation.Down: return -180f;
            case ERotation.Left: return -270f;
            default: return 0f;
        }
    }

    /// <summary>
    /// ERotation을 시계 방향으로 90도 회전합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ERotation RotateCW(ERotation rotation)
    {
        switch (rotation) {
            case ERotation.Up: return ERotation.Right;
            case ERotation.Right: return ERotation.Down;
            case ERotation.Down: return ERotation.Left;
            case ERotation.Left: return ERotation.Up;
            default: return ERotation.Up;
        }
    }

    /// <summary>
    /// ERotation을 반시계 방향으로 90도 회전합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ERotation RotateCCW(ERotation rotation)
    {
        switch (rotation) {
            case ERotation.Up: return ERotation.Left;
            case ERotation.Left: return ERotation.Down;
            case ERotation.Down: return ERotation.Right;
            case ERotation.Right: return ERotation.Up;
            default: return ERotation.Up;
        }
    }
}
