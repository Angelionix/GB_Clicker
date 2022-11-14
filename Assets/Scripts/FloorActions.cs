using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(FloorData))]
public class FloorActions : MonoBehaviour
{
    [SerializeField] private FloorData _floorData;                      //������ �� ������ �����

    private float _timerForEntitySpawn;                                 // ������ ������ ��������

    public delegate void OnChangeSpawnTimerValue(float value);          // ������� ��������� �������
    public static OnChangeSpawnTimerValue onChangeSpawnTimerValue;

    public delegate void OnSpawnToNewFloor(int floorIndex, int count);  //������� ������ �� ����� ����
    public static OnSpawnToNewFloor onSpawnToNewFloor;

    public delegate void OnEntitySpawn(float value);              // ������� ��� ������� �������
    public static OnEntitySpawn onEntitySpawn;

    void Awake()
    {
        _timerForEntitySpawn = _floorData.PeriodEntitySpawn;
    }

    private void OnEnable()
    {
        EntityActions.onSlotResease += SlotListsManaging;
        EntityActions.onEntityEvolution += EntityUpgrading;
        WorldManager.onChangeFloor += SpawnigStoredEnteties;
        _floorData.TimeOfEnterInFloor = System.DateTime.Now;
        SpawningEntityFromFile(_floorData);
     
    }

    private void OnDisable()
    {
        EntityActions.onSlotResease -= SlotListsManaging;
        EntityActions.onEntityEvolution -= EntityUpgrading;
        WorldManager.onChangeFloor += SpawnigStoredEnteties;       
        _floorData.TimeOfExit = System.DateTime.Now;
    }
    private void Start()
    {
        AfkSpawn();
    }

    private void Update()
    {
        EntitySpawnCountdowning();
    }
    private void EntitySpawnCountdowning()
    {
        if (_timerForEntitySpawn > 0)
        {
            _timerForEntitySpawn -= Time.deltaTime;
        }
        else
        {
            _timerForEntitySpawn = _floorData.PeriodEntitySpawn;
            SpawningEntity(false);
        }
        onChangeSpawnTimerValue(_timerForEntitySpawn);
    }
    /// <summary>
    /// ������� ����� ������ �������
    /// </summary>
    /// <param name="entity">������ ������� ����� ����������</param>
    /// <param name="spawnSlot">���� � ������� �� �������</param>
    /// <param name="fd">������ �� ���� ����</param>
    /// <param name="spawnType"> ��� ������, ������ ������� ������ ��� </param>
    public void SpawnEntity(EntityData entity, Slot spawnSlot, FloorData fd, int spawnType)
    {
        EntityData newEntity = Instantiate(entity.gameObject,
                                spawnSlot.transform.position,
                                Quaternion.identity).GetComponent<EntityData>();
        newEntity.FloorData = fd;
        if (spawnType == 1)
        {
            newEntity.CurrentSlot = spawnSlot;
            SlotListsManaging(newEntity.CurrentSlot, 1);
            SlotListsManaging(newEntity.CurrentSlot, 4);
            newEntity.transform.SetParent(newEntity.CurrentSlot.transform);
        }
        else
        {
            newEntity.CurrentSlot = entity.CurrentSlot;
            newEntity.transform.SetParent(newEntity.CurrentSlot.transform);
        }
        newEntity.CurrentSlot.EntityData = newEntity;
    }
    /// <summary>
    /// ����� ������ ������ ������� �� �����
    /// </summary>
    /// <param name="isStoredSpawn"></param>
    private void SpawningEntity(bool isStoredSpawn)
    {
        if (_floorData.EmptySlots.Count > 0)
        {
            int indexOfSlot = GetRandomIndex(_floorData.EmptySlots.Count);
            int indexOfEntity = 0; //GetRandomIndex(_entitySet.Length);
            SpawnEntity(_floorData.EntitySet[indexOfEntity], _floorData.EmptySlots[indexOfSlot], _floorData, 1);
            if (!isStoredSpawn)
            {
                onEntitySpawn(_floorData.EntitySet[indexOfEntity].ManyPerSecond);
            }
        }
    }
    /// <summary>
    /// ����� ����� �������� �� �����������
    /// </summary>
    /// <param name="fd"></param>
    private void SpawningEntityFromFile(FloorData fd)
    {
            for (int i = 0; i < _floorData.EntitiesToSpawnFromFile.Length; i++)
            {
                if (_floorData.EntitiesToSpawnFromFile[i] > -1)
                {
                    int indexOfSlot = i;
                    int indexOfEntity = _floorData.EntitiesToSpawnFromFile[i];
                    SpawnEntity(_floorData.EntitySet[indexOfEntity], _floorData.Slots[indexOfSlot], _floorData, 1);
                }// ���� ���� ���� ��� ������ ������ ��� ������� ������ � ����� ������ ��� ����� ����� �����, ������ ��� �� ������ �������
            }
    }
    /// <summary>
    /// ����� ������ �������� �� ������
    /// </summary>
    /// <param name="buffer"></param>
    public void SpawnigStoredEnteties(int[,] buffer)
    {
        if (buffer[_floorData.IndexFloor, 1] > 0)
        {
            for (int i = 0; i < buffer[_floorData.IndexFloor, 1]; i++)
            {
                SpawningEntity(true);
            }
            buffer[_floorData.IndexFloor, 1] = 0;
        }
    }

