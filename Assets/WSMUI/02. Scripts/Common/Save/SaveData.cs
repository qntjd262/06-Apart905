using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]을 반드시 붙여야 JSON으로 변환할 수 있습니다.
[System.Serializable]
public class SaveData
{
    [Header("플레이어 위치")]
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    [Header("생존 스탯")]
    public float currentHp;
    public float currentHunger;
    public float currentThirst;
    public float currentInfection;

    [Header("인벤토리 & 퀘스트")]
    // 아이템 객체 전체가 아니라 '아이템 ID(문자열)'와 수량만 리스트로 저장해야 가볍고 안전합니다.
    public List<string> bagItemIDs = new List<string>(); 
    public List<string> activeQuestIDs = new List<string>();
}