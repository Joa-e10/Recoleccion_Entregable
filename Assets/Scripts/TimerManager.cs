using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private float _timer = 100;
    private bool _timerRunning = false;
    [SerializeField]private TextMeshProUGUI _timeValue;
    void Start()
    {
        GameManager.OnTimeRunning += SetTimer;
    }

    public void SetTimer(bool newValue) 
    {
        _timerRunning = newValue;
    }


    public void timeEnded() 
    {
        if (_timer <= 0) 
        {
            Debug.Log("La partida termino");
        }
    }

    void Update()
    {
        if (_timerRunning) 
        {
            _timer -= Time.deltaTime;
            _timeValue.text = "" + _timer.ToString("f0");
            timeEnded();
        }
    }
}
