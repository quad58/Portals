using UnityEngine;

namespace Portals.Demo
{
    [DefaultExecutionOrder(90)]
    public class CameraFly : MonoBehaviour
    {
        [SerializeField] private float Sensitivity = 8;

        private Camera Camera;
        private float DefaultFiledOfView;
        private float xEulerRotation;
        private float yEulerRotation;

        private float CurrentSpeed = 0.05f;
        private float Speed = 0.05f;
        private Vector3 MoveDirection;
        private Vector3 MoveDirectionRaw;

        private bool CursorUnlocked = false;

        private void Awake()
        {
            Camera = Camera.main;
            DefaultFiledOfView = Camera.fieldOfView;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (CursorUnlocked == false)
            {
                xEulerRotation -= Input.GetAxisRaw("Mouse Y") * Sensitivity;
                xEulerRotation = Mathf.Clamp(xEulerRotation, -90, 90);
                yEulerRotation += Input.GetAxisRaw("Mouse X") * Sensitivity;
                transform.localRotation = Quaternion.Euler(xEulerRotation, yEulerRotation, 0);
            }

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

            if (Input.GetKey(KeyCode.C) | Input.GetKey(KeyCode.Mouse2))
            {
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, DefaultFiledOfView / 3, Time.deltaTime * 6);
            }
            else
            {
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, DefaultFiledOfView, Time.deltaTime * 6);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CursorUnlocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    CursorUnlocked = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    CursorUnlocked = true;
                }
            }
        }
    }
}
