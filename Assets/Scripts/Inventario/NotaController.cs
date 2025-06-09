using UnityEngine;

public class NotaController : MonoBehaviour
{
    public GameObject[] notasVisuales; // UI de cada nota
    private GameObject notaActualMostrada;
    private bool siNotaActivada = false;
    public bool siNotaActiva => siNotaActivada;

    private GameObject inventarioUI; // Referencia al inventario
    private bool volverAlInventario = false; // Desde dónde se abrió

    public GameObject barraRuido;
    public GameObject barraBateria;

    void Update()
    {
        if (siNotaActivada && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarNota();
        }
    }

    // desdeInventario indica si se abrió desde el inventario
    public void MostrarNota(GameObject notaGO, bool desdeInventario = false)
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = notaGO;
        notaActualMostrada.SetActive(true);
        Time.timeScale = 0f;
        siNotaActivada = true;

        volverAlInventario = desdeInventario;

        barraBateria.SetActive(false);
        barraRuido.SetActive(false);
    }

    public void CerrarNota()
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = null;
        Time.timeScale = 1f;
        siNotaActivada = false;

        if (volverAlInventario && inventarioUI != null)
            inventarioUI.SetActive(true);

        barraBateria.SetActive(true);
        barraRuido.SetActive(true);
    }

    public void SetInventarioReferencia(GameObject inventario)
    {
        inventarioUI = inventario;
    }
}
