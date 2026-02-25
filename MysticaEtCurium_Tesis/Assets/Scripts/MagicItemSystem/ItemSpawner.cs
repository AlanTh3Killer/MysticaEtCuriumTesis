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

        // ✅ NUEVO: Feedback de spawn
        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.ShowSpawnFeedback(spawnPoint.position);

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
        // Buscar el MeshRenderer: Visuales/Mesh/Prueba
        Transform meshTransform = go.transform.Find("Visuales/Mesh/Prueba");

        if (meshTransform == null)
        {
            Debug.LogWarning("[ItemSpawner] No se encontró Visuales/Mesh/Prueba");
            return;
        }

        MeshRenderer meshRenderer = meshTransform.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = meshTransform.GetComponent<MeshFilter>();

        if (meshRenderer == null || meshFilter == null)
        {
            Debug.LogWarning("[ItemSpawner] Falta MeshRenderer o MeshFilter");
            return;
        }

        //Aplicar mesh custom si existe
        MagicItemBehaviour behaviour = go.GetComponent<MagicItemBehaviour>();
        if (behaviour != null && behaviour.data != null && behaviour.data.customMesh != null)
        {
            meshFilter.mesh = behaviour.data.customMesh;
            meshTransform.localScale = behaviour.data.modelScale;
            Debug.Log($"[ItemSpawner] Mesh custom aplicado: {behaviour.data.customMesh.name}");
        }

        // Aplicar material según clasificación
        switch (classification)
        {
            case MagicItemDataSO.ItemClassification.Contenible:
                meshRenderer.material = matContenible;
                break;

            case MagicItemDataSO.ItemClassification.Destruible:
                meshRenderer.material = matDestruible;
                break;

            case MagicItemDataSO.ItemClassification.Vendible:
                meshRenderer.material = matVendible;
                break;
        }

    }
}