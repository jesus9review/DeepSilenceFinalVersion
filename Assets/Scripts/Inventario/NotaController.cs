using UnityEngine;

public class NotaController : MonoBehaviour
{
    public GameObject notaUIPanel;
    private bool siNotaActivada = false;
    public bool siNotaActiva => siNotaActivada;


    void Update()
    {
        if (siNotaActivada && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarNota();
        }
    }

    public void MostrarNota()
    {
        notaUIPanel.SetActive(true);
        Time.timeScale = 0f;
        siNotaActivada = true;
    }

    public void CerrarNota()
    {
        notaUIPanel.SetActive(false);
        Time.timeScale = 1f;
        siNotaActivada = false;
    }
}
