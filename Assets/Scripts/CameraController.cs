using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float scrollMovementSpeed;
    [Range(0.0f,1.0f)][SerializeField] private float cameraMovementSensMultiplier;

    private Vector3 _targetCoords;
    private GameObject _lockedObject;
    private bool _isLocked = false;
    private void Update()
    {
        if (!_isLocked)
        {
            HandleRotation();
            HandleScroll();
            HandleKeyboard();
            return;
        }

        transform.rotation = Quaternion.Euler(33.696f, 0f, 0f);
        Vector3 position = _lockedObject.transform.position;
        transform.position = Vector3.MoveTowards(transform.position,
            position  + new Vector3(0, yOffset, zOffset), movementSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            RotateAroundPoint(Vector3.up, rotationSpeed * cameraMovementSensMultiplier);
        }
        if (Input.GetKey(KeyCode.E))
        {
            RotateAroundPoint(Vector3.up, -rotationSpeed * cameraMovementSensMultiplier);
        }
    }

    private void RotateAroundPoint(Vector3 axis, float angle)
    {
        Vector3 point = Vector3.zero; 
        transform.RotateAround(point, axis, angle);
    }
    
    private void HandleKeyboard()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(xAxis, 0f, zAxis);
        Vector3 localMovement = transform.TransformDirection(movement) * cameraMovementSensMultiplier;

        localMovement.y = 0;

        transform.position += localMovement;
    }


    private void HandleScroll()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Create a movement vector in the local space
            Vector3 scrollMovement = new Vector3(0f, scroll * scrollMovementSpeed, scroll * scrollMovementSpeed);

            // Transform the local movement vector to the world space direction
            Vector3 localScrollMovement = transform.TransformDirection(scrollMovement);

            // Apply the local movement to the camera's position
            transform.position += localScrollMovement;
            transform.position += new Vector3(0f, -scroll * scrollMovementSpeed, 0f);
        }
    }



    public void LockOnGameObject(GameObject target)
    {
        _isLocked = true;
        _lockedObject = target;
        _targetCoords = transform.position + new Vector3(0, yOffset, zOffset);
        transform.position = Vector3.MoveTowards(transform.position,
            _targetCoords , movementSpeed * Time.deltaTime);
    }

    public void UnlockOnGameObject()
    {
        _isLocked = false;
    }
}
