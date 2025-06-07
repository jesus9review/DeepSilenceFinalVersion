using System.Collections;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    public Animator animPuerta;
    public bool rangoPuerta = false;
    public bool puertaAbierta = false;

    void Start()
    {
        animPuerta = GetComponent<Animator>();
    }

    private void Update()
    {
        if (rangoPuerta && Input.GetKeyDown(KeyCode.F))
        {
            AlternarPuerta();
        }
    }

    void AlternarPuerta()
    {
        if (!puertaAbierta) // Si está cerrada, ábrela
        {
            puertaAbierta = true;
            animPuerta.SetBool("Cerrado", false);
            animPuerta.SetBool("Abierto", true);
            animPuerta.SetBool("Ocio", false);
        }
        else // Si está abierta, ciérrala
        {
            puertaAbierta = false;
            animPuerta.SetBool("Cerrado", true);
            animPuerta.SetBool("Abierto", false);
            animPuerta.SetBool("Ocio", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangoPuerta = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangoPuerta = false;
        }
    }
}
