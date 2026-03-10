using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    [Header("Durum")]
    public bool inputLocked = false; 

    // Veriler
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float ZoomInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool ReloadTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool ThrowTriggered { get; private set; }
    public bool FireHeld { get; private set; }
    public bool FireTriggered { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (inputLocked) { ClearInputs(); return; }

        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        LookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        #if UNITY_WEBGL
        ZoomInput = 0f;
        #else
        ZoomInput = -Input.GetAxis("Mouse ScrollWheel");
        #endif

        JumpTriggered = Input.GetKeyDown(KeyCode.Space);
        ReloadTriggered = Input.GetKeyDown(KeyCode.R);
        InteractTriggered = Input.GetKeyDown(KeyCode.E);
        ThrowTriggered = Input.GetMouseButtonDown(1);
        FireHeld = Input.GetButton("Fire1");
        FireTriggered = Input.GetButtonDown("Fire1");
    }

    private void ClearInputs()
    {
        MoveInput = Vector2.zero; LookInput = Vector2.zero; ZoomInput = 0f;
        JumpTriggered = false; ReloadTriggered = false; InteractTriggered = false;
        ThrowTriggered = false; FireHeld = false; FireTriggered = false;
    }
}