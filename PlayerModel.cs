using System;
using System.Collections; // Requerido para Coroutines
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject initialBulletPrefab; // Prefab de bala inicial
    [SerializeField] private Transform firePoint;

    public GameObject BulletPrefab { get; private set; }
    public Transform FirePoint => firePoint;

    [Header("Movimiento")]
    [SerializeField] private Quaternion initTiltRotation;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tiltAngle = 30f;
    [SerializeField] private float tiltSpeed = 5f;

    public int CurrentCoins { get; private set; }

    [Header("Estadísticas de vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public event Action<int> OnCoinsChanged;
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    public float TiltSpeed { get => tiltSpeed; private set => tiltSpeed = value; }
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private PowerUpData currentPowerUp;
    private Coroutine powerUpCoroutine;

    // Inicializa la vida y el prefab de bala al iniciar el objeto
    void Awake()
    {
        currentHealth = maxHealth;
        if (initialBulletPrefab == null)
        {
            Debug.LogError($"PlayerModel ({gameObject.name}): ¡InitialBulletPrefab no está asignado en el Inspector!", this);
        }
        BulletPrefab = initialBulletPrefab; // Inicializa el prefab de bala actual
        Debug.Log($"PlayerModel ({gameObject.name}): Inicializado. Bala actual: {BulletPrefab?.name}", this);
    }

    // Calcula el vector de movimiento normalizado multiplicado por la velocidad
    public Vector3 CalculateMove(Vector3 direction)
    {
        return direction.normalized * moveSpeed;
    }

    // Calcula la rotación objetivo para el tilt visual del jugador
    public Quaternion CalculateTargetRotation(float inputX)
    {
        float tiltZ = 0f;
        if (Mathf.Abs(inputX) > 0.01f)
            tiltZ = -inputX * tiltAngle;
        return initTiltRotation * Quaternion.Euler(0f, 0f, tiltZ);
    }

    // Suma una moneda y notifica el cambio
    public void AddCoin()
    {
        CurrentCoins++;
        Debug.Log($"PlayerModel ({gameObject.name}): Moneda agregada, total de monedas: {CurrentCoins}", this);
        OnCoinsChanged?.Invoke(CurrentCoins);
    }

    // Aplica daño al jugador y notifica cambios de vida o muerte
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log($"PlayerModel ({gameObject.name}): Daño recibido, vida actual {currentHealth}/{maxHealth}", this);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log($"PlayerModel ({gameObject.name}): El jugador se murio.", this);
            OnPlayerDied?.Invoke();
        }
    }

    // Aplica un power-up al jugador, cambiando el tipo de bala y gestionando la duración
    public void ApplyPowerUp(PowerUpData newPowerUpData)
    {
        if (newPowerUpData == null)
        {
            Debug.LogWarning($"PlayerModel ({gameObject.name}): ApplyPowerUp llamado con datos nulos.", this);
            return;
        }

        Debug.Log($"PlayerModel ({gameObject.name}): Aplicando power-up - {newPowerUpData.powerUpName}", this);
        currentPowerUp = newPowerUpData;

        if (powerUpCoroutine != null)
        {
            StopCoroutine(powerUpCoroutine);
            powerUpCoroutine = null;
        }

        if (currentPowerUp.bulletPrefab != null)
        {
            BulletPrefab = currentPowerUp.bulletPrefab;
            Debug.Log($"PlayerModel ({gameObject.name}): BulletPrefab cambiado a {BulletPrefab.name}", this);
            if (PoolManager.Instance != null)
            {
                if (!PoolManager.Instance.IsPoolCreated(BulletPrefab))
                {
                    PoolManager.Instance.CreatePool(BulletPrefab);
                }
            }
        }
        else
        {
            BulletPrefab = initialBulletPrefab; // Revertir si el power-up no tiene bala
        }

        if (currentPowerUp.duration > 0)
        {
            powerUpCoroutine = StartCoroutine(PowerUpTimer(currentPowerUp.duration));
        }
    }

    // Corrutina que controla la duración del power-up con una interfaz
    private IEnumerator PowerUpTimer(float duration)
    {
        Debug.Log($"PlayerModel ({gameObject.name}): Power-up {currentPowerUp.powerUpName} activo por {duration} segundos.", this);
        yield return new WaitForSeconds(duration);
        Debug.Log($"PlayerModel ({gameObject.name}): Duración del power-up {currentPowerUp.powerUpName} finalizada.", this);
        RevertToInitialBullet();
        currentPowerUp = null;
        powerUpCoroutine = null;
    }

    // Revierte el tipo de bala al inicial tras finalizar el power-up
    private void RevertToInitialBullet()
    {
        Debug.Log($"PlayerModel ({gameObject.name}): Revirtiendo al tipo de bala inicial: {initialBulletPrefab?.name}", this);
        if (initialBulletPrefab != null)
        {
            BulletPrefab = initialBulletPrefab;
            if (PoolManager.Instance != null)
            {
                if (!PoolManager.Instance.IsPoolCreated(BulletPrefab))
                {
                    Debug.LogWarning($"PlayerModel ({gameObject.name}): El pool de la bala inicial {BulletPrefab.name} no se encontró al revertir. Creándolo ahora.", this);
                    PoolManager.Instance.CreatePool(BulletPrefab);
                }
            }
        }
    }
}