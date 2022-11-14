using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(FloorData))]
public class FloorActions : MonoBehaviour
{
    [SerializeField] private FloorData _floorData;                      //ссылка на данные этажа

    private float _timerForEntitySpawn;                                 // таймер спавна обьектов

    public delegate void OnChangeSpawnTimerValue(float value);          // событие изменения таймера
    public static OnChangeSpawnTimerValue onChangeSpawnTimerValue;

    public delegate void OnSpawnToNewFloor(int floorIndex, int count);  //событие спавна на новый этаж
    public static OnSpawnToNewFloor onSpawnToNewFloor;

    public delegate void OnEntitySpawn(float value);              // Событие при создани обьекта
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
    /// Базовый метод спавна обьекта
    /// </summary>
    /// <param name="entity">Обьект который нужно заспавнить</param>
    /// <param name="spawnSlot">Слот в который мы спавним</param>
    /// <param name="fd">Ссылка на Флор дату</param>
    /// <param name="spawnType"> тип спавна, просто спавним объект или </param>
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
    /// Метод спавна нового обьекта на этаже
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
    /// Метод спвна обьектов из созхраненки
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
                }// если слот брат ьиз емпати слотов тов какойто момент И будет больше чем длина эмпти слотс, потому что мы оттуда удаляем
            }
    }
    /// <summary>
    /// Метод спавна обьектов из буфера
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
    /// Подготоваливает к спавну прокаченную Версию Ентити
    /// </summary>
    /// <param name="entity">тот обьект, который вызвал метод </param>
    /// <param name="floorIndex"> индех этажа на котором произошло событие </param>
    private void EntityUpgrading(EntityData entity)
    {
        if (entity.FloorData.IndexFloor == _floorData.IndexFloor)
        {
            /// Если уровень обьекта меньше максимального на этаже 
            /// то мы создаем новую энтити уровень которой выше на 1 чем у 
            /// вызвавшего событие, далее в его текущий помещаем, 
            /// текущий слот вызвавшего обьетка, затем удаляемвызвавший обьект
            /// и далее вызываем метод для спавна обьекта на этаже
            if (entity.LevelEntity < entity.FloorData.EntitySet.Length - 1)
            {
                EntityData newEntity = entity.FloorData.EntitySet[entity.LevelEntity + 1];
                newEntity.CurrentSlot = entity.CurrentSlot;
                Destroy(entity.gameObject);
                SpawnEntity(newEntity, newEntity.CurrentSlot, entity.FloorData, 2);             
            }
            else
            {
                /// Если уровень вызвавшего обьекта максимальный на этаже
                /// Мы уничтожаем вызвавший обьект и
                /// вызываем событие в котором передаем индекс следующего этажа
                onSpawnToNewFloor(_floorData.IndexFloor + 1,1);
                onEntitySpawn(_floorData.EntitySet[_floorData.EntitySet.Length - 1].ManyPerSecond * 2);
                SlotListsManaging(entity.CurrentSlot,3);
                SlotListsManaging(entity.CurrentSlot, 2);
                GameObject.Destroy(entity.gameObject);            
            }
        }
    }
    /// <summary>
    /// метод для получения рандомного числа из интервала
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
    /// Метод для работы со списками пустых и заполненных слотов
    /// </summary>
    /// <param name="slot">слот который надо обработать</param>
    /// <param name="comand">Команда указывает с каким списком и что нужно сделать</param>
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
    /// Метод для спавна обьектов после акашивания
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
