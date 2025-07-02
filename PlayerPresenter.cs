using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerPresenter : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerModel _model;
    private PlayerInput _pInput;

    [SerializeField]
    private GameObject _mesh;

    public Action<bool> OnPlayerMoving { get; set; }
    public Action<int> OnCoinsCollected { get; set; }
    public Action<int, int> OnPlayerHealthChangedUI { get; set; }
    public Action OnPlayerDiedUI { get; set; }

    private bool canShoot = true;
    [SerializeField] private float fireRate = 0.5f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _model = GetComponent<PlayerModel>();
        _pInput = GetComponent<PlayerInput>();
    }

    // Inicialización de pool de balas
    void Start()
    {
        if (_model != null && _model.BulletPrefab != null && PoolManager.Instance != null)
        {
            PoolManager.Instance.CreatePool(_model.BulletPrefab);
        }
    }

    // Suscripción a eventos del modelo y del input
    private void OnEnable()
    {
        if (_model != null)
        {
            _model.OnCoinsChanged += PresenterCoinsChanged;
            _model.OnHealthChanged += HandleModelHealthChanged;
            _model.OnPlayerDied += HandleModelPlayerDied;
        }

        if (_pInput != null)
        {
            _pInput.OnFireInput += HandleFire;
        }
    }

    // Desuscripción de eventos
    private void OnDisable()
    {
        if (_model != null)
        {
            _model.OnCoinsChanged -= PresenterCoinsChanged;
            _model.OnHealthChanged -= HandleModelHealthChanged;
            _model.OnPlayerDied -= HandleModelPlayerDied;
        }

        if (_pInput != null)
        {
            _pInput.OnFireInput -= HandleFire;
        }
    }

    // Movimiento y rotación del jugador en FixedUpdate
    void FixedUpdate()
    {
        if (_pInput != null && !_pInput.enabled)
        {
            return;
        }

        Vector3 input = _pInput.Axis;
        ApplyMovement(input);
        UpdateTilt(input.x);
    }

    // Aplica el movimiento al Rigidbody y notifica si el jugador se está moviendo
    public void ApplyMovement(Vector3 direction)
    {
        if (_model == null) return;
        _rb.velocity = _model.CalculateMove(direction);

        bool isMoving = direction.magnitude > 0.1f;
        OnPlayerMoving?.Invoke(isMoving);
    }

    // Actualiza la inclinación visual del mesh según el input horizontal
    private void UpdateTilt(float inputX)
    {
        if (_model == null || _mesh == null) return;
        Quaternion targetRotation = _model.CalculateTargetRotation(inputX);
        Quaternion currentRotation = _mesh.transform.localRotation;
        _mesh.transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, _model.TiltSpeed * Time.fixedDeltaTime);
    }

    // Procesa el daño recibido por el jugador
    public void ProcessDamage(int damageAmount)
    {
        if (_model != null)
        {
            _model.TakeDamage(damageAmount);
        }
    }

    // Actualiza la UI de vida del jugador
    private void HandleModelHealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChangedUI?.Invoke(currentHealth, maxHealth);
    }

    // Maneja la muerte del jugador: desactiva el input y notifica a la UI
    private void HandleModelPlayerDied()
    {
        if (_pInput != null)
        {
            _pInput.enabled = false;
        }
        OnPlayerDiedUI?.Invoke();
    }

    // Maneja el disparo del jugador
    private void HandleFire()
    {
        if (!canShoot) return;

        if (_model == null || _model.BulletPrefab == null || PoolManager.Instance == null || _model.FirePoint == null)
        {
            Debug.LogError("PlayerPresenter: Faltan referencias para disparar. Modelo: " + (_model != null) +
                           ", PrefabBala: " + (_model?.BulletPrefab != null) +
                           ", PoolManager: " + (PoolManager.Instance != null) +
                           ", FirePoint: " + (_model?.FirePoint != null));
            return;
        }

        GameObject bulletObject = PoolManager.Instance.GetObject(_model.BulletPrefab);
        if (bulletObject != null)
        {
            bulletObject.transform.position = _model.FirePoint.position;
            bulletObject.transform.rotation = _model.FirePoint.rotation;

            Bullet bulletComponent = bulletObject.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Move(_model.FirePoint.forward);
            }
            else
            {
                Debug.LogError("PlayerPresenter: El prefab de bala no tiene el componente Bullet. Se devuelve al pool.");
                PoolManager.Instance.ReturnObject(bulletObject);
            }
            StartCoroutine(ShootCooldown());
        }
    }

    // Controla el cooldown entre disparos
    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    // Detecta colisiones con monedas unicamente
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            _model.AddCoin();
            Destroy(other.gameObject);
        }
    }

    // Notifica a la UI la cantidad de monedas recolectadas
    private void PresenterCoinsChanged(int newCoinCount)
    {
        OnCoinsCollected?.Invoke(newCoinCount);
    }
}