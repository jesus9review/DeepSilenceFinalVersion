using UnityEngine;

public class BatteryPowerPickup : MonoBehaviour
{
    public float PowerIntensityLight;
    private ElectricTorchOnOff _torchOnOff;

    private void Start()
    {
        _torchOnOff = FindObjectOfType<ElectricTorchOnOff>();
    }

    public void PickUp(ElectricTorchOnOff torch)
    {
        torch._PowerPickUp = true;
        torch.intensityLight = PowerIntensityLight;
        _torchOnOff._PowerPickUp = false;
        Destroy(gameObject);
    }
}
