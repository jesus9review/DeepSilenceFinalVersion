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


    private void Awake()
    {
        contenedorCodigos = GameObject.FindGameObjectWithTag("Player");
        nombreObjetos = contenedorCodigos.GetComponent<NombresObjetos>();
        baseDeDatosJugador = contenedorCodigos.GetComponent<BaseDeDatosJugador>();
    }

    void Update()
    {
        Debug.DrawRay(inicioRayCast.position, inicioRayCast.forward, Color.red);

        /////BUSCA SI RECOJO EL OBJETO CORRECTO///////
        
            if (Physics.Raycast(inicioRayCast.position, inicioRayCast.forward, out hit, 10)) {
                if (Input.GetKey(KeyCode.F))
                {
                    for (int i = 0; i < nombreObjetos.ItemsString.Length; i++)
                    {
                        if (hit.collider.CompareTag(nombreObjetos.ItemsString[i]))
                        {
                            Destroy(hit.collider.gameObject);
                        RecogerObjeto();
                        }
                    }
                }
            }
    }

    void RecogerObjeto()
    {
        for(int i=0; i < baseDeDatosJugador.slotsBasicos.Length; i++)
        {
            if (baseDeDatosJugador.slotsBasicos[i] == "") {
                 baseDeDatosJugador.slotsBasicos[i] = hit.collider.tag;
                 break;
            }
        }
    }

}
