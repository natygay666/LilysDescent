using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public float lifetime = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

}