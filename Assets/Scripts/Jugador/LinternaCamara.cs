using UnityEngine;

public class LinternaCamara : MonoBehaviour
{
    public Transform camara; // Asigna aqu� la c�mara del jugador en el Inspector
    public float velocidadSuavizado = 5f; // Ajusta para m�s o menos retraso

    private Vector3 offset; // Distancia inicial entre la linterna y la c�mara

    void Start()
    {
        // Guarda la posici�n inicial de la linterna respecto a la c�mara
        offset = transform.position - camara.position;
    }

    void Update()
    {
        // Mantiene la posici�n relativa original con suavizado
        Vector3 posicionObjetivo = camara.position + camara.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * velocidadSuavizado);

        // Suaviza la rotaci�n de la linterna
        transform.rotation = Quaternion.Slerp(transform.rotation, camara.rotation, Time.deltaTime * velocidadSuavizado);
    }
}
