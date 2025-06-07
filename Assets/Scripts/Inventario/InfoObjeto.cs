using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoObjeto : MonoBehaviour
{
    public string info;
    
    private void Awake()
    {
        
    }

    public void CambiarTexto()
    {
        transform.parent.parent.GetComponent<InventarioObjetos>().textoObjetos.GetComponent<TextMeshProUGUI>().text = info;
    }
    
}
