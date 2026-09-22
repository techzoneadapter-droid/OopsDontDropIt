using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpinnerObstacle : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.up;
    [SerializeField] private float degreesPerSecond = 95f;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        Quaternion delta = Quaternion.AngleAxis(degreesPerSecond * Time.fixedDeltaTime, axis.normalized);
        body.MoveRotation(body.rotation * delta);
    }
}
