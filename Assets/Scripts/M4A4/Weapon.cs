using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private RecoilSystem recoilSystem;
    [SerializeField] private float fireRate = 10f; // shots per second (full-auto)

    private PlayerControls controls;
    private bool isFiring;
    private float nextFireTime;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Fire.performed += ctx => isFiring = true;
        controls.Player.Fire.canceled += ctx => isFiring = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            recoilSystem.Fire();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }
}