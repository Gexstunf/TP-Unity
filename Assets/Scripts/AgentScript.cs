using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AgentScript : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] Animator anim;
    [SerializeField] float minDistance = 0.5f;

    [SerializeField] Transform player;
    [SerializeField] Transform eyePoint;

    int currentPatrolIndex = 0;
    float velocity;
    float loseTimer = 0f;
    bool chasing = false;

    [Header("Detección")]
    [SerializeField] float rayDistance = 5f;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] float fieldOfView = 45f;
    [SerializeField] float losePlayerTime = 2f;
    [SerializeField] int raysInCone = 5;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (eyePoint == null)
            eyePoint = transform; // si no hay ojo asignado, usa la base
    }

    void Update()
    {
        if (chasing)
            Chase();
        else
        {
            Patrol();
            DetectPlayer();
        }

        // Animación de velocidad
        velocity = agent.velocity.magnitude;
        if (anim != null)
            anim.SetFloat("Speed", velocity);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;

        if (!agent.pathPending && agent.remainingDistance <= minDistance)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Chase()
    {
        if (player == null) return;

        agent.destination = player.position;

        if (agent.remainingDistance <= 1.2f)
            SceneManager.LoadScene("GameOverScene");

        if (!CanSeePlayer())
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= losePlayerTime)
            {
                loseTimer = 0f;
                chasing = false;
                currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            }
        }
        else
        {
            loseTimer = 0f;
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        if (CanSeePlayer())
            chasing = true;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - eyePoint.position).normalized;

        for (int i = -raysInCone; i <= raysInCone; i++)
        {
            float angle = (fieldOfView / raysInCone) * i;
            Vector3 rayDir = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(eyePoint.position, rayDir, out hit, rayDistance, detectionMask))
            {
                if (hit.collider.CompareTag("Player"))
                    return true;
            }
        }

        return false;
    }

    // --- Dibujo de Gizmos ---
    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null)
            eyePoint = transform;

        Gizmos.color = Color.yellow;
        // Dibujar rayos del cono de visión
        for (int i = -raysInCone; i <= raysInCone; i++)
        {
            float angle = (fieldOfView / raysInCone) * i;
            Vector3 rayDir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawRay(eyePoint.position, rayDir * rayDistance);
        }

        // Dibujar línea hacia el jugador si lo ve
        if (player != null && CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePoint.position, player.position);
        }
    }
}
