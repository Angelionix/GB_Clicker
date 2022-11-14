using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private double _coins;
    [SerializeField] private WorldManager _currentWorldManager;
    [SerializeField] private float _afkTime;

    private bool _saveDataExist;
    private IOData _ioData;
    private System.DateTime _timeOfAfkStart;
    private System.DateTime _currentTime;

    public static GameManager instance = null;

    public double Coins
    {
        get 
        {
            return _coins;
        }
        set
        {
            _coins = value;
        }
    }
    public System.DateTime TimeOfAfkStart
    {
        get
        {
            return _timeOfAfkStart;
        }
        set
        {
            _timeOfAfkStart = value;
        }
    }
    public WorldManager CurrentWodrlManager
    {
        get 
        {
            return _currentWorldManager;
        }
        set 
        {
            _currentWorldManager = value;
        }
    }

    public delegate void OnCoinValueChange(double amount);
    public static OnCoinValueChange onCoinValueChange;

    public delegate void OnLoadData(FloorData[] fl);
    public static OnLoadData onLoadData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        _currentTime = System.DateTime.Now;
        Time.timeScale = 0;
        _ioData = new IOData();
        _saveDataExist = _ioData.LoadFromJsonFile(ref instance, ref _currentWorldManager);
        ObtainingCoinsForAfk();

    }

    private void OnEnable()
    {
        EntityMakeMoney.onSpawnMoney += AddCoin;

    }

    private void OnDisable()
    {
        EntityMakeMoney.onSpawnMoney -= AddCoin;
    }

    private void Start()
    {
        if (onLoadData != null && _saveDataExist)
        {
            onLoadData(_currentWorldManager.Floors);
        }
    }

    private void OnApplicationQuit()
    {
        _timeOfAfkStart = System.DateTime.Now;
        _ioData.SaveToFile(this, ref _currentWorldManager);
    }
    /// <summary>
    /// добавление монет
    /// </summary>
    /// <param name="amount"></param>
    private void AddCoin(float amount)
    {
        _coins += amount*PlaySettings.clickDamageMulti;
        onCoinValueChange(_coins);
    }
    /// <summary>
    /// Получаем монетки когда не в игре
    /// </summary>
    private void ObtainingCoinsForAfk()
    {
        double afkTime = Utility.CalculationOfAFKTime(_timeOfAfkStart,_currentTime);
        _coins += _currentWorldManager.WorldPassiveIncoming * (afkTime / _currentWorldManager.PeriodMoneySpawn);
    }

    /// <summary>
    /// Метод для применения апгрейда
    /// </summary>
    /// <param name="index"></param>
    public void PowerUp(int index)
    {
        PlaySettings.PowerUP(index);
    }
    /// <summary>
    /// Выходим из игры
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }
/// <summary>
/// Метод паузы
/// </summary>
/// <param name="pause"></param>
    public void GamePause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    /// <summary>
    /// метод для обнуления сохраненных данных
    /// </summary>
    public void WipeData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.HasKey("save"));
    }
}
