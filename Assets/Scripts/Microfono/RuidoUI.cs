using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicrofonoUI : MonoBehaviour
{
    public MicrofonoManager microfono;
    public Slider barraRuido;
    public Slider barraBateria;
    public ElectricTorchOnOff linternaBateria;
    public TextMeshProUGUI textoPorcentaje;

    void Update()
    {
        float ruido = microfono.ruidoTotal;
        barraRuido.value = ruido;
        textoPorcentaje.text = Mathf.RoundToInt(ruido * 100f) + "%";

        float restantebateria = linternaBateria.intensityLight;
        barraBateria.value = restantebateria;
    }
}
