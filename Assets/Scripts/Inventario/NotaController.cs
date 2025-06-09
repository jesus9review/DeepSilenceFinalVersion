using UnityEngine;

public class NotaController : MonoBehaviour
{
    public GameObject[] notasVisuales; // UI de cada nota
    private GameObject notaActualMostrada;
    private bool siNotaActivada = false;
    public bool siNotaActiva => siNotaActivada;

    private GameObject inventarioUI;
    private bool volverAlInventario = false;
    private bool mostrarPanelesExtraAlCerrar = false;

    public GameObject barraBateria;
    public GameObject barraRuido;

    void Update()
    {
        if (siNotaActivada && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarNota();
        }
    }

    public void MostrarNota(GameObject notaGO, bool desdeInventario = false)
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = notaGO;
        notaActualMostrada.SetActive(true);
        Time.timeScale = 0f;
        siNotaActivada = true;

        volverAlInventario = desdeInventario;
        mostrarPanelesExtraAlCerrar = !desdeInventario; // Solo mostrar si viene desde el juego

        // Oculta paneles extra siempre
        if (barraRuido != null) barraRuido.SetActive(false);
        if (barraBateria != null) barraBateria.SetActive(false);
    }

    public void CerrarNota()
    {
        if (notaActualMostrada != null)
            notaActualMostrada.SetActive(false);

        notaActualMostrada = null;
        Time.timeScale = 1f;
        siNotaActivada = false;

        // Mostrar paneles extra solo si la nota venía desde el juego
        if (mostrarPanelesExtraAlCerrar)
        {
            if (barraRuido != null) barraRuido.SetActive(true);
            if (barraBateria != null) barraBateria.SetActive(true);
        }

        if (volverAlInventario && inventarioUI != null)
        {
            inventarioUI.SetActive(true);
        }
    }

    public void SetInventarioReferencia(GameObject inventario)
    {
        inventarioUI = inventario;
    }
}
