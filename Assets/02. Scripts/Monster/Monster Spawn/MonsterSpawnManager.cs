using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 몬스터 종류 enum
public enum MonsterType
{
    BigZombie, Giant_Zombie, OldManZombie, UrbanZombie, ZombieGirl_B
}

public class MonsterSpawnManager : Singleton<MonsterSpawnManager>
{
    // 몬스터를 저장할 Dictionary
    private Dictionary<MonsterType, Stack<GameObject>> monsterPool = new Dictionary<MonsterType, Stack<GameObject>>();


    [SerializeField] private int initPoolSize;
    [SerializeField] private GameObject[] _monsters;
    public GameObject[] Monsters => _monsters;


    protected override void Awake()
    {
        base.Awake();
        InitPool();
    }

    // Object Pool 초기화
    private void InitPool()
    {
        GameObject instance = null;

        for (int i = 0; i < Monsters.Length; i++)
        {
            MonsterType monsterType = (MonsterType)System.Enum.Parse(typeof(MonsterType), Monsters[i].name);
            Stack<GameObject>  monsterStack = new Stack<GameObject>();

            for (int j = 0; j < initPoolSize; j++)
            {
                instance = Instantiate(Monsters[i], transform);
                instance.name = Monsters[i].name;
                instance.gameObject.SetActive(false);
                monsterStack.Push(instance);
            }
            monsterPool.Add(monsterType, monsterStack);
        }
    }

    // 몬스터를 Pool에서 찾거나 없으면 생성해서 반환
    public GameObject GetPoolObject(MonsterType monsterType)
    {
        // 해당 몬스터 타입이 아예 등록되어있지 않다면
        if (!monsterPool.ContainsKey(monsterType))
        {
            Debug.Log($"몬스터 오브젝트 풀에 {monsterType}이 존재하지 않음");
            return null;
        }

        GameObject monster;

        // Object Pool가 비어있다면
        if (monsterPool[monsterType].Count == 0)
        {
            GameObject newMonster = System.Array.Find(Monsters, monster => monster.name == monsterType.ToString());
            monster = Instantiate(newMonster, transform);
            monster.name = monsterType.ToString();
        }
        // Object Pool에 있다면
        else
        {
            monster = monsterPool[monsterType].Pop();
        }

        monster.SetActive(true);
        return monster;
    }

    // 소환된 몬스터를 Pool에 다시 넣음
    public void ReturnPoolObject(GameObject monster)
    {
        monster.SetActive(false);
        MonsterType monsterType = (MonsterType)System.Enum.Parse(typeof(MonsterType), monster.name);
        monsterPool[monsterType].Push(monster);
    }

    // 층 이동으로 인한 일시적 비활성화
    public void TemporarilyDeactive()
    {

    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}


