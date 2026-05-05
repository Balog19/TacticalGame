using UnityEngine;

public class MatchYawToCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;

    private void LateUpdate()
    {
        if (cameraRig == null) return;

        // Take only the Y rotation from the camera rig
        float yaw = cameraRig.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}