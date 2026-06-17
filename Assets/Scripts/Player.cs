using System.Globalization;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{

    private CharacterController _characterController;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _cam;
    private Point _currentPoint;
    private PointData _currentPointData;
    private bool _itemInRange;
    private NetworkVariable<int> _score = new NetworkVariable<int>(0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);
    private bool _collectedPoint;

    //NETWORK TRANSFORM
    [SerializeField]private Transform _transformSpawnHost;
    [SerializeField]private Transform _transformSpawnClient;

    private Transform _transform;
    private NetworkTransform _transformN;

    //CAMARA Y MOVIMIENTO DEL CHARACTER CONTROLLER
    private Vector2 _input;
    private float _speed = 5f;
    private float _yVelocity;
    private float _gravity = -9.81f;

    private void Awake()
    {
        
    }
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkSpawn()
    {
        _playerInput.enabled = IsOwner;
        _cam.SetActive(IsOwner);
        _transformN = GetComponent<NetworkTransform>();
        _transform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        CanvasManager.OnSceneLoad += NetTransformDisable;
    }
    private void OnDisable()
    {
        CanvasManager.OnSceneLoad -= NetTransformDisable;
    }
    public void NetTransformDisable()
    {
        if (IsServer)
        {
            Debug.Log("Entro el papu host");
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {

                Debug.Log("Se instancias para sus spawns");
                if (client.ClientId == 0)
                {
                    var transformN = client.PlayerObject.GetComponent<NetworkTransform>();
                    transformN.Teleport(_transformSpawnHost.position, Quaternion.identity, transform.localScale);

                }
                else
                {
                    var transformN = client.PlayerObject.GetComponent<NetworkTransform>();
                    transformN.Teleport(_transformSpawnClient.position, Quaternion.identity, transform.localScale);

                }
            }

        }
            
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDirection = GetCameraRelativeDirection();

        RotateCharacter(moveDirection);
        moveDirection = ApplyGravity(moveDirection);
        MoveCharacter(moveDirection);
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Esta tocando la q");
        }
        
    }


    public void SetScore(int value)
    {
        _score.Value = value;
    }

    private Vector3 GetCameraRelativeDirection()
    {
        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        return camRight * _input.x + camForward * _input.y;
    }

    private Vector3 ApplyGravity(Vector3 moveDirection)
    {
        if (_characterController.isGrounded && _yVelocity < 0)
        {
            _yVelocity = -2f;
        }

        _yVelocity += _gravity * Time.deltaTime;
        moveDirection.y = _yVelocity;

        return moveDirection;
    }
    private void RotateCharacter(Vector3 moveDirection)
    {
        if (moveDirection.magnitude <= 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    private void MoveCharacter(Vector3 moveDirection)
    {
        _characterController.Move(moveDirection * _speed * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }

    private void OnPickUp(InputValue value)
    {
        if (value.isPressed)
        {
            if (!_collectedPoint && _itemInRange)
            {
                _currentPointData = _currentPoint.GetPointData();
                pickUpServerRpc();
                _collectedPoint = true;
            }
        }
    }

    [ServerRpc]
    private void pickUpServerRpc() 
    {
        Debug.Log("El currentPoint: "+_currentPoint);

        if (_currentPoint != null)
        {
            _currentPoint.PointDespawn();
        }
        else
        {
            Debug.Log("El currentPoint para el server es null");
        }
    }

    public bool GetCollectPoint()
    {
    return _collectedPoint;
    }
    public void SetCollectPoint(bool newState)
    {
       _collectedPoint = newState;
    }


    public PointData GetCollectPointData()
    {
        return _currentPointData;
    }


    private void OnTriggerEnter(Collider other)
    {
        Point _point = other.GetComponent<Point>();

        if (_point != null) 
        {
            _currentPoint = _point;
            _itemInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Point _point = other.GetComponent<Point>();

        if (_point != null)
        {
            _currentPoint = null;
            _itemInRange = false;
        }
    }
}
