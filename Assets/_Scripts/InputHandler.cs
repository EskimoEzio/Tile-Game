using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    // this makes the input handler a singlton, thi means that any script can access this class but there can only be one instance (which is fine because i don't need more than 1)
    public static InputHandler Instance;

    private PlayerControls input;

    // creating a dedicated custom event makes it easier to subscribe methods to the event. the vector2 paramenter allows the mouse position to be passed into the functions, e.g. helpfull for knowing where the mouse is when it was clicked
    public delegate void InputEvent(Vector2 position);
    public event InputEvent OnMouseDown;
    public event InputEvent OnMouseUp;
    public event InputEvent OnMouseMove;


    public Vector2 PointerPosition { get; private set; }


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize input
        input = new PlayerControls();

    }

    private void OnEnable()
    {
        
        input.Enable();

        //This is subscribing my custom events ( will brodcast out) to the associated input
        input.Player.Click.performed += OnClickPerformed;
        input.Player.Click.canceled += OnClickCanceled;
        input.Player.Position.performed += OnPositionPerformed;
    }

    private void OnDisable()
    {
        //This is unsubscribing my custom events ( will brodcast out) to the associated input
        input.Player.Click.performed -= OnClickPerformed;
        input.Player.Click.canceled -= OnClickCanceled;
        input.Player.Position.performed -= OnPositionPerformed;

        input.Disable();
    }


    // These functions are activated on a given input (their name), they are used as an always accessible broadcaster of an input event
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // this means, if anything is subscribed to the "OnMouseDown" event, call their method with the parameter pointerposition
        OnMouseDown?.Invoke(PointerPosition);
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        OnMouseUp?.Invoke(PointerPosition);
    }

    private void OnPositionPerformed(InputAction.CallbackContext context)
    {
        PointerPosition = context.ReadValue<Vector2>();
        OnMouseMove?.Invoke(PointerPosition);
    }




}
