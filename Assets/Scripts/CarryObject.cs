using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarryObject : MonoBehaviour
{
    [SerializeField] private Transform tray;
    [SerializeField] private float maxHorizontalDistance = 2.15f;
    [SerializeField] private float failBelowTray = 0.75f;

    private bool failed;

    private void FixedUpdate()
    {
        if (failed || tray == null || GameStateController.Instance == null ||
            !GameStateController.Instance.IsRunning)
            return;

        Vector3 local = tray.InverseTransformPoint(transform.position);

        bool slippedTooFar = Mathf.Abs(local.x) > maxHorizontalDistance ||
                             Mathf.Abs(local.z) > maxHorizontalDistance;
        bool fellBelow = transform.position.y < tray.position.y - failBelowTray;

        if (slippedTooFar || fellBelow)
        {
            failed = true;
            GameStateController.Instance.Fail();
        }
    }
}
