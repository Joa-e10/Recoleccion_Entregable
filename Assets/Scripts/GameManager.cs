using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private GameObject _client;
    private string colors;
    private float _timer;
    private int _scoreShow = 0;
    //jose
    [SerializeField] private Transform _spawn1;
    [SerializeField] private Transform _spawn2;
    [SerializeField] private Transform _spawn3;
    [SerializeField] private Transform _spawn4;
    [SerializeField] private Transform _spawn5;
    [SerializeField] private GameObject[] _listPoints = new GameObject[5];
    [SerializeField] private Transform[] _listSpawn = new Transform[5];
    [SerializeField] private GameObject _panelGameUi;
    [SerializeField] private TextMeshProUGUI _timeValue;
    [SerializeField] private GameObject _timerObject;
    [SerializeField]private CanvasManager _canvasManager;

    public static event Action<bool> OnTimeRunning;
    public void InstantiateObjects()
    {

            for (int i = 0; i < _listSpawn.Length; i++)
            {
            GameObject[] _listObj= new GameObject[5];
                _listObj[i] = Instantiate(_listPoints[i], _listSpawn[i].position, Quaternion.identity);

                _listObj[i].GetComponent<NetworkObject>().Spawn();

                Debug.Log($"Se spawneo {i} punto en {_listSpawn[i]}");
            }
        
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == 0)
        {
            InstantiateObjects();
            _canvasManager._scoreHost.text = _scoreShow.ToString();
        }
        else 
        {
            _canvasManager._scoreClient.text = _scoreShow.ToString();
        }
    }

    public void setScoreShow(int value) 
    {
        _scoreShow = value;
    }

    private void OnEnable()
    {
        
        CanvasManager.OnSceneLoad += StartTimerServerRpc;
    }
    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        CanvasManager.OnSceneLoad -= StartTimerServerRpc;
    }

    [ServerRpc]
    public void StartTimerServerRpc()
    {
        _panelGameUi.SetActive(true);
        _timerObject.SetActive(true);
        OnTimeRunning?.Invoke(true);
    }

    void Update()
    {
        
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartHost(); //Iniciamos el Host

        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartClient(); //Iniciamos como client

        }
    }
}