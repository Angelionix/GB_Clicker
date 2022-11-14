using UnityEngine;

public class EntityData : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private float _manyPerSec =1;
    [SerializeField] private float _manyPerClick =2;
    [SerializeField] private byte _levelEntity;
    [SerializeField] private FloorData _floorData;
    [SerializeField] private Slot _currentSlot;

    public float ManyPerClick
    {
        get
        {
            return _manyPerClick;
        }
    }

    public byte LevelEntity
    {
        get
        {
            return _levelEntity;
        }
    }
    public float ManyPerSecond
    {
        get
        {
            return _manyPerSec;
        }
    }

    public string Name
    {
        get 
        {
            return _name;
        }
    }

    public FloorData FloorData
    {
        get
        {
            return _floorData;
        }
        set
        {
            _floorData = value;
        }
    }

    public Slot CurrentSlot
    {
        set
        {
            _currentSlot = value;
        }
        get
        {
            return _currentSlot;
        }
    }

    private void Awake()
    {
        _floorData = GetComponent<FloorData>();
    }
}
