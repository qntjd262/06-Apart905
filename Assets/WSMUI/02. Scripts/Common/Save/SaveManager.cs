using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private string GetSavePath(int slotIndex)
    {
        // Application.persistentDataPath는 게임이 설치된 기기의 안전한 저장 폴더 경로입니다.
        return Path.Combine(Application.persistentDataPath, $"SaveData_Slot_{slotIndex}.json");
    }

    public void SaveGame(int slotIndex)
    {
        SaveData data = new SaveData();

        // 1. 각 시스템에서 현재 데이터를 긁어와서 SaveData에 담기
        PlayerStat player = FindFirstObjectByType<PlayerStat>();
        if (player != null)
        {
            data.playerPosX = player.transform.position.x;
            data.playerPosY = player.transform.position.y;
            data.playerPosZ = player.transform.position.z;

            data.currentHp = player.hp.currentValue;
            data.currentHunger = player.hunger.currentValue;
            data.currentThirst = player.thirst.currentValue;
            data.currentInfection = player.infection.currentValue;
        }

        // TODO: InventoryManager.Instance.BagSlots를 돌면서 아이템 ID를 data.bagItemIDs에 담기
        // TODO: QuestManager.Instance.activeQuests를 돌면서 퀘스트 ID를 data.activeQuestIDs에 담기

        // 2. 클래스 데이터를 JSON 형태의 텍스트(문자열)로 변환
        string json = JsonUtility.ToJson(data, true);

        // 3. 실제 컴퓨터 파일로 저장
        string path = GetSavePath(slotIndex);
        File.WriteAllText(path, json);

        Debug.Log($"슬롯 {slotIndex}에 게임 데이터가 JSON으로 저장되었습니다! 경로: {path}");
    }

    public void LoadGame(int slotIndex)
    {
        string path = GetSavePath(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning("저장된 세이브 파일이 없습니다.");
            return;
        }

        // 1. JSON 텍스트 파일을 읽어오기
        string json = File.ReadAllText(path);

        // 2. 텍스트를 다시 SaveData 클래스로 변환
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 3. 변환된 데이터를 현재 게임 시스템에 덮어씌우기
        PlayerStat player = FindFirstObjectByType<PlayerStat>();
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 2. 위치 적용
            player.transform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

            // 3. 다시 켜기
            if (cc != null) cc.enabled = true;

            player.hp.currentValue = data.currentHp;
            player.hunger.currentValue = data.currentHunger;
            player.thirst.currentValue = data.currentThirst;
            player.infection.currentValue = data.currentInfection;
        }

        // TODO: 기존 인벤토리를 비우고 data.bagItemIDs를 기반으로 ItemDataManager에서 아이템을 찾아 다시 넣어주기
        // TODO: 퀘스트 상태 덮어씌우기
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    protected override void OnSceneUnloaded(Scene scene) { }
}