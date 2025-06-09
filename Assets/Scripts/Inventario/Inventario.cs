using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public Transform inicioRayCast;
    private GameObject contenedorCodigos;
    private NombresObjetos nombreObjetos;
    private BaseDeDatosJugador baseDeDatosJugador;
    private RaycastHit hit;

    public NotaController notaController;

    private void Awake()
    {
        contenedorCodigos = GameObject.FindGameObjectWithTag("Player");
        nombreObjetos = contenedorCodigos.GetComponent<NombresObjetos>();
        baseDeDatosJugador = contenedorCodigos.GetComponent<BaseDeDatosJugador>();
    }

    void Update()
    {
        if (notaController.siNotaActiva) return;

        Debug.DrawRay(inicioRayCast.position, inicioRayCast.forward, Color.red);

        if (Physics.Raycast(inicioRayCast.position, inicioRayCast.forward, out hit, 10))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                string tagObjeto = hit.collider.tag;

                // Verifica si el tag está en la lista de objetos válidos
                for (int i = 0; i < nombreObjetos.ItemsString.Length; i++)
                {
                    if (tagObjeto == nombreObjetos.ItemsString[i])
                    {
                        // Si el tag empieza con "nota"
                        if (tagObjeto.StartsWith("nota"))
                        {
                            // Ej: nota1 -> índice 0
                            string numeroStr = tagObjeto.Replace("nota", "");
                            if (int.TryParse(numeroStr, out int indexNota))
                            {
                                indexNota -= 1;
                                if (indexNota >= 0 && indexNota < notaController.notasVisuales.Length)
                                {
                                    GameObject notaGO = notaController.notasVisuales[indexNota];
                                    notaController.MostrarNota(notaGO);
                                }
                            }
                        }

                        // Guarda el objeto recogido y lo destruye
                        RecogerObjeto(tagObjeto);
                        Destroy(hit.collider.gameObject);
                        break;
                    }
                }
            }
        }
    }

    void RecogerObjeto(string tagObjeto)
    {
        for (int i = 0; i < baseDeDatosJugador.slotsBasicos.Length; i++)
        {
            if (baseDeDatosJugador.slotsBasicos[i] == "")
            {
                baseDeDatosJugador.slotsBasicos[i] = tagObjeto;
                break;
            }
        }
    }
}
