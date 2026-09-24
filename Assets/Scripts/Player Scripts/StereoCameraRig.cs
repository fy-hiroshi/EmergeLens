using UnityEngine;

/// <summary>
/// Configures two cameras as a simple side-by-side stereo pair for a
/// "cardboard box" style VR view, letterboxed/pillarboxed to a fixed
/// target aspect ratio regardless of the device's actual screen shape.
/// Attach this to a parent "VRRig" object and assign the two child eye
/// cameras in the inspector.
/// </summary>
[ExecuteAlways]
public class StereoCameraRig : MonoBehaviour
{
    [Header("Eye Cameras")]
    public Camera leftEyeCamera;
    public Camera rightEyeCamera;

    [Header("Stereo Settings")]
    [Tooltip("Distance between the two eyes in world units. Average human IPD is about 0.063m.")]
    public float eyeSeparation = 0.064f;

    [Tooltip("Field of view shared by both eyes.")]
    public float fieldOfView = 90f;

    [Header("Target Aspect Ratio")]
    [Tooltip("The width/height ratio you want the whole game to render at, e.g. 16/9. If the device's actual screen is a different shape, black bars fill the rest.")]
    public float targetAspectWidth = 16f;
    public float targetAspectHeight = 9f;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;

    void OnEnable() => ConfigureCameras();
    void OnValidate() => ConfigureCameras();

    void Update()
    {
        // Recompute if the screen size/orientation changes at runtime
        // (rotation, split-screen multitasking, different device, etc).
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ConfigureCameras();
        }
    }

    void ConfigureCameras()
    {
        if (leftEyeCamera == null || rightEyeCamera == null) return;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        Rect safeArea = ComputeLetterboxedRect();

        // Split the safe area in half: left eye / right eye.
        leftEyeCamera.rect = new Rect(
            safeArea.x, safeArea.y, safeArea.width * 0.5f, safeArea.height);
        rightEyeCamera.rect = new Rect(
            safeArea.x + safeArea.width * 0.5f, safeArea.y, safeArea.width * 0.5f, safeArea.height);

        // Keep both eyes optically identical except for their X offset.
        leftEyeCamera.fieldOfView  = fieldOfView;
        rightEyeCamera.fieldOfView = fieldOfView;
        rightEyeCamera.nearClipPlane    = leftEyeCamera.nearClipPlane;
        rightEyeCamera.farClipPlane     = leftEyeCamera.farClipPlane;
        rightEyeCamera.clearFlags       = leftEyeCamera.clearFlags;
        rightEyeCamera.backgroundColor  = leftEyeCamera.backgroundColor;
        rightEyeCamera.cullingMask      = leftEyeCamera.cullingMask;

        // Symmetric offset along local X for the eye separation.
        Vector3 l = leftEyeCamera.transform.localPosition;
        Vector3 r = rightEyeCamera.transform.localPosition;
        l.x = -eyeSeparation * 0.5f;
        r.x =  eyeSeparation * 0.5f;
        leftEyeCamera.transform.localPosition  = l;
        rightEyeCamera.transform.localPosition = r;

        // Neither camera should be treated as an XR eye target.
        leftEyeCamera.stereoTargetEye  = StereoTargetEyeMask.None;
        rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.None;
    }

    /// <summary>
    /// Returns a normalized (0-1) Rect, centered on screen, that has the
    /// requested target aspect ratio -- pillarboxed (bars on the sides) if
    /// the screen is proportionally wider than the target, letterboxed
    /// (bars top/bottom) if the screen is proportionally taller/narrower.
    /// </summary>
    Rect ComputeLetterboxedRect()
    {
        float targetAspect = targetAspectWidth / targetAspectHeight;
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > targetAspect)
        {
            // Screen is wider than target -> bars on left/right.
            float widthScale = targetAspect / screenAspect;
            return new Rect((1f - widthScale) * 0.5f, 0f, widthScale, 1f);
        }
        else if (screenAspect < targetAspect)
        {
            // Screen is narrower/taller than target -> bars on top/bottom.
            float heightScale = screenAspect / targetAspect;
            return new Rect(0f, (1f - heightScale) * 0.5f, 1f, heightScale);
        }

        return new Rect(0f, 0f, 1f, 1f);
    }
}