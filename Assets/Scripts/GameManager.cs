using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private GameObject _client;
    private string colors;
    private float _timer;
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private GameObject _panelGameUi;
    [SerializeField] private TextMeshProUGUI _timeValue;
    [SerializeField] private GameObject _timerObject;

    public static event Action<bool> OnTimeRunning;
    public void InstantiateObject()
    {
        /*GameObject timerGame = Instantiate(_timerObject);

        timerGame.GetComponent<NetworkObject>().Spawn();*/
        GameObject item = Instantiate(_pointPrefab);

        item.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawnearon objetos en escena");
    }

    private void OnEnable()
    {
        CanvasManager.OnSceneLoad += StartTimerServerRpc;
    }
    private void OnDisable()
    {
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
            InstantiateObject();
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartClient(); //Iniciamos como client

        }

    }
}