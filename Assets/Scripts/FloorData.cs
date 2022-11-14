using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс содержит в себе только данные этажа
/// </summary>
public class FloorData : MonoBehaviour
{
    [SerializeField] private bool _isOpened;                                //Открыт ли этаж
    [SerializeField] private int _indexFloor;                               //Индекс этажа
    [SerializeField] private GameObject _slotsContainer;                    //Контейнер в котором мы храним слоты
    [SerializeField] private Slot[] _slots;                                 //Набо слотов на этаже
    [SerializeField] private List<Slot> _emptySlots = new List<Slot>();     //Список пустых слотов
    [SerializeField] private List<Slot> _fullSlots = new List<Slot>();      //Список занятых слотов
    [SerializeField] private EntityData[] _entitySet;                       //Набор обьектов для текущего "этажа"
    [SerializeField] private int[] _entitiesToSpawnFromFile;                //Обьекты для спавна полученные из файла
    [SerializeField] private float _periodMoneySpawnBase;                   //Интервал получения бабла базовый без плюшек
    [SerializeField] private float _periodMoneySpawn;                       //Инетрва лполучени бабла с плюшками
    [SerializeField] private float _periodEntitySpawn;                      //Интервал спавна обьектов
    [SerializeField] private System.DateTime _timeOfDeactivatingFloor;      // Время и дата когда этаж был отключен
    [SerializeField] private System.DateTime _timeOfActivatingFloor;        //Время и дата когда этаж был включен
    
    public float PeriodEntitySpawn
    {
        get
        {
            return _periodEntitySpawn;
        }
        set
        {
            _periodEntitySpawn = value;
        }

    }
    public bool IsOpened
    {
        get
        {
            return _isOpened;
        }
        set
        {
            _isOpened = value;
        }
    }
    public Slot[] Slots
    {
        get
        {
            return _slots;
        }
        set
        {
            _slots = value;
        }
    }
    public int IndexFloor
    {
        get
        {
            return _indexFloor;
        }
        set
        {
            _indexFloor = value;
        }
    }
    public List<Slot> EmptySlots
    {
        get
        {
            return _emptySlots;
        }
        set
        {
            _emptySlots = value;
        }
    }
    public List<Slot> FullSlots
    {
        get
        {
            return _fullSlots;
        }
        set
        {
            _fullSlots = value;
        }
    }
    public System.DateTime TimeOfExit
    {
        get
        {
            return _timeOfDeactivatingFloor;
        }
        set
        {
            _timeOfDeactivatingFloor = value;
        }
    }
    public System.DateTime TimeOfEnterInFloor
    {
        get
        {
            return _timeOfActivatingFloor;
        }
        set
        {
            _timeOfActivatingFloor = value;
        }
    }
    public EntityData[] EntitySet
    {
        get
        {
            return _entitySet;
        }
    }
    public int[] EntitiesToSpawnFromFile
    {
        get
        {
            return _entitiesToSpawnFromFile;
        }
        set
        {
            _entitiesToSpawnFromFile = value;
        }
    }
    /// <summary>
    /// В эвейке мы инициализируем те данные которые нужны для работы этажа
    /// </summary>
    private void Awake()
    {
        _entitiesToSpawnFromFile = new int[35];
        _slots = _slotsContainer.GetComponentsInChildren<Slot>();
        for (int i = 0; i < _entitiesToSpawnFromFile.Length; i++)
        {
            _emptySlots.Add(_slots[i]);
            _entitiesToSpawnFromFile[i] = -1;
        }

        _periodEntitySpawn = PlaySettings._EntitySpawnDelay[_indexFloor];
    }
}
