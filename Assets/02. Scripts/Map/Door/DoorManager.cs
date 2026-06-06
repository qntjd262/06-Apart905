using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    /* 문의 상호작용과 각 층에서 열려있는 상태를 관리
     * 
     * doorNum은 0은 현관, 1은 Room1, 2는 Room2, 3은 Room3, 4는 Bathroom
     */
    
    private HashSet<DoorInfo> _openedDoors = new HashSet<DoorInfo>();   // 열린 문만 저장하는 HashSet

    public void SetDoorOpen(DoorInfo info, bool isOpen)                 // 문 상태 업데이트
    {
        if (isOpen) _openedDoors.Add(info);
        else _openedDoors.Remove(info);
    }

    public bool IsDoorOpen(DoorInfo info) => _openedDoors.Contains(info);       // 문이 열려있는지 확인
}
