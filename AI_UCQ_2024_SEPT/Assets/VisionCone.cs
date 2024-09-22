using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField]
    public float visionRadius = 5f;  // Radio del cono de visi�n
    [SerializeField]
    public float visionAngle = 60f;  // �ngulo del cono de visi�n
    public Color coneColorDetected = Color.red;  // Color cuando detecta
    public Color coneColorIdle = Color.green;  // Color cuando no detecta

    [SerializeField]
    public float speed = 5f;
    private Transform target; //El objetivo cuando algo pase enfrente


    private bool targetDetected = false;

    void Update()
    {
        DetectTargetInCone();  // Detectar el objetivo dentro del cono de visi�n

        if (targetDetected && target != null)
        {
            // Movimiento b�sico de persecuci�n
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // Detectar si el objetivo est� dentro del cono de visi�n
    void DetectTargetInCone()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, visionRadius);

        targetDetected = false;
        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform detectedTarget = targetCollider.transform;
            Vector3 directionToTarget = (detectedTarget.position - transform.position).normalized;

            // Aqu� calculamos el �ngulo entre la direcci�n hacia el objetivo y la direcci�n del agente usando el producto punto
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);
            float angleToTarget = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            // Si el �ngulo est� dentro del campo de visi�n (menor o igual que el �ngulo de visi�n)
            if (angleToTarget <= visionAngle / 2)
            {
                target = detectedTarget;
                targetDetected = true;
                return;
            }
        }

        target = null;  // Si no detecta ning�n objetivo
    }

    // Visualizaci�n del cono de visi�n con Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = targetDetected ? coneColorDetected : coneColorIdle;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 angleA = DireccionPorAngulo(-visionAngle / 2) * visionRadius;
        Vector3 angleB = DireccionPorAngulo(visionAngle / 2) * visionRadius;

        // Dibujar las l�neas que representan los bordes del cono de visi�n
        Gizmos.DrawLine(transform.position, transform.position + angleA);
        Gizmos.DrawLine(transform.position, transform.position + angleB);

        if (targetDetected && target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    // Obtener la direcci�n a partir de un �ngulo en grados
    public Vector3 DireccionPorAngulo(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;  // Convertimos de grados a radianes
        return new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));  // Usamos seno y coseno para obtener la direcci�n
    }
}
