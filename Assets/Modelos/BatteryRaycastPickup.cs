using UnityEngine;

public class BatteryRaycastPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.F;
    private ElectricTorchOnOff _torch;

    void Start()
    {
        _torch = FindObjectOfType<ElectricTorchOnOff>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                BatteryPowerPickup battery = hit.collider.GetComponent<BatteryPowerPickup>();
                if (battery != null)
                {
                    battery.PickUp(_torch);
                }
            }
        }
    }
}
