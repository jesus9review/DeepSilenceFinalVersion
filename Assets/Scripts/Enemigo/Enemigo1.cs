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

    [Header("Sonidos del Enemigo")]
    public AudioSource audioSourceLoop;
    public AudioSource audioSourceSFX;
    public AudioClip sonidoCorrer;
    public AudioClip sonidoAtacar;
    public AudioClip sonidoAgonia;
    public AudioClip sonidoAlerta;
    public AudioClip sonidoFlashazo;
    public AudioClip sonidoAsesinato;

    [Header("Percepción Visual")]
    public float campoVision = 60f;
    public float rangoVision = 20f;
    public LayerMask capasObstaculos;
    private float tiempoSinVerJugador = 0f;
    public float tiempoMaxSinVer = 4f;


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

    public int danoHecho { get; private set; }

    void Start()
    {
        Anim = GetComponent<Animator>();
        Jugador = GameObject.Find("Jugador");
        posicionInicial = transform.position;
        PlayLoop(sonidoAgonia, 0.8f);
    }

    void Update()
    {
        Vector3 posicionActual = transform.position;

        if (!aturdido && !datosJugador.estamuerto)
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
            if (!Alerta && !datosJugador.estamuerto)
            {
                StartCoroutine(ActivarAlerta());
            }

            if (Microfono.ruidoTotal < SensibilidadSonido &&
                Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin &&
                tiempoPersecucion >= TiempoMaxPersecucion)
            {
                DejarDePerseguir();
                Persiguiendo = false;
            }
        }

        if (Persiguiendo)
        {
            tiempoPersecucion += Time.deltaTime;

            if (PuedeVerAlJugador())
            {
                tiempoSinVerJugador = 0f;
                Perseguir();
                if (Vector3.Distance(transform.position, Jugador.transform.position) <= DistAtaque && PuedeVerAlJugador())
                {
                    Anim.SetBool("Atacar", true);
                    Anim.SetBool("Correr", false);
                }
                else
                {
                    Anim.SetBool("Atacar", false);
                    Anim.SetBool("Correr", true);
                }
            }
            else
            {
                tiempoSinVerJugador += Time.deltaTime;

                if (tiempoSinVerJugador >= tiempoMaxSinVer)
                {
                    DejarDePerseguir();
                }
            }
        }


        if (visionJugador.cameraLooking && linterna._flashLightOn && Vector3.Distance(transform.position, Jugador.transform.position) <= DistMin)
        {
            StartCoroutine(RecibirFlash());
        }
    }

    IEnumerator ActivarAlerta()
    {
        PlaySFX(sonidoAlerta, 0.9f);
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
        PlayLoop(sonidoCorrer, 0.2f);
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

        PlayLoop(sonidoAgonia, 0.8f);
    }

    public void StopLoopingSound()
    {
        if (audioSourceLoop != null)
        {
            audioSourceLoop.Stop();
            audioSourceLoop.clip = null;
            audioSourceLoop.loop = false;
        }
    }
    void PlayLoop(AudioClip clip, float volumen = 1f)
    {
        if (clip == null || audioSourceLoop == null) return;

        if (audioSourceLoop.clip != clip && !datosJugador.estamuerto)
        {
            audioSourceLoop.Stop();
            audioSourceLoop.clip = clip;
            audioSourceLoop.volume = volumen;
            audioSourceLoop.loop = true;
            audioSourceLoop.Play();
        }
    }


    void PlaySFX(AudioClip clip, float volumen = 1f)
    {
        if (clip == null || audioSourceSFX == null) return;

        audioSourceSFX.PlayOneShot(clip, volumen);
    }

    bool PuedeVerAlJugador()
    {
        Vector3 direccionJugador = (Jugador.transform.position - transform.position).normalized;
        float angulo = Vector3.Angle(transform.forward, direccionJugador);

        if (angulo < campoVision / 2f && Vector3.Distance(transform.position, Jugador.transform.position) <= rangoVision)
        {
            Vector3 origen = transform.position + Vector3.up * 1.6f; // Altura de la cabeza
            Vector3 destino = Jugador.transform.position + Vector3.up * 1.6f;

            if (!Physics.Linecast(origen, destino, capasObstaculos))
            {
                return true;
            }
        }

        return false;
    }


    void HacerDano()
    {
        datosJugador.RecibirDanio();
        danoHecho++;

        if (danoHecho == 3)
        {
            StopLoopingSound();
            audioSourceSFX.PlayOneShot(sonidoAsesinato,2f);
        }
        else
        {
            PlaySFX(sonidoAtacar, 0.9f);
        }
    }

    private IEnumerator RecibirFlash()
    {
        aturdido = true;
        Persiguiendo = false;
        IA.ResetPath();
        Anim.SetTrigger("Flashazo");
        Anim.SetBool("Alerta", true);
        Anim.SetBool("Correr", false);
        StopLoopingSound();
        PlaySFX(sonidoFlashazo, 1f);
        yield return new WaitForSeconds(3f);
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
