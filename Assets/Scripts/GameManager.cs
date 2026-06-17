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
    public void InstantiateObject()
    {
        GameObject item = Instantiate(_pointPrefab);

        item.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawnearon objetos en escena");
    }

    private void OnEnable()
    {
        //CanvasManager.OnSceneLoad += StartTimer;
    }
    private void OnDisable()
    {
        
    }

    public void StartTimer() 
    {
        /*if (state == true) 
        {
            _timer += Time.deltaTime;
        }*/
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