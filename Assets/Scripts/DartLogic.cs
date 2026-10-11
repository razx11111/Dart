using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class DartLogic : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isStuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs arg0)
    {
        isStuck = false;
        rb.isKinematic = false;
        transform.SetParent(null);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isStuck && collision.gameObject.CompareTag("Target"))
        {
            Vector3 hitPoint = collision.contacts[0].point;

            // infigere
            isStuck = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            transform.SetParent(collision.transform);

            // procesare lovitura
            DartTarget target = collision.gameObject.GetComponentInParent<DartTarget>();
            if (target != null)
                target.ProcessHit(hitPoint);
        }
    }
}