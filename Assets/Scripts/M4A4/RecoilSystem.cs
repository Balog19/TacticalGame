using UnityEngine;

public class RecoilSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gun;

    [Header("Visual Kick (gun model) — hipfire only")]
    [SerializeField] private Vector3 kickPositionAmount = new Vector3(0f, 0.01f, -0.05f);
    [SerializeField] private Vector3 kickRotationAmount = new Vector3(-8f, 2f, 1f);
    [SerializeField] private float positionReturnSpeed = 18f;
    [SerializeField] private float rotationReturnSpeed = 25f;
    [SerializeField] private float positionSnappiness = 12f;
    [SerializeField] private float rotationSnappiness = 14f;

    [Header("Camera Kick (aim impact)")]
    [SerializeField] private Vector2 cameraKickAmount = new Vector2(2f, 0.6f);
    [SerializeField] private Vector2 cameraKickAmountADS = new Vector2(3f, 1.2f);
    [SerializeField] private float cameraReturnSpeed = 8f;
    [SerializeField] private float cameraSnappiness = 10f;

    public bool isAiming = false;

    private Vector3 currentPosOffset;
    private Vector3 targetPosOffset;
    private Vector3 currentRotOffset;
    private Vector3 targetRotOffset;
    private Vector3 originalGunLocalPos;

    private Vector3 currentCamRot;
    private Vector3 targetCamRot;

    private void Start()
    {
        if (gun != null) originalGunLocalPos = gun.localPosition;
    }

    private void Update()
    {
        // Visual kick spring
        targetPosOffset = Vector3.Lerp(targetPosOffset, Vector3.zero, positionReturnSpeed * Time.deltaTime);
        currentPosOffset = Vector3.Lerp(currentPosOffset, targetPosOffset, positionSnappiness * Time.deltaTime);

        targetRotOffset = Vector3.Lerp(targetRotOffset, Vector3.zero, rotationReturnSpeed * Time.deltaTime);
        currentRotOffset = Vector3.Lerp(currentRotOffset, targetRotOffset, rotationSnappiness * Time.deltaTime);

        // Camera kick spring
        targetCamRot = Vector3.Lerp(targetCamRot, Vector3.zero, cameraReturnSpeed * Time.deltaTime);
        currentCamRot = Vector3.Lerp(currentCamRot, targetCamRot, cameraSnappiness * Time.deltaTime);

        // Apply visual kick to gun (only matters when not aiming, since it springs to zero anyway)
        if (gun != null && !isAiming)
        {
            gun.localPosition = originalGunLocalPos + currentPosOffset;
            gun.localRotation = Quaternion.Euler(currentRotOffset);
        }
    }

    public void Fire()
    {
        // Visual kick — hipfire only
        if (!isAiming)
        {
            targetPosOffset += new Vector3(
                Random.Range(-kickPositionAmount.x, kickPositionAmount.x),
                kickPositionAmount.y,
                kickPositionAmount.z
            );

            targetRotOffset += new Vector3(
                kickRotationAmount.x,
                Random.Range(-kickRotationAmount.y, kickRotationAmount.y),
                Random.Range(-kickRotationAmount.z, kickRotationAmount.z)
            );
        }

        // Camera kick — always, but stronger when aiming since there's no visual kick to sell the recoil
        Vector2 camKick = isAiming ? cameraKickAmountADS : cameraKickAmount;
        targetCamRot += new Vector3(
            -camKick.y,
            Random.Range(-camKick.x, camKick.x),
            0f
        );
    }

    public Vector3 GetCameraRecoilDelta()
    {
        return currentCamRot;
    }
}