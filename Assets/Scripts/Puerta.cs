using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Animator animPuerta;
    public bool rangoPuerta;
    public bool puertaAbierta = false;
    public BoxCollider colliderPuerta;

    //Inventario
    [SerializeField]
    private BaseDeDatosJugador inventarioJugador;

    //Atributo que requiere llave la primera vez
    public bool requiereLlave = true;


    void Start()
    {
        animPuerta = GetComponent<Animator>();

        //Busca la etiqueta Player donde está la base de datos de los items
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            inventarioJugador = jugador.GetComponent<BaseDeDatosJugador>();
        }
    }

    private void Update()
    {
        if(rangoPuerta == true && Input.GetKey(KeyCode.F)) 
        { 
            InteraccionPuerta();
        }
    }

    void InteraccionPuerta()
    {  
        if (inventarioJugador == null) return;

        // Si requiere llave y el jugador NO la tiene
        if (requiereLlave && !inventarioJugador.TieneObjeto("llave"))
        {
            Debug.Log("Necesitas una llave para abrir esta puerta.");
            Debug.Log("Activar Interfaz de aviso.");
            return;
        }

        // Si requiere llave y el jugador la tiene, se usa
        if (requiereLlave && inventarioJugador.TieneObjeto("llave"))
        {
            inventarioJugador.EliminarObjeto("llave");
            requiereLlave = false; // ya no se necesitará más
            Debug.Log("Usaste la llave para abrir la puerta.");
        }

        if (puertaAbierta == false)
        {
            puertaAbierta = true;
            animPuerta.SetBool("Abierto", true);
            animPuerta.SetBool("Cerrado", false);
            animPuerta.SetBool("Ocio", false);
            animPuerta.SetBool("OcioAbierto", true);
            colliderPuerta.enabled = false;
            
        }
        else if (puertaAbierta == true)
        {
            puertaAbierta = false;
            animPuerta.SetBool("Cerrado", true);
            animPuerta.SetBool("Abierto", false);
            animPuerta.SetBool("Ocio", true);
            animPuerta.SetBool("OcioAbierto", false);
        }
    }

    private void OnTriggerStay(Collider puerta)
    {
        if (puerta.gameObject.tag == "Player")
        {
            rangoPuerta = true;
        }
    }

    private void OnTriggerExit(Collider puerta)
    {
        if (puerta.gameObject.tag == "Player")
        {
            rangoPuerta = false;
        }
    }

}
