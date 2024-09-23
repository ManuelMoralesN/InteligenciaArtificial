using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField]
    public float visionRadius = 5f;  // Radio del cono de visión
    [SerializeField]
    public float visionAngle = 60f;  // Ángulo del cono de visión
    public Color coneColorDetected = Color.red;  // Color cuando detecta
    public Color coneColorIdle = Color.green;  // Color cuando no detecta

    [SerializeField]
    public float speed = 5f;
    
    public string targetTag = "MovingObject"; // El tag que el objeto debe tener para ser detectado
    private Transform target; //El objetivo cuando algo pase enfrente


    private bool targetDetected = false;

    void Update()
    {
        DetectTargetInCone();  // Detectar el objetivo dentro del cono de visión

        if (targetDetected && target != null)
        {
            // Movimiento básico de persecución
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // Detectar si el objetivo está dentro del cono de visión
    void DetectTargetInCone()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, visionRadius);

        targetDetected = false;
        foreach (Collider targetCollider in targetsInViewRadius)
        {
            // Verifica si el objeto tiene el tag correcto
            if (targetCollider.CompareTag(targetTag))
            {
                Transform detectedTarget = targetCollider.transform;
                Vector3 directionToTarget = (detectedTarget.position - transform.position).normalized;

                // Aquí calculamos el ángulo entre la dirección hacia el objetivo y la dirección del agente usando el producto punto
                float dotProduct = Vector3.Dot(transform.forward, directionToTarget);
                float angleToTarget = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

                // Si el ángulo está dentro del campo de visión (menor o igual que el ángulo de visión)
                if (angleToTarget <= visionAngle / 2)
                {
                    target = detectedTarget;
                    targetDetected = true;
                    return;
                }
            }
        }

        target = null;  // Si no detecta ningún objetivo
    }

    // Visualización del cono de visión con Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = targetDetected ? coneColorDetected : coneColorIdle;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 angleA = DireccionPorAngulo(-visionAngle / 2) * visionRadius;
        Vector3 angleB = DireccionPorAngulo(visionAngle / 2) * visionRadius;

        // Dibujar las líneas que representan los bordes del cono de visión
        Gizmos.DrawLine(transform.position, transform.position + angleA);
        Gizmos.DrawLine(transform.position, transform.position + angleB);

        if (targetDetected && target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    // Obtener la dirección a partir de un ángulo en grados
    public Vector3 DireccionPorAngulo(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;  // Convertimos de grados a radianes
        return new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));  // Usamos seno y coseno para obtener la dirección
    }
}

//Links, referencias, paginas, etc. https://www.domestika.org/es/courses/716-introduccion-a-unity-para-videojuegos-2d, https://youtu.be/lV47ED8h61k?si=6m012cxUMIkJvd5z, 
//https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwjG19nWr9mIAxVrJEQIHSANN1UQFnoECBkQAQ&url=https%3A%2F%2Fdocs.unity3d.com%2FScriptReference%2FGizmos.html&usg=AOvVaw3Lkfm2k27FE7PVsjCHF7Xw&opi=89978449