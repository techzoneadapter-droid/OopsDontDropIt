using UnityEngine;
using UnityEngine.InputSystem;

public class TrayController : MonoBehaviour
{
    [SerializeField] private float maxRoll = 24f;
    [SerializeField] private float maxPitch = 16f;
    [SerializeField] private float sensitivity = 2.2f;
    [SerializeField] private float returnSpeed = 6f;
    [SerializeField] private float followSpeed = 10f;

    private bool dragging;
    private Vector2 dragStart;
    private Quaternion targetRotation = Quaternion.identity;

    private void Update()
    {
        if (GameStateController.Instance == null || !GameStateController.Instance.IsRunning)
        {
            targetRotation = Quaternion.identity;
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * returnSpeed);
            return;
        }

        bool pressed;
        bool pressedThisFrame;
        Vector2 pointer = ReadPointer(out pressed, out pressedThisFrame);

        if (pressedThisFrame)
        {
            dragging = true;
            dragStart = pointer;
        }

        if (dragging && pressed)
        {
            Vector2 delta = pointer - dragStart;
            float nx = Mathf.Clamp(delta.x / Mathf.Max(1f, Screen.width) * sensitivity, -1f, 1f);
            float ny = Mathf.Clamp(delta.y / Mathf.Max(1f, Screen.height) * sensitivity, -1f, 1f);

            float roll = -nx * maxRoll;
            float pitch = ny * maxPitch;
            targetRotation = Quaternion.Euler(pitch, 0f, roll);
        }
        else
        {
            dragging = false;

            float keyboardRoll = 0f;
            float keyboardPitch = 0f;
            if (Keyboard.current != null)
            {
                keyboardRoll = (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f)
                             - (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f);
                keyboardPitch = (Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                              - (Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
            }

            targetRotation = Quaternion.Euler(
                keyboardPitch * maxPitch,
                0f,
                keyboardRoll * maxRoll);
        }

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            1f - Mathf.Exp(-followSpeed * Time.deltaTime));
    }

    private static Vector2 ReadPointer(out bool pressed, out bool pressedThisFrame)
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            pressed = true;
            pressedThisFrame = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            pressed = Mouse.current.leftButton.isPressed;
            pressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
            return Mouse.current.position.ReadValue();
        }

        pressed = false;
        pressedThisFrame = false;
        return Vector2.zero;
    }
}
