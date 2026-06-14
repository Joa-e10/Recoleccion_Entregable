using UnityEngine;

public class Goal : MonoBehaviour
{
    private int _pointsAdded = 0;
    private bool _collectionPoint;
    private Point _pointCollected;
    private Player _player;


    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        _player = other.GetComponent<Player>();

        if (_player != null)
        {
            Debug.Log("Entra en el trigger");
            _collectionPoint = _player.GetCollectionPoint();
            addScore();
            
            Debug.Log("El jugador tiene una puntuacion de: " + _pointsAdded);
            

        }
        else 
        {
            Debug.Log("No hay colision con un player");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Sale del trigger");
            _collectionPoint = false;
            _pointCollected = null;
        }
    }

    public void addScore() 
    {
        if (_collectionPoint == true)
        {
            _pointCollected = _player.GetGatheredPoint();
            _pointsAdded = _pointsAdded + _pointCollected.GetPointValue();
            _pointCollected.PointDespawn();
            _player.SetScore(_pointsAdded);
            _player.SetCollectedPoint(false);
        }
    }

    void Update()
    {
    }
}
