using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CarrierController>() != null)
        {
            GameStateController.Instance?.Complete();
        }
    }
}
