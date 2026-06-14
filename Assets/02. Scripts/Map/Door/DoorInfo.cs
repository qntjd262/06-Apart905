using System;

public struct DoorInfo : IEquatable<DoorInfo>
{
    /* 문의 정보를 담는 구조체
     */

    public int floor;           // 층
    public bool isLeft;         // 왼쪽 문인지
    public int doorNum;         // 몇 번째 문
    public bool isOpen;         // 열려있는지

    public bool Equals(DoorInfo other)
        => floor == other.floor && isLeft == other.isLeft && doorNum == other.doorNum;

    public override int GetHashCode()
        => HashCode.Combine(floor, isLeft, doorNum);
}
