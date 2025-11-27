using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject itemPrefab;                // Tu prefab ItemPrueba
    public Transform spawnPoint;                 // Punto donde aparecen los objetos

    [Header("Visual Materials")]
    public Material matContenible;               // ItemContenible (Morado)
    public Material matDestruible;               // ItemDestruible (Negro)
    public Material matVendible;                 // ItemVendible (Verde)

    [Header("Item Data (ScriptableObjects)")]
    public List<MagicItemDataSO> posiblesItems;  // Datos de items posibles

    private GameObject currentItem;

    private void Start()
    {
        SpawnNewItem();
    }

    public void SpawnNewItem()
    {
        if (itemPrefab == null || spawnPoint == null)
        {
            Debug.LogError("[ItemSpawner] Falta prefab o spawnPoint.");
            return;
        }

        // Si ya hay uno, no spawnear doble
        if (currentItem != null) return;

        // Elegir un ScriptableObject al azar
        if (posiblesItems == null || posiblesItems.Count == 0)
        {
            Debug.LogError("[ItemSpawner] No hay ScriptableObjects asignados.");
            return;
        }

        MagicItemDataSO data = posiblesItems[Random.Range(0, posiblesItems.Count)];

        // Crear instancia
        currentItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // Asignar data al MagicItemBehaviour
        MagicItemBehaviour behaviour = currentItem.GetComponent<MagicItemBehaviour>();
        behaviour.data = data;

        // Aplicar material según clasificación
        ApplyClassificationMaterial(currentItem, data.classification);

        Debug.Log("[ItemSpawner] Nuevo item generado: " + data.itemName);
    }

    // El contenedor llamará esto cuando destruya un objeto
    public void NotifyItemDestroyed()
    {
        currentItem = null;
        StartCoroutine(SpawnNextDelayed());
    }

    private IEnumerator SpawnNextDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        SpawnNewItem();
    }

    private void ApplyClassificationMaterial(GameObject go, MagicItemDataSO.ItemClassification classification)
    {
        // Buscar el MeshRenderer exacto: Visuales/Mesh/Prueba
        MeshRenderer mesh = go.transform.Find("Visuales/Mesh/Prueba").GetComponent<MeshRenderer>();

        if (mesh == null)
        {
            Debug.LogWarning("[ItemSpawner] No se encontró el MeshRenderer en Visuales/Mesh/Prueba");
            return;
        }

        switch (classification)
        {
            case MagicItemDataSO.ItemClassification.Contenible:
                mesh.material = matContenible;
                break;

            case MagicItemDataSO.ItemClassification.Destruible:
                mesh.material = matDestruible;
                break;

            case MagicItemDataSO.ItemClassification.Vendible:
                mesh.material = matVendible;
                break;
        }
    }
}