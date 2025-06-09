using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventario : MonoBehaviour
{
    [SerializeField]
    private GameObject panelInventario;
    //public GameObject cameraController;

    public bool juegoPausado = false;
    public FirstPersonController jugador;
    public NotaController notaController;

    public GameObject PanelNotas;
    public GameObject PanelObjetos;

    public GameObject bateria;
    public GameObject ruido;


    private void Start()
    {
        bateria.SetActive(true);
        ruido.SetActive(true);
        panelInventario.SetActive(false);
        PanelObjetos.SetActive(true);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (notaController != null && notaController.siNotaActiva)
                return;

            // Si el juego no está pausado, abre el inventario
            if (!juegoPausado)
            {
                AbrirInventario();
                
            }
            else
            {
                CerrarInventario();
                
            }
        }
    }



    public void AbrirInventario()
    {
        panelInventario.SetActive(true);
        jugador.cameraCanMove = false;
        Time.timeScale = 0f;
        juegoPausado = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ruido.SetActive(false);
        bateria.SetActive(false);
    }

    public void CerrarInventario()
    {
        panelInventario.SetActive(false);
        jugador.cameraCanMove = true;
        Time.timeScale = 1f;
        juegoPausado = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        ruido.SetActive(true);
        bateria.SetActive(true);
    }

    public void ActivarPanelNotas()
    {
        PanelNotas.SetActive(true);
        PanelObjetos.SetActive(false);
    }
    public void ActivarPanelObjetos()
    {
        PanelNotas.SetActive(false);
        PanelObjetos.SetActive(true);
    }
}
