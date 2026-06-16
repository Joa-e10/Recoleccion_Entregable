using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ServerManager : MonoBehaviour
{
    private GameObject _client;
    private string colors;
    [SerializeField] private GameObject _pointPrefab;
    public void InstantiateObject()
    {
        GameObject item = Instantiate(_pointPrefab);

        item.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawnearon objetos en escena");
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