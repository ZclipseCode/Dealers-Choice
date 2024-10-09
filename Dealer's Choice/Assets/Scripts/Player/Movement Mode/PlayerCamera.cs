using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] float xSensitivity = 0.1f;
    [SerializeField] float ySensitivity = 0.1f;
    float xRotation;
    float yRotation;
    PlayerControls playerControls;
    bool inputDisabled;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!inputDisabled)
        {
            float mouseX = playerControls.Player.LookX.ReadValue<float>() * xSensitivity;
            float mouseY = playerControls.Player.LookY.ReadValue<float>() * ySensitivity;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    public void EnableInput()
    {
        xRotation = transform.rotation.eulerAngles.x;
        yRotation = transform.rotation.eulerAngles.y;
        inputDisabled = false;
    }

    public void DisableInput()
    {
        inputDisabled = true;
    }

    private void OnDestroy()
    {
        playerControls.Player.Disable();
    }
}
