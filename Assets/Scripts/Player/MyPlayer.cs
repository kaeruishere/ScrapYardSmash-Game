using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Walkthrough.SimpleJumping
{
    public class MyPlayer : MonoBehaviour
    {
        public ExampleCharacterCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public MyCharacterController Character;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            OrbitCamera.SetFollowTransform(CameraFollowPoint);
            OrbitCamera.IgnoredColliders.Clear();
            OrbitCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        void Update() { if (Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked; HandleCharacterInput(); }
        void LateUpdate() { HandleCameraInput(); }

        private void HandleCameraInput()
        {
            Vector2 look = GameInputManager.Instance.LookInput;
            Vector3 lookVec = (Cursor.lockState == CursorLockMode.Locked) ? new Vector3(look.x, look.y, 0f) : Vector3.zero;
            OrbitCamera.UpdateWithInput(Time.deltaTime, GameInputManager.Instance.ZoomInput, lookVec);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs inputs = new PlayerCharacterInputs();
            inputs.MoveAxisForward = GameInputManager.Instance.MoveInput.y;
            inputs.MoveAxisRight = GameInputManager.Instance.MoveInput.x;
            inputs.CameraRotation = OrbitCamera.Transform.rotation;
            inputs.JumpDown = GameInputManager.Instance.JumpTriggered;
            Character.SetInputs(ref inputs);
        }
    }
}