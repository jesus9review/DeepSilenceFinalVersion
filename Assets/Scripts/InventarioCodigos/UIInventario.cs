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
            // Si el juego no está pausado, abre el inventario
            if (!juegoPausado)
            {
                AbrirInventario();
                ruido.SetActive(false);
                bateria.SetActive(false);
            }
            else
            {
                CerrarInventario();
                ruido.SetActive(true);
                bateria.SetActive(true);
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
    }

    public void CerrarInventario()
    {
        panelInventario.SetActive(false);
        jugador.cameraCanMove = true;
        Time.timeScale = 1f;
        juegoPausado = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
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
