using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{

    private CharacterController _characterController;
    [SerializeField]private PlayerInput _playerInput;
   [SerializeField]private GameObject _cam;
    private Point _currentPoint;
    private Point _gatheredPoint;
    private int _quantityPress;
    private bool _itemInRange;
    private bool _collectionPoint;
    private NetworkVariable<int> _score = new NetworkVariable<int>(0);

    //CAMARA Y MOVIMIENTO DEL CHARACTER CONTROLLER
    private Vector2 _input;
    private float _speed = 5f;
    private float _yVelocity;
    private float _gravity = -9.81f;

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
    }
    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDirection = GetCameraRelativeDirection();

        RotateCharacter(moveDirection);
        moveDirection = ApplyGravity(moveDirection);
        MoveCharacter(moveDirection);

        Debug.Log("El contador esta en: "+_quantityPress);
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
        if (value.isPressed && _itemInRange)
        {
            _quantityPress++;
            _collectionPoint = true;
            _currentPoint.SetIsCollected(_collectionPoint);
            _gatheredPoint = _currentPoint;
             Debug.Log("Esta en rango y lo recolecto");
        }
        if (_quantityPress == 1 && _collectionPoint == true)
        {
            if (_gatheredPoint != null)
            {
                _collectionPoint = false;
                _gatheredPoint.SetIsReleased(transform.position);
                _gatheredPoint.SetIsCollected(_collectionPoint);
                _gatheredPoint.Released();
                _gatheredPoint = null;

            }
        }
        else 
        {
            _quantityPress = 0;
        }
            _quantityPress++;
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