    /// <summary>
    /// ��������������� � ������ ����������� ������ ������
    /// </summary>
    /// <param name="entity">��� ������, ������� ������ ����� </param>
    /// <param name="floorIndex"> ����� ����� �� ������� ��������� ������� </param>
    private void EntityUpgrading(EntityData entity)
    {
        if (entity.FloorData.IndexFloor == _floorData.IndexFloor)
        {
            /// ���� ������� ������� ������ ������������� �� ����� 
            /// �� �� ������� ����� ������ ������� ������� ���� �� 1 ��� � 
            /// ���������� �������, ����� � ��� ������� ��������, 
            /// ������� ���� ���������� �������, ����� ���������������� ������
            /// � ����� �������� ����� ��� ������ ������� �� �����
            if (entity.LevelEntity < entity.FloorData.EntitySet.Length - 1)
            {
                EntityData newEntity = entity.FloorData.EntitySet[entity.LevelEntity + 1];
                newEntity.CurrentSlot = entity.CurrentSlot;
                Destroy(entity.gameObject);
                SpawnEntity(newEntity, newEntity.CurrentSlot, entity.FloorData, 2);             
            }
            else
            {
                /// ���� ������� ���������� ������� ������������ �� �����
                /// �� ���������� ��������� ������ �
                /// �������� ������� � ������� �������� ������ ���������� �����
                onSpawnToNewFloor(_floorData.IndexFloor + 1,1);
                onEntitySpawn(_floorData.EntitySet[_floorData.EntitySet.Length - 1].ManyPerSecond * 2);
                SlotListsManaging(entity.CurrentSlot,3);
                SlotListsManaging(entity.CurrentSlot, 2);
                GameObject.Destroy(entity.gameObject);            
            }
        }
    }
    /// <summary>
    /// ����� ��� ��������� ���������� ����� �� ���������
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    private int GetRandomIndex(int max)
    {
        if (max > 0)
        {
            return Random.Range(0, max);
        }
        return -1;
    }
    /// <summary>
    /// ����� ��� ������ �� �������� ������ � ����������� ������
    /// </summary>
    /// <param name="slot">���� ������� ���� ����������</param>
    /// <param name="comand">������� ��������� � ����� ������� � ��� ����� �������</param>
    public void SlotListsManaging(Slot slot, int comand)
    {
        switch (comand)
        {
            case 1:
                _floorData.EmptySlots.Remove(slot);
                break;
            case 2:
                _floorData.EmptySlots.Add(slot);
                break;             
            case 3:
                _floorData.FullSlots.Remove(slot);
                break;
            case 4:
                _floorData.FullSlots.Add(slot);
                break;
        }
    }
    /// <summary>
    /// ����� ��� ������ �������� ����� ����������
    /// </summary>
    public void AfkSpawn()
    {
        double afkTime = Utility.CalculationOfAFKTime(_floorData.TimeOfExit, _floorData.TimeOfEnterInFloor);
        int entityToSpawnValue = (int)(afkTime / _floorData.PeriodEntitySpawn);
        for (int i = 0; i < entityToSpawnValue; i++)
        {
            SpawningEntity(false);
        }
    }



}
