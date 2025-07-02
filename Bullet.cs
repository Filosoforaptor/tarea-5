using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterDelay(lifeTime));
    }

    public void Move(Vector3 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
        else
        {
            Debug.LogError("La bala no tiene rigidbody.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bala colisiona con {collision.gameObject.name} (tag: {collision.gameObject.tag})", this);

        //Tratamos de aplicar daño al meteorito si tiene el componente MeteoriteModelFinal
        MeteoriteModelFinal meteoriteModel = collision.gameObject.GetComponent<MeteoriteModelFinal>();
        if (meteoriteModel != null)
        {
            Debug.Log($"Bala hace {damage} de daño a {collision.gameObject.name}", this);
            meteoriteModel.TakeDamage(damage);
        }
        ReturnToPool();
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        Debug.Log("Bala regresa al contenedor.");
    }
}