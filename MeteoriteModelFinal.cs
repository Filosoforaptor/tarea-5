using System;
using UnityEngine;

public class MeteoriteModelFinal : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private int damageToPlayer = 1;
    [SerializeField] private float speed = 5f;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public event Action OnMeteoriteDestroyed;
    public int DamageToPlayer => damageToPlayer;
    public float Speed => speed;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // Inicializa la vida del meteorito al activarse
    void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"MeteoriteModelFinal ({gameObject.name}): Inicia con vida {currentHealth}/{maxHealth}");
    }

    // Aplica da�o al meteorito y verifica si debe ser destruido
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; // Ya est� destruido o pendiente de destrucci�n

        currentHealth -= amount;
        Debug.Log($"MeteoriteModelFinal ({gameObject.name}): Recibi� {amount} de da�o, vida actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"MeteoriteModelFinal ({gameObject.name}): Vida agotada, invocando OnMeteoriteDestroyed.");
            OnMeteoriteDestroyed?.Invoke();
        }
    }
}