using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Code repurposed from course material provided by Simon Colreavy
public class NavMesh : MonoBehaviour
{
    Transform player;                   // Reference to the player's position.
    Transform me;                       // Reference to the enemy's position.
    UnityEngine.AI.NavMeshAgent nav;    // Reference to the nav mesh agent.
    Animator anim;
    private float health = 50;
    private bool isPath;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        me = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        nav.isStopped = true;
    }


    void FixedUpdate()
    {
        isPath = nav.SetDestination(player.position);
        if (!isPath)
        {
            nav.isStopped = true;
        }
        if (!nav.isStopped)
        {
            anim.SetTrigger("Move");
        }
        else
        {
            anim.SetTrigger("Stop");
        }
    }

    private void Damage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            nav.isStopped = true;
            anim.SetTrigger("Death");
            StartCoroutine(Death());
        }
    }

    private void setHealth(int newHealth)
    {
        health = newHealth;
    }

    IEnumerator Death()
    {
        float timeElapsed = 0;
        float duration = 1.5f;
        float start = me.eulerAngles.z;
        Vector3 lastAngle = Vector3.zero;
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
        // Trigger a basic death animation ala Minecraft style
        while (timeElapsed < duration)
        {
            lastAngle.z = Mathf.LerpAngle(start, 180, timeElapsed / duration) - lastAngle.z;
            me.Rotate(lastAngle);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}