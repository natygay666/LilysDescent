using UnityEngine;

public class HomingShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject target = FindClosestEnemy();

            if (target != null)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                proj.GetComponent<HomingProyectile>().SetTarget(target.transform);
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPos, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }
}