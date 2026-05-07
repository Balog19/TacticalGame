using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float positionAmount = 0.01f;   // max position jitter
    [SerializeField] private float rotationAmount = 0.4f;     // max rotation jitter (degrees)
    [SerializeField] private float decaySpeed = 8f;           // how fast shake fades

    private float currentShake;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        if (currentShake > 0.001f)
        {
            Vector3 posJitter = Random.insideUnitSphere * positionAmount * currentShake;
            Vector3 rotJitter = Random.insideUnitSphere * rotationAmount * currentShake;

            transform.localPosition = originalLocalPos + posJitter;
            transform.localRotation = originalLocalRot * Quaternion.Euler(rotJitter);

            currentShake = Mathf.Lerp(currentShake, 0f, decaySpeed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            currentShake = 0f;
        }
    }

    public void Shake(float intensity = 1f)
    {
        // Use the larger of current and new — rapid shots refresh shake instead of stacking forever
        currentShake = Mathf.Max(currentShake, intensity);
    }
}