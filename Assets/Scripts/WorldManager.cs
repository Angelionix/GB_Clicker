using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Класс отвечает за работу текущего мира
/// </summary>
public class WorldManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private int _id;                                               //На случай если будет много сцен-миров
    [SerializeField] private Camera _mainCamera;                                    //Ссылка на главную камеру
    [SerializeField] private FloorData[] _floors;                                   //Набор этажей в текущем мире
    [SerializeField] private float _periodMoneySpawnBase;                           //Кд спавна денюшек
    [SerializeField] private float _periodMoneySpawn;                               
    [SerializeField] private float _timerForMoneySpawn;                             //глобальный таймер спавна денюшек
    [SerializeField] private int _currentFloor;                                     //Индекс активного этажа
    [SerializeField] private float _worldPassiveIncomingBase;                       //Пассивынй доход мира, без плюшек
    [SerializeField] private float _worldPassiveIncoming;                           //пассивный доход уже с плюшками
    [SerializeField] private int[,] _entityToSpawnNextField = new int[7, 2];        // Буфер, в котором храним сколько обьектов нужно
                                                                                    // создать при переходе на след уровень
                                                                                    // 1 колонка - этаж, 2 - сколько надо заспавнить
    #endregion          
    #region Properties
    public int ID
    {
        get
        {
            return _id;
        }
    }
    public FloorData[] Floors
    {
        get
        {
            return _floors;
        }
        set
        {
            _floors = value;
        }
    }
    public float ClickDamageMult
    {
        get
        {
            return PlaySettings.clickCritRate;
        }
        set
        {
            PlaySettings.clickCritRate = value;
        }
    }
    public int PassiveMult
    {
        get
        {
            return PlaySettings.passiveIncomingMulti;
        }
        set
        {
            PlaySettings.passiveIncomingMulti = value;
        }
    }
    public float CritChance
    {
        get
        {
            return PlaySettings.clickCritRate;
        }
        set
        {
            PlaySettings.clickCritRate = value;
        }
    }
    public float WorldPassiveIncoming
    {
        get
        {
            return _worldPassiveIncomingBase;
        }
        set
        {
            _worldPassiveIncomingBase = value;
        }
    }
    public int[,] EntityToSpawnNextField
    {
        get
        {
            return _entityToSpawnNextField;
        }
        set
        {
            _entityToSpawnNextField = value;
        }
    }

    public float PeriodMoneySpawn
    {
        get
        {
            return _periodMoneySpawn;
        }
    }
    #endregion
    #region Events
    public delegate void OnWorldPassIncChange(float value);
    public static OnWorldPassIncChange onWorldPassIncChange;

    public delegate void OnChangeFloor(int[,] entityBuffer);
    public static OnChangeFloor onChangeFloor;

    public delegate void OnMoneyTick();
    public static OnMoneyTick onMoneyTick;
    #endregion
    /// <summary>
    /// В ОнИнейбл/ОнДизайбел мы подписываемся и отписываемяся от событий увеличения дохода пассивного и 
    /// Добавления обьекта в буфер спавна
    /// </summary>
    private void OnEnable()
    {
        FloorActions.onEntitySpawn += ChangePassiveIncomine;
        FloorActions.onSpawnToNewFloor += SpawnBufferFilling;
        _id = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnDisable()
    {
        FloorActions.onEntitySpawn -= ChangePassiveIncomine;
        FloorActions.onSpawnToNewFloor -= SpawnBufferFilling;
    }
    /// <summary>
    /// В эвейке устанавливанем индекс текущего этажа, активируем его
    /// и в циле инициализируем буфер
    /// </summary>
    private void Awake()
    {
        _currentFloor = 0;
        _floors[_currentFloor].GetComponent<FloorActions>().enabled = true;
        for (int i = 0; i < 7; i++)
        {
            _entityToSpawnNextField[i, 0] = i;
            _entityToSpawnNextField[i, 1] = 0;
        }
    }
    /// <summary>
    /// В апдейте просто крутим таймер выработки денег
    /// </summary>
    private void Update()
    {
        MoneySpawnCountdowning();
    }
    /// <summary>
    /// Метод изменения пассивного дохода
    /// </summary>
    /// <param name="value">То насколько увелициваем</param>
    private void ChangePassiveIncomine(float value)
    {
        _worldPassiveIncomingBase += value;
        _worldPassiveIncoming = _worldPassiveIncomingBase * PlaySettings.passiveIncomingMulti;
        onWorldPassIncChange(_worldPassiveIncoming);            // вызываем собтие которое обновит значение в ЮИ
    }
    /// <summary>
    /// Заполняем буфер когда, у нас появялется обьект уровня выше максимального на текущем этаже
    /// </summary>
    /// <param name="indexFloor"></param>
    /// <param name="buffer"></param>
    private void SpawnBufferFilling(int indexFloor, int buffer)
    {
        _entityToSpawnNextField[indexFloor, buffer] += 1;
        _floors[indexFloor].IsOpened = true;
    }
    /// <summary>
    /// Сменяем этаж, перемещая камеру и активируя/деактивируя компонетн ФлорАкшинс
    /// </summary>
    /// <param name="index"></param>
    public void ChangeFloor(int index)
    {
        _floors[_currentFloor].GetComponent<FloorActions>().enabled = false;
        _currentFloor = index;
        _floors[index].GetComponent<FloorActions>().enabled = true;
        Vector3 newPos = new Vector3(_floors[index].transform.position.x,
                                     _mainCamera.transform.position.y,
                                     _mainCamera.transform.position.z);
        _mainCamera.transform.position = newPos;

        onChangeFloor(_entityToSpawnNextField);
    }
    /// <summary>
    /// Таймер спавна денех
    /// </summary>
    private void MoneySpawnCountdowning()
    {
        if (_timerForMoneySpawn > 0)
        {
            _timerForMoneySpawn -= Time.deltaTime;

        }
        else
        {
            _timerForMoneySpawn = _periodMoneySpawn;
            if (onMoneyTick != null)
            {
                onMoneyTick();
            }

        }
    }
}
