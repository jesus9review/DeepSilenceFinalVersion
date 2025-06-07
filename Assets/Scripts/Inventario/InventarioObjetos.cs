using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventarioObjetos : MonoBehaviour
{

    public GameObject contenedorInventario;
    private NombresObjetos objetosDisponibles;
    private BaseDeDatosJugador objetosObtenidos;

    private Transform panelObjetos;
    private Transform panelNotas;

    public TextMeshProUGUI textoObjetos;

    //IconosObjetos
    public GameObject objetoInventario;
    

    void Awake()
    {
        panelObjetos = transform.GetChild(0);
        panelNotas = transform.GetChild(1);
    }

    // Start is called before the first frame update
    void OnEnable()
    {

        objetosDisponibles = contenedorInventario.GetComponent<NombresObjetos>();
        objetosObtenidos = contenedorInventario.GetComponent<BaseDeDatosJugador>();

        
        for (int i = panelObjetos.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(panelObjetos.GetChild(i).gameObject);
        }

        for (int i = panelNotas.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(panelNotas.GetChild(i).gameObject);
        }


        //Notas
        for (int i = 0; i < objetosDisponibles.ItemsString.Length; i++)
        {
            string item = objetosDisponibles.ItemsString[i];


            int j = 0;
            while (j < objetosObtenidos.slotsBasicos.Length)
            {
                if (item == objetosObtenidos.slotsBasicos[j])
                {
                    GameObject objeto;
                    Sprite icono;

                    if (item.Substring(0, 4) == "nota")
                    {
                        objeto = Instantiate(objetoInventario, panelNotas);
                        icono = Resources.Load<Sprite>("IconosInventario/nota");
                        
                    }
                    else
                    {
                        objeto = Instantiate(objetoInventario, panelObjetos);
                        icono = Resources.Load<Sprite>("IconosInventario/" + item);
                    }

                    string itemDescripcion = objetosDisponibles.ItemsDescripcion[j];

                    objeto.transform.GetChild(0).GetComponent<Image>().sprite = icono;
                    objeto.transform.GetComponent<InfoObjeto>().info = itemDescripcion;
                    

                    break;
                }
                j++;
            }

        }

        //Objetos
        /* for (int i = 0; i < objetosDisponibles.ItemsString.Length; i++)
         {
             string item = objetosDisponibles.ItemsString[i];
             if (item.Substring(0, 4) != "nota")
             {
                 int j = 0;
                 while (j < objetosObtenidos.slotsBasicos.Length)
                 {
                     if (item == objetosObtenidos.slotsBasicos[j])
                     {
                         //Instantiate()
                         print("Tengo este objeto " + objetosObtenidos.slotsBasicos[j]);
                         break;
                     }
                     j++;
                 }
             }     
         }*/
    }

}
