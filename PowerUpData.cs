using UnityEngine;

// Permite crear un nuevo asset de datos de PowerUp desde el menú de Unity
[CreateAssetMenu(fileName = "NewPowerUpData", menuName = "Gameplay/PowerUp Data")]
public class PowerUpData : ScriptableObject
{
    [Header("Información")]
    public string powerUpName = "Nuevo PowerUp"; // Nombre del power-up
    public Sprite powerUpIcon; // Icono para la interfaz pero no lo use por renegar con fisicas

    [Header("Efecto de Gameplay")]
    public GameObject bulletPrefab; // Prefab de bala que otorga este power-up. Puede ser nulo si el power-up tiene otro efecto.
    public float duration = 10f;   // Duración del power-up en segundos. <= 0 para permanente o hasta ser reemplazado.

    [Header("Visuales")]
    public Color powerUpColor = Color.white; // Opcional: color para feedback visual en el jugador o la bala
}