using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))] 
[RequireComponent(typeof(MeteoriteModelFinal))]

public class MeteoritePresenterFinal : MonoBehaviour 
{
    private MeteoriteModelFinal model; 
    private MeteoriteViewFinal view; 
    private Rigidbody rb;
    private bool isDestroyed = false;

    void Awake()
    {
        model = GetComponent<MeteoriteModelFinal>();
        view = GetComponent<MeteoriteViewFinal>();
        rb = GetComponent<Rigidbody>();

        if (model != null)
        {
            model.OnMeteoriteDestroyed += HandleMeteoriteModelDestroyed;
        }
    }

    void FixedUpdate()
    {
        if (model != null && !isDestroyed)
        {
            float currentSpeed = model.Speed;
            Vector3 direction = Vector3.down;
            // Calcular la velocidad del meteorito
            Vector3 positionChangeThisFrame = direction * currentSpeed * Time.fixedDeltaTime;
            // Calcula nueva posicion basada en la velocidad y el tiempo transcurrido
            Vector3 newCalculatedPosition = rb.position + positionChangeThisFrame;

            if (rb.isKinematic) //Solo se mueve si es kinetico y tuve que desactivar la animacion porque no podia caer el meteorito! caso contrario los disparos me sacaban el meteorito del eje
            {
                rb.MovePosition(newCalculatedPosition);
            }
        }
    }

    private void OnDisable() 
    {
        if (model != null)
        {
            model.OnMeteoriteDestroyed -= HandleMeteoriteModelDestroyed;
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (isDestroyed) return;

        if (model == null)
        {
            Debug.LogError($"MeteoritePresenterFinal ({gameObject.name}): OnCollisionEnter es null en el modelo.", this);
            return;
        }

        // Logica de colision con el player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"MeteoritePresenterFinal ({gameObject.name}): Colision con jugador.", this);
            PlayerPresenter playerPresenter = collision.gameObject.GetComponent<PlayerPresenter>();
            if (playerPresenter != null)
            {
                playerPresenter.ProcessDamage(model.DamageToPlayer);
            }
            else
            {
                Debug.LogError($"MeteoritePresenterFinal ({gameObject.name}): PlayerPresenter No se encontro objeto con tag de player: {collision.gameObject.name}", collision.gameObject);
            }
            HandleDestruction();
            return;
        }

    }

    private void HandleMeteoriteModelDestroyed()
    {
        HandleDestruction();
    }

    private void HandleDestruction()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false; // Desabilitar el collider para evitar colisiones adicionales
        }

        if (view != null)
        {
            view.PlayDestructionEffects();
        }
        Destroy(gameObject); 
    }
}