using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Firing")]
    [SerializeField] private float fireRate = 12.5f;
    [SerializeField] private float fireRange = 100f;
    [SerializeField] private LayerMask hitMask = ~0; // default: everything

    [Header("References")]
    [SerializeField] private Recoil recoilScript;
    [SerializeField] private WeaponKick weaponKick;
    [SerializeField] private AudioSource fireAudio;
    [SerializeField] private Camera fireCamera;
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private ScreenShake screenShake;
    

    [Header("Decal Settings")]
    [SerializeField] private float decalOffset = 0.001f; // tiny offset to prevent z-fighting

    private PlayerControls controls;
    private bool isFiring;
    private float nextFireTime;

private void Awake()
{
    controls = new PlayerControls();
    controls.Player.Fire.performed += ctx =>
    {
        isFiring = true;
        if (recoilScript != null) recoilScript.SetFiring(true);
    };
    controls.Player.Fire.canceled += ctx =>
    {
        isFiring = false;
        if (recoilScript != null) recoilScript.SetFiring(false);
    };
}

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private void Fire()
    {
        if (recoilScript != null) recoilScript.RecoilFire();
        if (weaponKick != null) weaponKick.Kick();
        if (screenShake != null) screenShake.Shake();

        if (fireAudio != null)
        {
            fireAudio.pitch = Random.Range(0.85f, 1f);
            fireAudio.PlayOneShot(fireAudio.clip);
        }

        // Raycast and place decal
        if (fireCamera != null && decalPrefab != null)
        {
            Ray ray = new Ray(fireCamera.transform.position, fireCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hitMask))
            {
                Vector3 decalPos = hit.point + hit.normal * decalOffset;
                Quaternion decalRot = Quaternion.LookRotation(-hit.normal);

                Instantiate(decalPrefab, decalPos, decalRot);
                // No Destroy call — decals are permanent for the play session
            }
        }
    }
}