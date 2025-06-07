using UnityEngine;

public class LinternaCamara : MonoBehaviour
{
    public Transform camara; // Asigna aquí la cámara del jugador en el Inspector
    public float velocidadSuavizado = 5f; // Ajusta para más o menos retraso

    private Vector3 offset; // Distancia inicial entre la linterna y la cámara

    void Start()
    {
        // Guarda la posición inicial de la linterna respecto a la cámara
        offset = transform.position - camara.position;
    }

    void Update()
    {
        // Mantiene la posición relativa original con suavizado
        Vector3 posicionObjetivo = camara.position + camara.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * velocidadSuavizado);

        // Suaviza la rotación de la linterna
        transform.rotation = Quaternion.Slerp(transform.rotation, camara.rotation, Time.deltaTime * velocidadSuavizado);
    }
}
