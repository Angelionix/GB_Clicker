using UnityEngine;

public class Slot : MonoBehaviour
{
    private bool _isEmpty;
    private EntityData _entity;

    public bool IsEmpty
    {
        get
        {
            return _isEmpty;
        }
        set
        {
            _isEmpty = value;
        }
    }

    public EntityData EntityData
    {
        get 
        {
            return _entity;
        }
        set
        {
            _entity = value;   
        }
    }
}
