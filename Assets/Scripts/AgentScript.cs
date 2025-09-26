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

    Transform player;
    int currentPatrolIndex = 0;
    float velocity;
    float loseTimer = 0f;
    bool chasing = false;

    [SerializeField] float rayDistance = 2f;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] float fieldOfView = 45f;
    [SerializeField] float losePlayerTime = 2f;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasing)
        {
            Chase();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }

        // Actualizar animación de velocidad
        velocity = agent.velocity.magnitude;
        anim.SetFloat("Speed", velocity);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;

        if (!agent.pathPending && agent.remainingDistance <= minDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        if (player == null) return;

        agent.destination = player.position;

        if (agent.remainingDistance <= 1.2f)
        {
            Debug.Log("Jugador atrapado!");
            SceneManager.LoadScene("GameOverScene");
        }

        if (!CanSeePlayer())
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= losePlayerTime)
            {
                loseTimer = 0f;
                chasing = false; // vuelve a patrullar
                currentPatrolIndex = Random.Range(0, patrolPoints.Length); // arranca desde cualquier punto
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
        {
            chasing = true;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // Chequeo ángulo de visión
        if (Vector3.Angle(transform.forward, dirToPlayer) < fieldOfView)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out hit, rayDistance, detectionMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position + Vector3.up, dirToPlayer * rayDistance, Color.red);
                    return true;
                }
            }
        }

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * rayDistance, Color.green);
        return false;
    }
}
