using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]private GameObject _panelMainMenu;
    [SerializeField]private GameObject _panelSetIp;
    [SerializeField] private GameObject _waitingPanel;
    [SerializeField]private Button _startHostButton;
    [SerializeField] private Button _startClientButton;
    [SerializeField]private Button _JoinButton;
    [SerializeField] private Button _BackButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField]private TMP_InputField _clientInputField;
    [SerializeField] private TextMeshProUGUI _waitingText;

    private ulong _clientID;
    private bool _isClientReady;
    private bool _isHostReady;
    private int _quantityClients = 0;
    private NetworkObject _client;
    private ulong _hostClientId;
    private string _newText;

    private void OnEnable()
    {
        // Las suscripciones a componentes de la misma escena (UI) están perfectas aquí
        _startHostButton.onClick.AddListener(StartHost);
        _startClientButton.onClick.AddListener(showPanelIp);
        _JoinButton.onClick.AddListener(StartClient);
        _BackButton.onClick.AddListener(HidePanelIp);
        _startGameButton.onClick.AddListener(StartGame);
    }

    void Start()
    {
        // Movemos la suscripción del NetworkManager aquí.
        // Para este punto, todos los Awake() del juego ya corrieron,
        // garantizando que el Singleton ya está listo.
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
        // Removemos listeners de botones de forma segura
        if (_startHostButton != null) _startHostButton.onClick.RemoveListener(StartHost);
        if (_startClientButton != null) _startClientButton.onClick.RemoveListener(showPanelIp);
        if (_JoinButton != null) _JoinButton.onClick.RemoveListener(StartClient);
        if (_BackButton != null) _BackButton.onClick.RemoveListener(HidePanelIp);
        if (_startGameButton != null) _startGameButton.onClick.RemoveListener(StartGame);

        // Desuscripción segura del Singleton de red
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
    public void OnClientConnected(ulong clientId)
    {
        
        if (NetworkManager.Singleton == null)
            Debug.Log("NetworkManager es NULL");

        if (_waitingText == null)
            Debug.Log("_waitingText es NULL");

        if (_startGameButton == null)
            Debug.Log("_startGameButton es NULL");

        Debug.Log("Se conecto el jugador nro: " + clientId);
        

        if (_isHostReady)
        {
            _quantityClients++;
            Debug.Log("Entro el host de la sesion");
            _newText = "Waiting for players";
            _waitingText.text = _newText;
        }

        if (_isClientReady)
        {
            _quantityClients++;
            Debug.Log("Entro el client en la sesion");
            _newText = "Loading game...";
            _waitingText.text = _newText;
        }

        if (_quantityClients >= 2)
        {
            Debug.Log("Entro en la activacion del boton start");
            _newText = "Ready to start the game";
            _startGameButton.enabled = true;
        }
        Debug.Log($"Hay {_quantityClients} en partida");
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
        _isHostReady = true;
        NetworkManager.Singleton.StartHost();
        _waitingPanel.SetActive(true);

    }

    public void StartClient()
    {
        if (_clientInputField != null)
        {
            _isClientReady = true;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(_clientInputField.text, (ushort)7777); //Consultar con la IA y probar si se conectan dos
            NetworkManager.Singleton.StartClient();
            _waitingPanel.SetActive(true);

        }
    }

    public void StartGame()
    {

        NetworkManager.Singleton.SceneManager.LoadScene(
        "SampleScene",
         LoadSceneMode.Single);
    }

    private void Update()
    {
        if (_quantityClients >= 2)
        {
            StartGame();
            
        }
    }
}
