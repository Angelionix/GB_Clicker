using UnityEngine;

public class EntityMakeMoney : MonoBehaviour
{
    [SerializeField] private EntityData _entity;

    public delegate void OnSpawnMoney(float amount);
    public static OnSpawnMoney onSpawnMoney;

    private void OnEnable()
    {
        WorldManager.onMoneyTick += PeiodicMoneySpawning;
    }

    private void OnDisable()
    {
        WorldManager.onMoneyTick -= PeiodicMoneySpawning;
    }

    private void Awake()
    {
        _entity = GetComponent<EntityData>();
    }
    private void PeiodicMoneySpawning()
    {
        if (onSpawnMoney != null)
        {
            onSpawnMoney(_entity.ManyPerSecond);
        }
    }

    public void SpawnMoneyByClicking()
    {
        onSpawnMoney(_entity.ManyPerClick*PlaySettings.clickDamageMulti);
    }

}
