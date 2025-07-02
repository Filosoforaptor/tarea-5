using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PowerUpController : MonoBehaviour
{
    public PowerUpData powerUpData;
    public float fallSpeed = 2f;

    private Rigidbody _rb;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (!col.isTrigger)
            {
                col.isTrigger = true;
            }
        }

        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            // Ya no recuerdo porque necesite implementar este debug...
            Debug.LogError($"PowerUpController ({gameObject.name}): ¡Falta el componente Rigidbody! El power-up no se moverá como se espera. Esto es inesperado con RequireComponent.", this);
        }
        else
        {
            _rb.useGravity = false; // Controlamos la gravedad manualmente
        }
    }

    void Start()
    {
        // Comprobamos si powerUpData ha sido asignado
        if (powerUpData == null)
        {
            Debug.LogError($"PowerUpController ({gameObject.name}): PowerUpData no fue asignado en el inspector.", this);
        }

        // Aplicamos la velocidad de caída al Rigidbody
        if (_rb != null)
        {
            _rb.velocity = Vector3.down * fallSpeed;
            Debug.Log($"PowerUpController ({gameObject.name}): Movimiento iniciado. Velocidad: {_rb.velocity} (Velocidad de caída: {fallSpeed})", this);
        }
    }

    // Detecta colisiones con el jugador en caso de que sea un PowerUp
    private void OnTriggerEnter(Collider other)
    {
        if (powerUpData == null)
        {
            Debug.LogError($"PowerUpController ({gameObject.name}): PowerUpData no asignado en el momento de la colisión. No se puede aplicar el power-up.", this);
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log($"PowerUpController ({gameObject.name}): Colisionó con el Jugador ({other.name}). Intentando aplicar power-up: {powerUpData.powerUpName}", this);

            PlayerModel playerModel = other.GetComponent<PlayerModel>();
            if (playerModel != null)
            {
                playerModel.ApplyPowerUp(powerUpData);
                Debug.Log($"PowerUpController: PlayerModel encontrado en {other.name}. Power-up '{powerUpData.powerUpName}' aplicado.", this);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"PowerUpController ({gameObject.name}): Componente PlayerModel no encontrado en el objeto Player {other.name}. No se puede aplicar el power-up.", this);
            }
        }
    }
}