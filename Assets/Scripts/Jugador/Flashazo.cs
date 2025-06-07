using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashazo : MonoBehaviour
{
    // VARIABLES
    public Transform Enemigo;
    public Camera CamaraJugador; // Usar la cámara en vez de Transform
    [Range(0.4f, 0.6f)]
    public float centerThreshold = 0.5f; // Ajustar para que sea más estricto en el centro
    public bool cameraLooking;

    void Update()
    {
        CheckIfCameraIsLooking();
    }

    // DETECTAR SI EL ENEMIGO ESTÁ EN EL CENTRO DE LA VISTA
    public void CheckIfCameraIsLooking()
    {
        // Convertir la posición del enemigo a coordenadas de viewport (0 a 1)
        Vector3 screenPoint = CamaraJugador.WorldToViewportPoint(Enemigo.position);

        // Verificar si está dentro del centro de la pantalla
        bool isCentered = screenPoint.x > (0.5f - centerThreshold) && screenPoint.x < (0.5f + centerThreshold)
                       && screenPoint.y > (0.5f - centerThreshold) && screenPoint.y < (0.5f + centerThreshold)
                       && screenPoint.z > 0; // Asegurar que no está detrás del jugador

        cameraLooking = isCentered;

        // Llamar a funciones según el estado
        if (cameraLooking)
        {
            PlayerIsLooking();
        }
        else
        {
            PlayerIsNotLooking();
        }
    }

    void PlayerIsLooking()
    {
        Debug.Log("¡El jugador está mirando al enemigo!");
    }

    void PlayerIsNotLooking()
    {
        Debug.Log("El jugador no está mirando al enemigo");
    }
}
