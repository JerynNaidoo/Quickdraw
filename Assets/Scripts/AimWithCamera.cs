using UnityEngine;

public class AimWithCamera : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;          // FP camera
    public Transform upperSpine;               // mixamorig:Spine2 (or Spine1)
    public Transform rightShoulder;            // optional, for finer aim control
    public float rotationIntensity = 1f;       // how much the body follows pitch
    public float maxPitch = 30f;               // limit to avoid twisting

    private Quaternion baseSpineRotation;
    private Quaternion baseShoulderRotation;

    void Start()
    {
        if (upperSpine != null) baseSpineRotation = upperSpine.localRotation;
        if (rightShoulder != null) baseShoulderRotation = rightShoulder.localRotation;
    }

    void LateUpdate()
    {
        if (cameraTransform == null || upperSpine == null) return;

        // Get camera pitch (x-axis)
        float pitch = cameraTransform.localEulerAngles.x;
        if (pitch > 180) pitch -= 360; // convert from 0–360 to -180–180

        // Clamp pitch range
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // Apply pitch to upper spine
        Quaternion targetRot = baseSpineRotation * Quaternion.Euler(pitch * rotationIntensity, 0f, 0f);
        upperSpine.localRotation = targetRot;

        // Optional: add a smaller rotation to shoulder for better alignment
        if (rightShoulder != null)
        {
            Quaternion shoulderRot = baseShoulderRotation * Quaternion.Euler(pitch * rotationIntensity * 0.5f, 0f, 0f);
            rightShoulder.localRotation = shoulderRot;
        }
    }
}

//---------------------------------------------------------------------------------------------

