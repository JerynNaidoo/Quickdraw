using UnityEngine;
using UnityEngine.AI;



public class Enemy : MonoBehaviour
{

    public Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} cannot move - missing NavMesh or player!");
        }
    }
}
