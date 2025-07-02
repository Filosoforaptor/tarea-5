using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PoolManager es NULO.");
            }
            return _instance;
        }
    }

    [SerializeField]
    private Transform _container;
    [SerializeField]
    private int _initialPoolSize = 10;

    private Dictionary<GameObject, List<GameObject>> pools;

    // Inicializa la instancia del PoolManager y el diccionario de pools.
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            pools = new Dictionary<GameObject, List<GameObject>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Crea un pool para el prefab especificado con la cantidad indicada de objetos.
    public void CreatePool(GameObject prefab, int size)
    {
        if (pools.ContainsKey(prefab))
        {
            Debug.LogWarning($"El pool para {prefab.name} ya existe.");
            return;
        }

        List<GameObject> objectList = new List<GameObject>();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = $"{prefab.name}_{i}";
            obj.transform.SetParent(_container);
            obj.SetActive(false);
            objectList.Add(obj);
        }
        pools.Add(prefab, objectList);
        Debug.Log($"Pool creado para {prefab.name} con {size} objetos.");
    }

    // Crea un pool para el prefab especificado usando el tamaño inicial por defecto.
    public void CreatePool(GameObject prefab)
    {
        CreatePool(prefab, _initialPoolSize);
    }

    // Devuelve un objeto activo del pool correspondiente al prefab.
    public GameObject GetObject(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogError($"No se encontró un pool para {prefab.name}.");
            return null;
        }

        List<GameObject> objectList = pools[prefab];
        for (int i = 0; i < objectList.Count; i++)
        {
            if (!objectList[i].activeInHierarchy)
            {
                objectList[i].SetActive(true);
                return objectList[i];
            }
        }
        Debug.LogWarning($"No hay objetos inactivos disponibles en el pool para {prefab.name}. Considera expandir el pool.");
        return null;
    }

    // Devuelve un objeto al pool, desactivándolo.
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    // Verifica si ya existe un pool para el prefab dado.
    public bool IsPoolCreated(GameObject prefabKey)
    {
        if (prefabKey == null) return false;
        return pools.ContainsKey(prefabKey);
    }
}