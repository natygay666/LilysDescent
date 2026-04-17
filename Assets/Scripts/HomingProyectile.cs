using UnityEngine;

public class HomingProyectile : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 200f;
    public float lifeTime = 5f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.forward).y;

        transform.Rotate(0, rotateAmount * rotateSpeed * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // o aplicar daño
            Destroy(gameObject);
        }
    }
}