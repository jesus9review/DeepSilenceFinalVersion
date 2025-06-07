using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseDeDatosJugador : MonoBehaviour
{
    public string[] slotsBasicos;

    public bool TieneObjeto(string nombreObjeto)
    {
        foreach (string item in slotsBasicos)
        {
            if (item == nombreObjeto)
            {
                return true;
            }
        }
        return false;
    }

    public void EliminarObjeto(string nombreObjeto)
    {
        for (int i = 0; i < slotsBasicos.Length; i++)
        {
            if (slotsBasicos[i] == nombreObjeto)
            {
                slotsBasicos[i] = ""; // Marca como vacío
                break;
            }
        }
    }
}
