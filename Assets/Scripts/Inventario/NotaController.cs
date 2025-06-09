using UnityEngine;

public class NotaController : MonoBehaviour
{
    public GameObject[] notasVisuales; // UI de cada nota
    private GameObject notaActualMostrada;
    private bool siNotaActivada = false;
    public bool siNotaActiva => siNotaActivada;

    void Update()
    {
        if (siNotaActivada && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarNota();
        }
    }

    public void MostrarNota(GameObject notaGO)
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = notaGO;
        notaActualMostrada.SetActive(true);
        Time.timeScale = 0f;
        siNotaActivada = true;
    }

    public void CerrarNota()
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = null;
        Time.timeScale = 1f;
        siNotaActivada = false;
    }
}
