using Generated;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private PlayerControls _input;
    private void Awake() => _input = new PlayerControls();
    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
    private void Start()
    {
        _camera = Camera.main;
        _input.Interactions.Tap.started += OnTap;
    }
    private void OnTap(InputAction.CallbackContext callbackContext)
    {
        var tapPosition = _input.Interactions.Position.ReadValue<Vector2>();
        Debug.Log($"tap position: {tapPosition}");
        DetectInteractable(tapPosition);
    }
    private void DetectInteractable(Vector2 screenPosition)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out var hit, 1000f))
        {
            Debug.Log("Hit!");
            // TODO: later add code complexity to enable flexibility
            // Probably check for a component that impl some sort of interface
            var maybeRocket = hit.transform.GetComponent<Rocket>();
            if (maybeRocket != null) maybeRocket.ToggleEngine();
        }
    }
}