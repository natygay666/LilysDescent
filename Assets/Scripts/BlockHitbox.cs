using UnityEngine;
using UnityEngine;

public class BlockHitbox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Destroy(other.gameObject);
        }
    }
}