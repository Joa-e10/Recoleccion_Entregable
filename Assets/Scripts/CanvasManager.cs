using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;


public class CanvasManager : NetworkBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject _panelMainMenu;
    [SerializeField] private GameObject _panelSetIp;
    [SerializeField] private GameObject _waitingPanel;

    [Header("Botones")]
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;
    [SerializeField] private Button _JoinButton;
    [SerializeField] private Button _BackButton;
    [SerializeField] private Button _startGameButton;

    [Header("Texto/Input")]
    [SerializeField] private TMP_InputField _clientInputField;
    [SerializeField] private TextMeshProUGUI _waitingText;

    private bool _gameStarted = false; // Freno para que StartGame ocurra una sola vez

    private NetworkVariable<int> _quantityClients = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public static event Action OnSceneLoad;

    private void OnEnable()
    {
        _startHostButton.onClick.AddListener(StartHost);
        _startClientButton.onClick.AddListener(showPanelIp);
        _JoinButton.onClick.AddListener(StartClient);
        _BackButton.onClick.AddListener(HidePanelIp);
        _startGameButton.onClick.AddListener(StartGame);
    }

    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            Debug.LogError("¡No se encontró un NetworkManager en la escena!");
        }
    }

    private void OnDisable()
    {
        _startHostButton.onClick.RemoveListener(StartHost);
        _startClientButton.onClick.RemoveListener(showPanelIp);
        _JoinButton.onClick.RemoveListener(StartClient);
        _BackButton.onClick.RemoveListener(HidePanelIp);
        _startGameButton.onClick.RemoveListener(StartGame);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        Debug.Log("Se conecto el jugador nro: " + clientId);

        _quantityClients.Value++;

        if (clientId == 0)
        {
            Debug.Log("Entro el host de la sesion");
            _waitingText.text = "Waiting for players...";
        }
        else
        {
            Debug.Log("Entro el client en la sesion");
            _waitingText.text = "Loading game...";
        }

        
        if (_quantityClients.Value >= 2)// Si alcanzamos la cantidad de jugadores, el servidor inicia automáticamente
        {
            Debug.Log("iniciara la partida");
            StartGame();
        }
    }

    public void showPanelIp()
    { 
        _panelSetIp.SetActive(true);
    }
    public void HidePanelIp()
    {
     _panelSetIp.SetActive(false);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        _waitingPanel.SetActive(true);
    }

    public void StartClient()
    {
        if (_clientInputField != null)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(_clientInputField.text, (ushort)7777);
            NetworkManager.Singleton.StartClient();
            _waitingPanel.SetActive(true);
        }
    }

    public void StartGame()
    {
  
        if (_gameStarted) return;
        _gameStarted = true;


        RefreshSceneClientRpc();

        OnSceneLoad?.Invoke();


    }

    [ClientRpc]
    public void RefreshSceneClientRpc()
    {
        Debug.Log("[NETCODE] Orden recibida del servidor: Apagando todos los paneles.");


            _panelMainMenu.SetActive(false);
            _panelSetIp.SetActive(false); 
            _waitingPanel.SetActive(false);
    }
}

