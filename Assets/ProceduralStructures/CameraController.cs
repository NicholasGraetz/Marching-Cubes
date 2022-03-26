using UnityEngine;

// Attach to a camera
// TODO use the event-system instead
public class CameraController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 10f;
    [SerializeField] float shiftSpeedup = 5;
    [SerializeField] float rotationSpeed = 1f;

    Vector3 lastMousePosition;
    Quaternion targetRotation;


    // Use this for initialization
    void Start()
    {
        lastMousePosition = Input.mousePosition;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            HandleMovement();
            HandleRotation();
        }

        lastMousePosition = Input.mousePosition;
    }

    private void HandleRotation()
    {
        float horizontalRotationSpeed = (Input.mousePosition.x - lastMousePosition.x) * rotationSpeed;
        float verticalRotationSpeed = (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed;

        Quaternion horizontalRotation = Quaternion.Euler(new Vector3(0, 1, 0) * horizontalRotationSpeed);
        Quaternion verticalRotation = Quaternion.Euler(Vector3.Normalize(Vector3.Cross(transform.up, transform.forward)) * -verticalRotationSpeed);

        targetRotation = horizontalRotation * verticalRotation * targetRotation;
        transform.rotation = targetRotation;
    }

    private void HandleMovement()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        Vector3 left = Vector3.Cross(transform.forward, transform.up);

        moveDir -= left * Input.GetAxis("Horizontal");
        moveDir += transform.forward * Input.GetAxis("Vertical");
        moveDir += transform.up * Input.GetAxis("Elevation");

        moveDir *= Time.deltaTime * moveSpeed;

        if (Input.GetButton("Fire3"))
            moveDir *= shiftSpeedup;

        transform.position = new Vector3(
            transform.position.x + moveDir.x,
            transform.position.y + moveDir.y,
            transform.position.z + moveDir.z
        );
    }
}

