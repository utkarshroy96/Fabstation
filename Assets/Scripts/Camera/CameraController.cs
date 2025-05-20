using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform focusObject;
    [SerializeField] Camera mainCamera;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private float rotationAmount;

    public Transform cameraTransform;
    public Quaternion newRotation;

    Vector3 rotateStartPosition;
    Vector3 rotateCurrentPosition;
    Vector3 mouseWorldPosStart;

    bool isRotating;

    public float MovementSpeed
    { 
        get
        {
            return movementSpeed;
        }
        set
        {
            movementSpeed = value;
        }
    }

    public float ZoomSpeed
    {
        get
        {
            return zoomSpeed;
        }
        set
        {
            zoomSpeed = value;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && !Input.GetKey(KeyCode.LeftShift))
        {
            mouseWorldPosStart = GetPerspectivePos();
        }
        if (Input.GetMouseButton(2))
        {
            Pan();
        }

        PerspectiveMovement();
        HandleRotationInput();
        Zoom(Input.GetAxis("Mouse ScrollWheel"));

        transform.SetPositionAndRotation(Vector3.Lerp(transform.position, transform.position, Time.deltaTime * movementTime), Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime));
        cameraTransform.LookAt(transform);
    }

    void PerspectiveMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float pedestal = Input.GetAxis("Pedestal");
        float shiftVar = 1;

        float distance = Vector3.Distance(transform.position, cameraTransform.position);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            shiftVar = 0.2f;
        }

        if (horizontal != 0)
        {
            transform.position += transform.right * horizontal * movementSpeed * shiftVar * Time.deltaTime;
        }
        if (vertical != 0)
        {
            // transform.position += transform.forward * vertical * movementSpeed * shiftVar;

            if (vertical > 0)
            {
                // Zoom In
                if (distance > 1f)
                {
                    cameraTransform.position += cameraTransform.forward * vertical * movementSpeed * shiftVar * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * vertical * movementSpeed * shiftVar * Time.deltaTime;
                }
            }

            if (vertical < 0)
            {
                // Zoom Out
                if (Vector3.Distance(transform.position, cameraTransform.position) < 10)
                {
                    cameraTransform.position += cameraTransform.forward * vertical * movementSpeed * shiftVar * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * vertical * movementSpeed * shiftVar * Time.deltaTime;
                }
            }
        }
        if (pedestal != 0)
        {
            transform.position += transform.up * pedestal * movementSpeed * shiftVar * Time.deltaTime;
        }
    }

    void Zoom(float zoomDiff)
    {
        if (zoomDiff > 0)
        {
            if(mainCamera.orthographic)
            {
                if(mainCamera.orthographicSize >= 0)
                {
                    mainCamera.orthographicSize -= zoomSpeed * Time.deltaTime;
                }
                else
                {
                    mainCamera.orthographicSize = 0;
                }
            }
            else
            {
                // Zoom In
                if (Vector3.Distance(transform.position, cameraTransform.position) > 2)
                {
                    cameraTransform.position += cameraTransform.forward * zoomSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * zoomSpeed * Time.deltaTime;
                }
            }
        }

        if (zoomDiff < 0)
        {
            if(mainCamera.orthographic)
            {
                mainCamera.orthographicSize += zoomSpeed * Time.deltaTime;
            }
            else
            {
                // Zoom Out
                if (Vector3.Distance(transform.position, cameraTransform.position) < 10)
                {
                    cameraTransform.position -= cameraTransform.forward * zoomSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position -= transform.forward * zoomSpeed * Time.deltaTime;
                }
            }
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            rotateStartPosition = Input.mousePosition;
        }
        if (isRotating && Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            Vector3 rotationEulerAngles = newRotation.eulerAngles;

            if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
            {
                rotationEulerAngles += Vector3.right * (difference.y / 5f) * rotationAmount;

                if (rotationEulerAngles.x < 265f)
                {
                    if (rotationEulerAngles.x > 88f)
                    {
                        rotationEulerAngles.x = 88f;
                    }
                }
                else if (rotationEulerAngles.x < 272f)
                {
                    rotationEulerAngles.x = 272f;
                }
            }
            else
            {
                rotationEulerAngles = newRotation.eulerAngles - (Vector3.up * (difference.x / 5f) * rotationAmount);
            }

            newRotation = Quaternion.Euler(rotationEulerAngles);
        }
        if (isRotating && Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }
    }

    void Pan()
    {
        float shiftVar = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            shiftVar = 0.2f;
        }

        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            transform.Translate(new Vector3(-Input.GetAxis("Mouse X") * shiftVar, -Input.GetAxis("Mouse Y") * shiftVar, 0f));
        }
    }

    Vector3 GetPerspectivePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(transform.forward, 1.0f);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    public void ResetPosRot()
    {
        if (transform.position != Vector3.zero)
        {
            transform.DOMove(Vector3.zero, 0.5f);
        }

        ResetRot();
    }

    public void ResetRot()
    {
        if (transform.eulerAngles != Vector3.zero)
        {
            newRotation = Quaternion.identity;
            // transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);
        }

        cameraTransform.localPosition = new Vector3(0f, 0f, -5f);
    }
}