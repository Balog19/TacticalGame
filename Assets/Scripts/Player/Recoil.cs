using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Per Shot")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [Header("Settings")]
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        Vector3 kick = new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        targetRotation += kick;
    }
}