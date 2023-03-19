using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 70f; // the speed of the bullet
    public int damage = 50; // the amount of damage the bullet deals to enemies
    public GameObject impactEffect; // the prefab for the impact effect

    private Transform target; // the target the bullet is seeking
    
    

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        //GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //Destroy(effectIns, 2f);
        target.GetComponent<EnemyUnit>().TakeDamage(damage);
        Destroy(gameObject);
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }
}