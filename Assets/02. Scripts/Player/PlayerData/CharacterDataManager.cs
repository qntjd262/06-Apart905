using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance;

    [Header("구글 csv 링크")]
    public string csvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRutBw_yG6vGn-BNBktruf-_QfgAIn89INa2YE1oZQWN9X7mWfp9LO7k9lZHt-Cng/pub?gid=2074671912&single=true&output=csv";

    public Dictionary<string, CharacterStatSO> characterDB = new Dictionary<string, CharacterStatSO>();
    
    public CharacterStatSO selectedCharacterSO;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(LoadDataFromGoogleDrive());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadDataFromGoogleDrive()
    {
        UnityWebRequest ww = UnityWebRequest.Get(csvUrl);
        yield return ww.SendWebRequest();

        if(ww.result == UnityWebRequest.Result.ConnectionError || ww.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("데이터 다운로드 실패"+ww.error);
        }
        else
        {
            Debug.Log("구글 드라이브 데이터 다운 성공");
            ParseCSV(ww.downloadHandler.text);
            
        }
    }

    private void ParseCSV(string csvData)
    {
        string[] lines = csvData.Split('\n');

        for(int i = 1 ; i <lines.Length ; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = lines[i].Split(',');

            CharacterStatSO newSO = ScriptableObject.CreateInstance<CharacterStatSO>();

            newSO.ID = row[0].Trim();
            newSO.Name = row[1].Trim();
            newSO.Hp = float.Parse(row[2].Trim());
            newSO.Def = float.Parse(row[3].Trim());
            newSO.AttackPower = float.Parse(row[4].Trim());
            newSO.MaxStamina = float.Parse(row[5].Trim());
            newSO.InfectionIncreaseRate = float.Parse(row[6].Trim());
            newSO.ThirstDecreaseRate = float.Parse(row[7].Trim());
            newSO.HungerDecreaseRate = float.Parse(row[8].Trim());

            if (!characterDB.ContainsKey(newSO.Name))
            {
                characterDB.Add(newSO.Name, newSO);
            }
        }
        Debug.Log("캐릭터 테이터 파싱 완료");

    }
}
