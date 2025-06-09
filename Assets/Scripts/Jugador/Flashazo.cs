using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashazo : MonoBehaviour
{
    // VARIABLES
    public List<Transform> Enemigos; // Lista de enemigos
    public Camera CamaraJugador; // C�mara del jugador
    [Range(0.4f, 0.6f)]
    public float centerThreshold = 0.5f; // Precisi�n del centro
    public bool cameraLooking;

    void Update()
    {
        CheckIfCameraIsLooking();
    }

    // DETECTAR SI ALG�N ENEMIGO EST� EN EL CENTRO DE LA VISTA
    public void CheckIfCameraIsLooking()
    {
        cameraLooking = false; // Reiniciar estado

        foreach (Transform enemigo in Enemigos)
        {
            Vector3 screenPoint = CamaraJugador.WorldToViewportPoint(enemigo.position);

            bool isCentered = screenPoint.x > (0.5f - centerThreshold) && screenPoint.x < (0.5f + centerThreshold)
                           && screenPoint.y > (0.5f - centerThreshold) && screenPoint.y < (0.5f + centerThreshold)
                           && screenPoint.z > 0;

            if (isCentered)
            {
                cameraLooking = true;
                break; // Salir del bucle si ya encontr� uno
            }
        }

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
        Debug.Log("�El jugador est� mirando a un enemigo!");
    }

    void PlayerIsNotLooking()
    {
        Debug.Log("El jugador no est� mirando a ning�n enemigo");
    }
}
