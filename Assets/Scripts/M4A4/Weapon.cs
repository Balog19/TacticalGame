using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float fireRate = 10f; // shots per second (full-auto)

    private PlayerControls controls;
    private bool isFiring;
    private float nextFireTime;
    [SerializeField] private Recoil recoilScript;

    [SerializeField] private AudioSource fireAudio;
    [SerializeField] private WeaponKick weaponKick;

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
            Fire();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    private void Fire()
    {
        // Placeholder — add raycast, muzzle flash, sound, etc. here later
        recoilScript.RecoilFire();
            if (weaponKick != null)
        weaponKick.Kick();

        if (fireAudio != null)
        {
            fireAudio.pitch = Random.Range(0.85f, 1f); // ±5% pitch variation
            fireAudio.PlayOneShot(fireAudio.clip);
        }
    }
}