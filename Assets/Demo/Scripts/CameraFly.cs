using UnityEngine;

namespace Portals.Demo
{
    public class CameraFly : MonoBehaviour
    {
        [SerializeField] private float Sensitivity = 8;
        private Camera Camera;
        private float xEulerRotation;
        private float yEulerRotation;

        private float CurrentSpeed = 0.05f;
        private float Speed = 0.05f;
        private Vector3 MoveDirection;
        private Vector3 MoveDirectionRaw;

        private void Awake()
        {
            Camera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            xEulerRotation -= Input.GetAxisRaw("Mouse Y") * Sensitivity;
            xEulerRotation = Mathf.Clamp(xEulerRotation, -90, 90);
            yEulerRotation += Input.GetAxisRaw("Mouse X") * Sensitivity;
            transform.localRotation = Quaternion.Euler(xEulerRotation, yEulerRotation, 0);

            MoveDirectionRaw =
                (Input.GetAxisRaw("Horizontal") * transform.right) +
                (Input.GetAxisRaw("Vertical") * transform.forward) +
                (Input.GetAxisRaw("HeightControl") * transform.up);

            MoveDirection =
                Vector3.Lerp(
                    MoveDirection,
                    MoveDirectionRaw,
                    0.1f);
            transform.position += MoveDirection * CurrentSpeed;

            CurrentSpeed = Mathf.Lerp(CurrentSpeed, Speed, 0.1f);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Speed = 0.5f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Speed = 0.05f;
            }
        }
    }
}
