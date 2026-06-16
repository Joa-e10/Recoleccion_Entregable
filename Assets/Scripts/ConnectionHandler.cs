using UnityEngine;
using Unity.Netcode;
public class ConnectionHandler : MonoBehaviour
{

    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }


    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("Se Desconecto el jugador nro: " + clientId);
    }
    void Update()
    {
        
    }
}
