using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarrierController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 2.7f;
    [SerializeField] private float gentleWobble = 0.025f;
    [SerializeField] private float wobbleFrequency = 3.2f;

    private Rigidbody body;
    private Vector3 startPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (GameStateController.Instance == null || !GameStateController.Instance.IsRunning)
            return;

        Vector3 next = body.position + transform.forward * (forwardSpeed * Time.fixedDeltaTime);
        next.x = startPosition.x + Mathf.Sin(Time.time * wobbleFrequency) * gentleWobble;
        body.MovePosition(next);
    }
}
