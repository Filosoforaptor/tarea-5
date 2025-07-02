using System.Collections;
using UnityEngine;

// Duplicate using statements removed

public class Respawner : MonoBehaviour
{
    [SerializeField] private BoxCollider respawnArea;     // Asignar en inspector
    [SerializeField] private float respawnY = 1f;          // Altura de respawn
    [SerializeField] private GameObject[] objectsToSpawn; // Array de prefabs (coins, Meteorites, PowerUps)
    [SerializeField] private float spawnInterval = 2f;     // Intervalo entre spawns

    private void Start()
    {
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            Debug.LogError("Respawner: No se asignaron objetos al array objectsToSpawn. Por favor, asigna prefabs en el Inspector.", this);
            return;
        }

        if (respawnArea == null)
        {
            Debug.LogError("Respawner: El área de respawn (BoxCollider) no está asignada. Por favor, asígnala en el Inspector.", this);
            return;
        }

        StartCoroutine(SpawnObjectsRoutine());
    }

    // Corrutina que instancia objetos periódicamente en posiciones aleatorias dentro del área de respawn
    private IEnumerator SpawnObjectsRoutine()
    {
        while (true)
        {
            // Verifica que el array no esté vacío
            if (objectsToSpawn.Length == 0)
            {
                yield return new WaitForSeconds(spawnInterval);
                continue;
            }

            GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
            if (prefab == null)
            {
                Debug.LogWarning("Respawner: Se seleccionó un prefab nulo del array objectsToSpawn. Se omite este ciclo de respawn.", this);
                yield return new WaitForSeconds(spawnInterval);
                continue;
            }

            GameObject newObj = Instantiate(prefab);
            Debug.Log($"Respawner: Objeto {newObj.name} instanciado.", this);
            RespawnAtRandomX(newObj);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Evento que se dispara cuando un objeto entra en el trigger del área de respawn
    private void OnTriggerEnter(Collider other)
    {
        if (respawnArea == null) return;

        if (other.CompareTag("Meteorite") || other.CompareTag("Coin") || other.CompareTag("PowerUp")) //no le copa ni miercoles cuando entra la nave
        {
            Debug.Log($"Respawner: El objeto {other.name} con tag {other.tag} reingresó al trigger de respawn. Se reposiciona.", this);
            RespawnAtRandomX(other.gameObject);
        }
    }

    // Reposiciona el objeto recibido en una posición aleatoria dentro del área de respawn
    public void RespawnAtRandomX(GameObject obj)
    {
        if (respawnArea == null)
        {
            Debug.LogError("Respawner: El área de respawn es nula en RespawnAtRandomX. No se puede reposicionar el objeto.", this);
            return;
        }
        Vector3 center = respawnArea.bounds.center;
        float halfWidth = respawnArea.bounds.extents.x;

        float randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);
        // Se mantiene la posición Z del centro del área de respawn
        Vector3 respawnPos = new Vector3(randomX, respawnY, center.z);

        obj.transform.position = respawnPos;
        obj.transform.rotation = Quaternion.identity; // Reinicia la rotación

        // Reinicia el estado físico si tiene Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
    }
}