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
        
        // Când prindem obiectul cu mâna, ne asigurăm că nu mai este "înfipt"
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs arg0)
    {
        isStuck = false;
        // La ridicare, ne asigurăm că fizica redevine normală
        rb.isKinematic = false; 
        transform.SetParent(null); // Îl detașăm de țintă
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificăm dacă a lovit un obiect cu tag-ul "Target"
        if (!isStuck && collision.gameObject.CompareTag("Target"))
        {
            StickToTarget(collision);
            CalculateScore(collision.contacts[0].point, collision.transform);
        }
    }

    void StickToTarget(Collision collision)
    {
        isStuck = true;

        // 1. Oprim forțele fizice ca săgeata să stea nemișcată
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // 2. Setăm săgeata ca un "copil" al țintei (în caz că ținta se mișcă, săgeata se va mișca cu ea)
        transform.SetParent(collision.transform);

        // Opțional: Poți reda un sunet sau activa un Particle System (pentru bonusul de efecte)
        Debug.Log("Săgeata s-a înfipt!");
    }

    void CalculateScore(Vector3 hitPoint, Transform targetTransform)
    {
        // Calculăm distanța dintre punctul unde a lovit și centrul țintei
        float distance = Vector3.Distance(hitPoint, targetTransform.position);
        int score = 0;

        // Ajustează aceste valori în funcție de dimensiunea (scale-ul) țintei tale din Unity
        if (distance < 0.1f) score = 100;
        else if (distance < 0.3f) score = 50;
        else if (distance < 0.5f) score = 10;

        Debug.Log($"Scor obținut: {score}. Distanța: {distance}");
        
        // AICI vei apela scriptul tău de UI pentru a aduna și afișa scorul pe ecran
    }
}