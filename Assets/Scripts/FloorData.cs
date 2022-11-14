using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����� �������� � ���� ������ ������ �����
/// </summary>
public class FloorData : MonoBehaviour
{
    [SerializeField] private bool _isOpened;                                //������ �� ����
    [SerializeField] private int _indexFloor;                               //������ �����
    [SerializeField] private GameObject _slotsContainer;                    //��������� � ������� �� ������ �����
    [SerializeField] private Slot[] _slots;                                 //���� ������ �� �����
    [SerializeField] private List<Slot> _emptySlots = new List<Slot>();     //������ ������ ������
    [SerializeField] private List<Slot> _fullSlots = new List<Slot>();      //������ ������� ������
    [SerializeField] private EntityData[] _entitySet;                       //����� �������� ��� �������� "�����"
    [SerializeField] private int[] _entitiesToSpawnFromFile;                //������� ��� ������ ���������� �� �����
    [SerializeField] private float _periodMoneySpawnBase;                   //�������� ��������� ����� ������� ��� ������
    [SerializeField] private float _periodMoneySpawn;                       //������� ��������� ����� � ��������
    [SerializeField] private float _periodEntitySpawn;                      //�������� ������ ��������
    [SerializeField] private System.DateTime _timeOfDeactivatingFloor;      // ����� � ���� ����� ���� ��� ��������
    [SerializeField] private System.DateTime _timeOfActivatingFloor;        //����� � ���� ����� ���� ��� �������
    
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
    /// � ������ �� �������������� �� ������ ������� ����� ��� ������ �����
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
