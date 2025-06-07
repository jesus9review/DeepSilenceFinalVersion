using UnityEngine;

public class MicrofonoManager : MonoBehaviour
{
    private AudioClip microfonoClip;
    private int sampleWindow = 128;
    private float ruidoMic;

    [Range(1f, 100f)]
    public float sensibilidad = 10f;

    [Header("Aumentos por acción del jugador")]
    public float ruidoCorrer = 0.3f;
    public float ruidoCaminar = 0.2f;
    public float ruidoAgachado = -0.1f;

    [Header("Valores actuales")]
    public float ruidoTotal = 0f;

    public FirstPersonController jugador;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microfonoClip = Microphone.Start(null, true, 1, 44100);
        }
        else
        {
            Debug.LogWarning("No se detectó micrófono");
        }
    }

    void Update()
    {
        ruidoMic = ObtenerNivelVolumen();
        float ruidoAcciones = 0f;

        if (jugador.isSprinting)
            ruidoAcciones += ruidoCorrer;
        else if (jugador.isWalking)
        {
            if (jugador.isCrouched)
                ruidoAcciones += ruidoAgachado;
            else
                ruidoAcciones += ruidoCaminar;
        }

        // Calculamos el total y lo limitamos entre 0 y 1
        ruidoTotal = Mathf.Clamp01(ruidoMic + ruidoAcciones);
    }

    float ObtenerNivelVolumen()
    {
        if (microfonoClip == null) return 0f;

        float[] data = new float[sampleWindow];
        int pos = Microphone.GetPosition(null) - sampleWindow;
        if (pos < 0) return 0f;

        microfonoClip.GetData(data, pos);

        float maxVol = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = Mathf.Abs(data[i]);
            if (wavePeak > maxVol)
                maxVol = wavePeak;
        }

        return Mathf.Clamp01(maxVol * sensibilidad);
    }
}
