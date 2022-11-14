using UnityEngine;
using UnityEngine.UI;

public class UISCoreUpdater : MonoBehaviour
{
    [SerializeField] private Text _totalCoinsText;
    [SerializeField] private Text _currentWorldIncomeing;
    [SerializeField] private Text _delayTimerText;
    [SerializeField] private Image _spawnButtonImage;
    [SerializeField] private GameObject[] _buttonFloors;

    private void OnEnable()
    {
        GameManager.onCoinValueChange += ChangeCoinsOnUi;                   //������� ��������� ���-�� ������
        WorldManager.onWorldPassIncChange += ChangeWorldPassIncText;        //������� ��������� ���������� ������
        FloorActions.onChangeSpawnTimerValue += ChangingSpawnDelayTimer;    //������� ��������� �������� ������� ������
        FloorActions.onSpawnToNewFloor += FloorButtonsActivating;           // ������� ��� ��������� ������ ������ � �������� ����
        GameManager.onLoadData += FloorButtonsActivating;                   // ������� ��� ��������� ������ ������ ��� �������� ����
    }

    private void OnDisable()
    {
        GameManager.onCoinValueChange -= ChangeCoinsOnUi;
        WorldManager.onWorldPassIncChange -= ChangeWorldPassIncText;
        FloorActions.onChangeSpawnTimerValue -= ChangingSpawnDelayTimer;
        FloorActions.onSpawnToNewFloor -= FloorButtonsActivating;
        GameManager.onLoadData -= FloorButtonsActivating;
    }
    /// <summary>
    /// ����� ���������� ������� ������
    /// </summary>
    /// <param name="value">�������� �������</param>
    private void ChangingSpawnDelayTimer(float value)
    {
        _delayTimerText.text = ((int)value).ToString();
        if (value > 0)
        {
            _spawnButtonImage.fillAmount = 1 / value;
        }
        else
        {
            _spawnButtonImage.fillAmount = 1;
        }
        
    }
    /// <summary>
    /// ����� ���������� ���-�� ������
    /// </summary>
    /// <param name="value">���-�� ������</param>
    private void ChangeCoinsOnUi(double value)
    {
        _totalCoinsText.text = CoingValueFormating(value);
    }
    /// <summary>
    /// ����� ��������� ������
    /// </summary>
    /// <param name="floorIndex">������ �����/������ </param>
    /// <param name="count">�� �����</param>
    private void FloorButtonsActivating(int floorIndex, int count)
    {
        if (floorIndex < _buttonFloors.Length)
        {
            _buttonFloors[floorIndex].SetActive(true);
        }
    }
    /// <summary>
    /// ���������� �������� ��� ����������� ����� �������
    /// </summary>
    /// <param name="fd">������ ������</param>
    private void FloorButtonsActivating(FloorData[] fd)
    {

        for(int i=0; i < fd.Length; i++)
        {
            if (fd[i].IsOpened)
            {
                _buttonFloors[i].SetActive(true);
            }
        }
    }
    /// <summary>
    /// ����� ��� ���������� �������� ���������� ������ � ��
    /// </summary>
    /// <param name="value">�������� ���������� ������</param>
    private void ChangeWorldPassIncText(float value)
    {
        _currentWorldIncomeing.text = CoingValueFormating(value);
    }
    /// <summary>
    /// ����� �������������� ������ �������� � ��
    /// </summary>
    /// <param name="value">�������� � ������</param>
    /// <returns></returns>
    private string CoingValueFormating(double value)
    {
        int step = 0;
        string prefix = "";
        while (value > 1000)
        {
            value = value / 1000;
            step++;
        }
        switch (step)
        {
            case 0:// ��� coins < 1000
                prefix = "";
                value = System.Math.Floor(value);
                break;
            case 1: // 1000 <=coins< 10^6
                prefix = "K";
                break;
            case 2: // 10^6 <=coins< 10^9
                prefix = "M";
                break;
            case 3: // 10^9 <=coins< 10^12
                prefix = "B";
                break;
            case 4: // 10^12 <=coins< 10^15
                prefix = "T";
                break;
            case 5: // 10^15 <=coins< 10^18
                prefix = "QD";
                break;
            case 6: // 10^18 <=coins< 10^21
                prefix = "QN";
                break;
            case 7: // 10^21 <=coins< 10^24
                prefix = "SX";
                break;
            case 8: // 10^24 <=coins< 10^27
                prefix = "SP";
                break;
            case 9: // 10^27 <=coins< 10^30
                prefix = "OT";
                break;
        }

        return $"{value:###.##}{prefix}";
    }
}
