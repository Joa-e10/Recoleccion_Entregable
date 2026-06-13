using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
public class Point : NetworkBehaviour
{
    private bool _isCollected;
    private MeshRenderer _meshRenderer;
    private Transform _transformPoint;
    private Vector3 _newPosition;
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
       _meshRenderer = GetComponent<MeshRenderer>();
        _transformPoint = GetComponent<Transform>();
    }
    public void SetIsCollected(bool state) 
    {
        _isCollected = state;
    }
    public void SetIsReleased(Vector3 newPosition)
    {
        _newPosition = newPosition;
    }

    public void Collected() 
    {
        if (_isCollected)
        {
            _meshRenderer.enabled = false;
        }
        else 
        {
            _meshRenderer.enabled = true;
        }
    }

    public void Released() 
    {
        _transformPoint.position = _newPosition;
        Debug.Log("El item tiene una nueva posicion en el mundo");
    }
    void Update()
    {
        Collected();
        Debug.Log("Estamos con el mesh en: "+_meshRenderer.enabled);
    }
}
