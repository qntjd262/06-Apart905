    using UnityEngine;

public class MoveLevel : MonoBehaviour
{
    /*    ���� �̵��� �� �۵��ϴ� ��ũ��Ʈ
     *    �� �� ���̳��� ���� ������� ��/�Ʒ��� �̵��Ǵ� ���
     *    �̵��� �� �ش� ���� �ִ� ������Ʈ/����/NPC ���� ��Ȱ��ȭ�ǰ� �� �ڸ��� ��������
     *    ��� �߾ӿ� ���� ��ũ��Ʈ�� �۵��ϴ� ���
     *    �� ������ prefab�������� 5.4
     *    �ٴ� y���� -45.9
     */

    [SerializeField] private FloorManager _floorManager;
    [SerializeField] private DoorManager _doorManager;

    public enum PlayerState         // �÷��̾��� �� �̵� ���� 
    {
        None, MoveUp, MoveDown
    }
    public PlayerState state;
    public bool check = false;      // �÷��̾ ��� �߾ӿ� �ִ��� üũ�ϴ� bool


    [SerializeField]
    private GameObject
        _LevelBott, _LevelMid, _LevelTop,           // �̵���ų 3���� ����
        _RoofTop, _Ground;                          // ����, 1�� �ٴ�
    [SerializeField] private int _currLevel = 9;             // ���� �� ��, ������ �� 9������ ����

    // set�Ұ� get���� ������Ƽ, Ÿ Ŭ�������� �ҷ����� ��
    public int CurrLevel => _currLevel;
    public GameObject LevelBottom { get{ return _LevelBott; }}
    public GameObject LevelMid { get{ return _LevelMid; }}
    public GameObject LevelTop { get{ return _LevelTop; }}

    public void MoveUp()
    {
        if (_currLevel >= 2 && _currLevel < 16)          // 18���� ���� ����
        {          
            _LevelBott.transform.Translate(0, 5.4f * 3, 0);   // �������� ���� ����
            IsMoveUp(true);

            if (_currLevel == 3)
                _Ground.SetActive(false);

            _floorManager.SetActiveFurnitures(_currLevel, true, false);                                 // ���� ���� ���� Ȱ������ ����
        }        
        else if(_currLevel != 1)
        {
            if (!_RoofTop.gameObject.activeSelf)
            {
                _RoofTop.transform.position = new Vector3(0, _LevelTop.transform.position.y + 5.4f, 0);    // ���� ��ġ ����, ���� Ȱ��ȭ�ϰ� ���� 3���� Floor���� ��ġ��ȭX
                _RoofTop.SetActive(true);
            }
        }

        _currLevel++;
        _floorManager.SetQuests(_currLevel);
        _floorManager.SetZombies(_currLevel);
        OnFloorShifted();
    }

    public void MoveDown()
    {
        if (_currLevel > 2 && _currLevel < 17)           // �ֻ����� ���� �Ʒ���
        {
            _LevelTop.transform.Translate(0, -5.4f * 3, 0);
            IsMoveUp(false);

            if (_currLevel == 16)                       // ���� ��Ȱ��ȭ
                _RoofTop.SetActive(false);

            _floorManager.SetActiveFurnitures(_currLevel, false, false);                                // ���� ���� ���� Ȱ������ ����
        }
        else if( _currLevel == 2)
        {
            _Ground.SetActive(true);
        }

        _currLevel--;                               // �� ����
        _floorManager.SetQuests(_currLevel);
        _floorManager.SetZombies(_currLevel);
        OnFloorShifted();
    }

    private void IsMoveUp(bool isMoveUp)     // �� �̵� �� �� ��ġ ����
    {
        if (isMoveUp)           // 1-2-3 -> 2-3-1
        {
            GameObject LvTop = _LevelBott;   // ���� �ֻ����� levelbottom
            GameObject LvMiddle = _LevelTop; // ���� �߰����� leveltop

            _LevelBott = _LevelMid;         // �������� 2��
            _LevelMid = LvMiddle;           // �߰����� 3����
            _LevelTop = LvTop;              // �ֻ����� 1��
        }
        else                    // 1-2-3 -> 3-1-2
        {
            GameObject LvBottom = _LevelTop;   // ���� �������� leveltop
            GameObject LvMiddle = _LevelBott; // ���� �߰����� levelBottom

            _LevelTop = _LevelMid;         // �ֻ����� 2��
            _LevelMid = LvMiddle;          // �߰����� 1��
            _LevelBott = LvBottom;         // �������� 3����            
        }
    }

    public void OnFloorShifted()
    {
        Door[] currDoors = _LevelMid.GetComponentsInChildren<Door>();

        foreach (Door door in currDoors)
        {
            door.realFloor = _currLevel;

            DoorInfo info = new DoorInfo
            {
                floor = _currLevel,
                isLeft = door.isLeft,
                doorNum = door.doorNum
            };

            bool checkOpen = _doorManager.IsDoorOpen(info);
            Debug.Log($"{checkOpen}, {door.isLeft}, {door.doorNum}");
            door.SetStateImmediate(checkOpen);
        }
    }

    public void MonsterFloorMove(MonsterController targetMonster, bool isUp)
    {
        _floorManager.MonsterFloorMove(targetMonster, isUp);
    }

    private void GetChangedCurrLv()
    {

    }
}
