using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Central place for VR controller button bindings. Every interactable
/// script asks this instead of checking Input directly, so the whole
/// game's controls live in one spot. Add exactly ONE of these to your
/// scene (e.g. on the Player rig).
///
/// The Right Trigger prefers Gamepad.current.rightTrigger when available
/// (in case the device is ever recognized as a proper Gamepad), and
/// otherwise falls back to looking up a named raw control -- "rz" by
/// default, matching this controller's HID Joystick classification, found
/// via the Input Debugger.
///
/// Crouch stays on the legacy JoystickButton1 (B button) check, since
/// that's a separate system from the new Input System entirely (both run
/// side by side under "Both" Active Input Handling) and it's already
/// confirmed working.
/// </summary>
public class VRInputConfig : MonoBehaviour
{
    public static VRInputConfig Instance;

    [Header("Interact / Grab / Trigger (raw HID fallback)")]
    [Tooltip("Name of the raw control for the Right Trigger, as shown in the Input Debugger (e.g. 'rz'). Only used if the device isn't recognized as a full Gamepad. Re-check this on Android -- it's often consistent since it's a standardized HID usage name, but verify rather than assume.")]
    public string triggerControlName = "rz";

    [Tooltip("Axis value counted as 'pressed'. This controller rests at -1 and reads +1 when pulled, so 0 sits safely in between.")]
    public float triggerThreshold = 0f;

    [Header("Movement")]
    [Tooltip("Button used to toggle crouch on the VR controller (B button on most gamepads). Unchanged -- legacy system, already confirmed working.")]
    public KeyCode crouchButton = KeyCode.JoystickButton1;

    // Cached once per frame, refreshed lazily by whichever script asks
    // first -- avoids depending on Script Execution Order.
    private bool cachedHeld;
    private bool heldLastFrame;
    private int lastRefreshedFrame = -1;

    // Cached device lookup so we don't scan every connected device every frame.
    private AxisControl cachedTriggerControl;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    AxisControl FindTriggerControl()
    {
        if (cachedTriggerControl != null && cachedTriggerControl.device.added)
            return cachedTriggerControl;

        foreach (var device in InputSystem.devices)
        {
            var found = device.TryGetChildControl(triggerControlName) as AxisControl;
            if (found != null)
            {
                cachedTriggerControl = found;
                return found;
            }
        }

        return null;
    }

    void RefreshIfNeeded()
    {
        if (Time.frameCount == lastRefreshedFrame) return;
        heldLastFrame = cachedHeld;

        bool triggerHeld;

        var pad = Gamepad.current;
        if (pad != null)
        {
            // Preferred path, in case this or another device is ever
            // recognized as a proper Gamepad.
            triggerHeld = pad.rightTrigger.isPressed;
        }
        else
        {
            // Fallback: raw named control on whatever device reports it
            // (this controller's Right Trigger, as "rz").
            var control = FindTriggerControl();
            triggerHeld = control != null && control.ReadValue() > triggerThreshold;
        }

        cachedHeld = triggerHeld || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0);
        lastRefreshedFrame = Time.frameCount;
    }

    // --- Interact / Grab (Right Trigger) ---

    public static bool InteractHeld()
    {
        if (Instance == null) return false;
        Instance.RefreshIfNeeded();
        return Instance.cachedHeld;
    }

    public static bool InteractPressed()
    {
        if (Instance == null) return false;
        Instance.RefreshIfNeeded();
        return Instance.cachedHeld && !Instance.heldLastFrame;
    }

    public static bool InteractReleased()
    {
        if (Instance == null) return false;
        Instance.RefreshIfNeeded();
        return !Instance.cachedHeld && Instance.heldLastFrame;
    }

    // --- Crouch (B Button, unchanged) ---

    public static bool CrouchPressed()
    {
        return Instance != null && Input.GetKeyDown(Instance.crouchButton);
    }
}