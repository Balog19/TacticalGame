using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private RecoilProfile profile;
    [SerializeField] private MouseLook mouseLook;

    private int patternIndex = 0;
    private bool isFiring;

    private void Update()
    {
        if (profile == null || mouseLook == null) return;

        // Decay recoil offset on MouseLook when not firing
        if (!isFiring)
        {
            patternIndex = 0;
            mouseLook.DecayRecoil(profile.returnSpeed);
        }
    }

    public void RecoilFire()
    {
        if (profile == null || profile.recoilPattern == null || profile.recoilPattern.Length == 0) return;
        if (mouseLook == null) return;

        int idx = Mathf.Min(patternIndex, profile.recoilPattern.Length - 1);
        Vector2 step = profile.recoilPattern[idx];

        // Apply random variation as a percentage of the absolute value
        float variationFactor = 0.1f;
        float xVariation = Mathf.Abs(step.x) * variationFactor;
        float yVariation = Mathf.Abs(step.y) * variationFactor;

        step.x += Random.Range(-xVariation, xVariation);
        step.y += Random.Range(-yVariation, yVariation);

        mouseLook.AddRecoil(step.x, step.y);
        patternIndex++;
    }

    public void SetFiring(bool firing)
{
    isFiring = firing;
    if (mouseLook != null)
    {
        if (firing) mouseLook.StartFiring();
        else mouseLook.StopFiring();
    }
}
    public void SetProfile(RecoilProfile newProfile)
    {
        profile = newProfile;
        patternIndex = 0;
        mouseLook?.ResetRecoil();
    }
}