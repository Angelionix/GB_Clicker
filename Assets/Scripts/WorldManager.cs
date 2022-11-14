using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// ����� �������� �� ������ �������� ����
/// </summary>
public class WorldManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private int _id;                                               //�� ������ ���� ����� ����� ����-�����
    [SerializeField] private Camera _mainCamera;                                    //������ �� ������� ������
    [SerializeField] private FloorData[] _floors;                                   //����� ������ � ������� ����
    [SerializeField] private float _periodMoneySpawnBase;                           //�� ������ �������
    [SerializeField] private float _periodMoneySpawn;                               
    [SerializeField] private float _timerForMoneySpawn;                             //���������� ������ ������ �������
    [SerializeField] private int _currentFloor;                                     //������ ��������� �����
    [SerializeField] private float _worldPassiveIncomingBase;                       //��������� ����� ����, ��� ������
    [SerializeField] private float _worldPassiveIncoming;                           //��������� ����� ��� � ��������
    [SerializeField] private int[,] _entityToSpawnNextField = new int[7, 2];        // �����, � ������� ������ ������� �������� �����
                                                                                    // ������� ��� �������� �� ���� �������
                                                                                    // 1 ������� - ����, 2 - ������� ���� ����������
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
    /// � ��������/���������� �� ������������� � ������������� �� ������� ���������� ������ ���������� � 
    /// ���������� ������� � ����� ������
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
    /// � ������ �������������� ������ �������� �����, ���������� ���
    /// � � ���� �������������� �����
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
    /// � ������� ������ ������ ������ ��������� �����
    /// </summary>
    private void Update()
    {
        MoneySpawnCountdowning();
    }
    /// <summary>
    /// ����� ��������� ���������� ������
    /// </summary>
    /// <param name="value">�� ��������� �����������</param>
    private void ChangePassiveIncomine(float value)
    {
        _worldPassiveIncomingBase += value;
        _worldPassiveIncoming = _worldPassiveIncomingBase * PlaySettings.passiveIncomingMulti;
        onWorldPassIncChange(_worldPassiveIncoming);            // �������� ������ ������� ������� �������� � ��
    }
    /// <summary>
    /// ��������� ����� �����, � ��� ���������� ������ ������ ���� ������������� �� ������� �����
    /// </summary>
    /// <param name="indexFloor"></param>
    /// <param name="buffer"></param>
    private void SpawnBufferFilling(int indexFloor, int buffer)
    {
        _entityToSpawnNextField[indexFloor, buffer] += 1;
        _floors[indexFloor].IsOpened = true;
    }
    /// <summary>
    /// ������� ����, ��������� ������ � ���������/����������� ��������� ����������
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
    /// ������ ������ �����
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
