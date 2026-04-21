using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private bool isDragging = false;
    public float dragSpeed = 0.5f;

    public Camera cam;
    public float zoomSpeed;
    void Awake() => inputActions = new InputSystem_Actions();

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.MiddleClick.performed += _ => isDragging = true;
        inputActions.Player.MiddleClick.canceled += _ => isDragging = false;
    }

    void Update()
    {
        float scrollDelta = inputActions.Player.Zoom.ReadValue<Vector2>().y;

        if (scrollDelta != 0)
        {
            float targetSize = cam.orthographicSize - (scrollDelta * zoomSpeed * Time.deltaTime);
            cam.orthographicSize = Mathf.Clamp(targetSize, 1f, 15f);

        }
        if (isDragging)
        {
            Vector2 mouseDelta = inputActions.Player.CameraMovement.ReadValue<Vector2>();

            Vector3 move = cam.orthographicSize*dragSpeed * Time.deltaTime * new Vector3(-mouseDelta.x, -mouseDelta.y, 0);
            transform.Translate(move, Space.World);
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

}
