using UnityEngine;

public class BatteryPowerPickup : MonoBehaviour
{
    public float PowerIntensityLight;
    private ElectricTorchOnOff _torchOnOff;
    public AudioSource audiosource;
    public AudioClip sonidoRecogerBatt;

    private void Start()
    {
        _torchOnOff = FindObjectOfType<ElectricTorchOnOff>();
    }

    public void PickUp(ElectricTorchOnOff torch)
    {
        audiosource.PlayOneShot(sonidoRecogerBatt);
        torch._PowerPickUp = true;
        torch.intensityLight = PowerIntensityLight;
        _torchOnOff._PowerPickUp = false;
        Destroy(gameObject);
    }
}
