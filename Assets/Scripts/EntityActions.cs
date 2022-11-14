using UnityEngine;

[RequireComponent(typeof(EntityData))]
public class EntityActions : MonoBehaviour
{
    [SerializeField] EntityData _entityData;
    [SerializeField] private float _distanceForCheckSlot;

    public delegate void OnSlotResease(Slot slot, int command);
    public static OnSlotResease onSlotResease;

    public delegate void OnEntityEvolution(EntityData entity);
    public static OnEntityEvolution onEntityEvolution;

    private void OnEnable()
    {
        DragNDrop.onDragEnding += ChangeSlot;
    }

    private void OnDisable()
    {
        DragNDrop.onDragEnding -= ChangeSlot;
    }

    private void Awake()
    {
        _entityData = GetComponent<EntityData>();
    }

    /// <summary>
    /// Метод проверяет есть ли слот поблизости
    /// _distanceForCheckSlot - дистанция на которой проверяются слоты. если их нет
    /// то метод взвращает текущий слот к которому приписан обьект
    /// </summary>
    /// <returns></returns>
    private Slot CheckNearSlots()
    {
        foreach (Slot slot in _entityData.FloorData.Slots)
        {
            if ((slot.GetComponent<Transform>().position - this.transform.position).magnitude < _distanceForCheckSlot)
            {
                return slot;
            }
        }
        return _entityData.CurrentSlot;
    }

    private void ChangeSlot()
    {
        Slot newSlot = CheckNearSlots();
        if (newSlot != _entityData.CurrentSlot)
        {
            if (newSlot.EntityData == null)
            {
                newSlot.EntityData = _entityData;
                onSlotResease(_entityData.CurrentSlot, 3);
                onSlotResease(_entityData.CurrentSlot, 2);
                _entityData.CurrentSlot.IsEmpty = true;
                _entityData.CurrentSlot.EntityData = null;
                _entityData.CurrentSlot = newSlot;
                _entityData.CurrentSlot.IsEmpty = false;
                onSlotResease(_entityData.CurrentSlot, 4);
                onSlotResease(_entityData.CurrentSlot, 1);
            }
            else if (newSlot.EntityData.LevelEntity == _entityData.LevelEntity)
            {
                if (onSlotResease != null)
                {
                    onSlotResease(_entityData.CurrentSlot, 3);
                    onSlotResease(_entityData.CurrentSlot, 2);
                }
                Destroy(this.gameObject);
                onEntityEvolution(newSlot.EntityData);             
            }
        }
        this.transform.position = _entityData.CurrentSlot.GetComponent<Transform>().position;
    }

    

}
