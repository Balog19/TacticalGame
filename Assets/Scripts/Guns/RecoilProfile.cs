using UnityEngine;

[CreateAssetMenu(fileName = "New Recoil Profile", menuName = "Weapons/Recoil Profile")]
public class RecoilProfile : ScriptableObject
{
    [Header("Pattern (per-bullet pitch and yaw)")]
    public Vector2[] recoilPattern;

    [Header("Settings")]
    public float patternScale;
    public float snappiness;
    public float returnSpeed;
    public float fireStopDelay;
}