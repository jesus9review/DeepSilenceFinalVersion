using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo1 : MonoBehaviour
{
    public Transform PosicionJugador;
    public MicrofonoManager Microfono;
    public float Velocidad;
    public float DistMin;
    public float DistAtaque;
    public float SensibilidadSonido = 0.5f;
    public float TiempoMaxPersecucion = 10f;
    public float tiempoEsperaFlash = 3f;

    public NavMeshAgent IA;
    public bool Alerta = false;
    private bool Persiguiendo = false;
    private bool aturdido = false;
    private float tiempoPersecucion = 0f;

    public Animator Anim;
    public GameObject Jugador;
    public FirstPersonController datosJugador;
    public Flashazo visionJugador;
    public ElectricTorchOnOff linterna;
    private Vector3 posicionInicial;


    void Start()
    {
        Anim = GetComponent<Animator>();
        Jugador = GameObject.Find("Jugador");
        posicionInicial = transform.position;
    }

    void Update()
    {
        /*if (!aturdido)
        {
            ComportamientoEnemigo();
        }
        else
        {
            StartCoroutine(RecuperarseDelFlash());
        }*/
        Vector3 posicionActual = transform.position;

        if (!aturdido)
        {
            ComportamientoEnemigo();
        }
        else
        {
            StartCoroutine(RecuperarseDelFlash());
            if (posicionActual != posicionInicial)
            {
                Anim.SetBool("Agonia", false);
                Anim.SetBool("Correr", true);
            }
        }

        posicionInicial = posicionActual;
    }

    public void ComportamientoEnemigo()
    {
        IA.speed = Velocidad;

        if (Microfono.ruidoTotal > SensibilidadSonido && Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin)
        {
            if (!Alerta)
            {
                StartCoroutine(ActivarAlerta());
            }
            if (Microfono.ruidoTotal < SensibilidadSonido && Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin && tiempoPersecucion >= TiempoMaxPersecucion)
            {
                DejarDePerseguir();
                Persiguiendo = false;
            }
        }

        if (Persiguiendo)
        {
            tiempoPersecucion += Time.deltaTime;
            Anim.SetBool("Agonia", false);

            if (Vector3.Distance(transform.position, Jugador.transform.position) <= DistAtaque)
            {
                Anim.SetBool("Atacar", true);
                Anim.SetBool("Correr", false);
            }
            else
            {
                Anim.SetBool("Atacar", false);
                Anim.SetBool("Correr", true);
                Perseguir();
            }
            if (Microfono.ruidoTotal < SensibilidadSonido && Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin && tiempoPersecucion >= TiempoMaxPersecucion)
            {
                DejarDePerseguir();
                Persiguiendo = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, Jugador.transform.position) > DistMin)
            {
                DejarDePerseguir();
            }
        }

        if (visionJugador.cameraLooking && linterna._flashLightOn && Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin)
        {
            RecibirFlash();
        }
    }

    IEnumerator ActivarAlerta()
    {
        Alerta = true;
        Anim.SetBool("Alerta", true);
        Anim.SetBool("Agonia", false);

        Vector3 direccion = (Jugador.transform.position - transform.position).normalized;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
        float tiempoGiro = 0.5f;

        float t = 0;
        while (t < tiempoGiro)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, t / tiempoGiro);
            t += Time.deltaTime;
            yield return null;
        }
        transform.rotation = rotacionObjetivo;

        yield return new WaitForSeconds(0.6f);

        Persiguiendo = true;
        tiempoPersecucion = 0f;
        Anim.SetBool("Alerta", false);
        Anim.SetBool("Correr", true);
    }

    public void Perseguir()
    {
        IA.SetDestination(Jugador.transform.position);
        Anim.SetBool("Correr", true);
        Anim.SetBool("Agonia", false);
    }

    public void DejarDePerseguir()
    {
        IA.SetDestination(transform.position);
        Persiguiendo = false;
        Alerta = false;
        Anim.SetBool("Correr", false);
        Anim.SetBool("Alerta", false);
        Anim.SetBool("Agonia", true);
    }

    public void RecibirFlash()
    {
        aturdido = true;
        Persiguiendo = false;
        IA.ResetPath();
        Anim.SetTrigger("Flashazo");
        Anim.SetBool("Alerta", true);
        Anim.SetBool("Correr", false);
    }

    private IEnumerator RecuperarseDelFlash()
    {
        yield return new WaitForSeconds(tiempoEsperaFlash);
        aturdido = false;
        Anim.SetBool("Agonia", true);

        if (Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin)
        {
            StartCoroutine(ActivarAlerta());
        }
    }
}
