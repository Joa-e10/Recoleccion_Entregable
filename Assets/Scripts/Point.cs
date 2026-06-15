using UnityEngine;
using Unity.Netcode;
public class Point : NetworkBehaviour
{
    private MeshRenderer _meshRenderer;
    private Transform _transformPoint;
   [SerializeField] private PointData _pointData;
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
       _meshRenderer = GetComponent<MeshRenderer>();
        _transformPoint = GetComponent<Transform>();
        
    }

    public PointData GetPointData() 
    {
    return _pointData;
    }

    public void PointDespawn() 
    {
            GetComponent<NetworkObject>().Despawn(true);
    }

    [ServerRpc]
    private void despawnServerRpc() 
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    void Update()
    {
    }
}
