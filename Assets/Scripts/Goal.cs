using UnityEngine;

public class Goal : MonoBehaviour
{
    
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = GetComponent<Player>();

        if (player != null) 
        {
            //cargar los puntos
            //Actualizar el UI
        }
    }

    void Update()
    {
        
    }
}
