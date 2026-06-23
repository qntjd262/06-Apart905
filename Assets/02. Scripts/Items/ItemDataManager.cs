using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Networking;

public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance;

    [Header("구글 시트 링크")]
    public string eatItemCsvUrl ="https://docs.google.com/spreadsheets/d/e/2PACX-1vT4U4_4nxPvI1TD9RGs0fVEZv3vmIRNjUeNpPgdod62Gr3Kt0VP61e9D_cVFaAs0Q/pub?gid=195872406&single=true&output=csv";
    public string equipItemCsvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vT4U4_4nxPvI1TD9RGs0fVEZv3vmIRNjUeNpPgdod62Gr3Kt0VP61e9D_cVFaAs0Q/pub?gid=1711219477&single=true&output=csv";
    public string useItemCsvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vT4U4_4nxPvI1TD9RGs0fVEZv3vmIRNjUeNpPgdod62Gr3Kt0VP61e9D_cVFaAs0Q/pub?gid=1830928452&single=true&output=csv";

    [Header("아이템 데이터베이스")]
    public Dictionary<string, ItemData> itemDB = new Dictionary<string, ItemData>();
    public List<ItemData> allItemList = new List<ItemData>();

    [Header("아이템 에셋 모음")]
    [SerializeField] private Sprite[] allItemIcons;
    [SerializeField] private GameObject[] allItemPrefabs;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadAllItemData());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadAllItemData()
    {
        Debug.Log("아이템 데이터 로드 시작");
        yield return StartCoroutine(DownloadAndParseCSV(eatItemCsvUrl, ItemType.Eatable));
        yield return StartCoroutine(DownloadAndParseCSV(equipItemCsvUrl, ItemType.Equipable));
        yield return StartCoroutine(DownloadAndParseCSV(useItemCsvUrl, ItemType.Useable));

        Debug.Log($"파싱 완료 총 {allItemList.Count}종의 아이템 로드 됨");

        GlobalItemCountManager.Instance.InitializeItemDeck(allItemList);
        Debug.Log("아이템 주머니 생성 완료");
    }

    private IEnumerator DownloadAndParseCSV(string url, ItemType expectedType)
    {
        UnityWebRequest ww = UnityWebRequest.Get(url);
        yield return ww.SendWebRequest();

        if(ww.result == UnityWebRequest.Result.ConnectionError || ww.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log($"CSV 다운로드 실패{expectedType} :{ww.error}");
        }
        else
        {
            ParseCSV(ww.downloadHandler.text, expectedType);
        }
    }

    private void ParseCSV(string csvData, ItemType type)
    {
        
        string[] lines = csvData.Split('\n');

        Debug.Log($"[{type}] 총 읽어들인 줄 수: {lines.Length}줄 (1줄이면 링크가 잘못된 것입니다!)");
        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] row = lines[i].Split(',');

            ItemData newItem = null;
            string currentItemID = row[0].Trim();

            if(type == ItemType.Eatable)
            {
                var eatItem = ScriptableObject.CreateInstance<EatableItemData>();
                if(Enum.TryParse(row[4].Trim(), out EatableType t1)) eatItem.eatableType_1 = t1;
                if(Enum.TryParse(row[5].Trim(), out EatableType t2)) eatItem.eatableType_2 = t2;
                if(float.TryParse(row[6].Trim(), out float v1)) eatItem.value_1 = v1;
                if(float.TryParse(row[7].Trim(), out float v2)) eatItem.value_2 = v2;

                eatItem.description = row.Length > 8 ? row[8].Trim() : "";
                newItem = eatItem;
            }
            else if(type == ItemType.Equipable)
            {
                var equipItem = ScriptableObject.CreateInstance<EquipItemData>();
                if(float.TryParse(row[4].Trim(),out float v1)) equipItem.equipValue = v1;
                equipItem.description = row.Length > 5 ? row[5].Trim() : "";

                equipItem.equipPrefab = FindPrefabByID(currentItemID);

                newItem = equipItem;
            }
            else if(type == ItemType.Useable)
            {
                var useItem = ScriptableObject.CreateInstance<UseItemData>();
                useItem.description = row.Length > 4 ? row[4].Trim() : "";
                newItem = useItem;
            }

            newItem.ID = currentItemID;
            newItem.Name = row[1].Trim();

            newItem.itemName = row[1].Trim();
            if(int.TryParse(row[2].Trim(), out int spawnCount)) newItem.SpawnCount = spawnCount;
            newItem.itemType = type;

            newItem.icon = FindIconByID(currentItemID);
            
            if (!itemDB.ContainsKey(newItem.ID))
            {
                itemDB.Add(newItem.ID, newItem);
                allItemList.Add(newItem);
            }
            else
            {
                Debug.LogWarning($"[{type}] ID 중복 발생! 제외됨: {newItem.ID}");
            }
        }
    }

    private Sprite FindIconByID(string id)
    {
        foreach(Sprite spr in allItemIcons)
        {
            if(spr != null && spr.name == id)
            {
                return spr;
            }
        }
        Debug.Log($"아이콘 매칭 실패 : {id}");
        return null;
    }

    private GameObject FindPrefabByID(string id)
    {
        foreach(GameObject prefab in allItemPrefabs)
        {
            if(prefab != null && prefab.name == id)
            {
                return prefab;
            }
        }
        Debug.Log($"프리팹 매칭 실패 : {id}");
        return null;
    }
}
