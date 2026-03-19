using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject itemPrefab;                // Prefab placeholder genérico
    public Transform spawnPoint;

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

        if (currentItem != null) return;

        if (posiblesItems == null || posiblesItems.Count == 0)
        {
            Debug.LogError("[ItemSpawner] No hay ScriptableObjects asignados.");
            return;
        }

        MagicItemDataSO data = posiblesItems[Random.Range(0, posiblesItems.Count)];

        // Crear instancia del ItemPrueba (estructura completa)
        currentItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // Asignar data
        MagicItemBehaviour behaviour = currentItem.GetComponent<MagicItemBehaviour>();
        behaviour.data = data;
        behaviour.inspectedCharacteristics.Clear(); // ← limpiar progreso previo

        //Reemplazar visual si existe customPrefab
        ReplaceVisual(currentItem, data);
        // Aplicar efectos visuales de características
        VisualCharacteristicsApplier applier = currentItem.GetComponent<VisualCharacteristicsApplier>();
        if (applier == null)
        {
            applier = currentItem.AddComponent<VisualCharacteristicsApplier>();

            // Asignar prefabs de efectos (esto solo la primera vez)
            // OPCIÓN A: Asignar manualmente en el Inspector del ItemPrueba prefab
            // OPCIÓN B: Cargar desde Resources (ver abajo)
            
        }

        applier.ApplyCharacteristics(data, currentItem);
        // Feedback de spawn
        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.ShowSpawnFeedback(spawnPoint.position);

        Debug.Log("[ItemSpawner] Nuevo item generado: " + data.itemName);
    }

    private GameObject GetEffectPrefab(ItemCharacteristic characteristic)
    {
        string prefabName = GetPrefabName(characteristic);
        if (string.IsNullOrEmpty(prefabName)) return null;

        return Resources.Load<GameObject>($"VFX/{prefabName}");
    }

    private string GetPrefabName(ItemCharacteristic c)
    {
        switch (c)
        {
            case ItemCharacteristic.AuraBlanca: return "AuraBlanca";
            case ItemCharacteristic.AuraNaranja: return "AuraNaranja";
            case ItemCharacteristic.AuraRoja: return "AuraRoja";
            case ItemCharacteristic.AuraOscura: return "AuraOscura";
            case ItemCharacteristic.RunasBenignasVisibles: return "RunaBenigna_VFX";
            case ItemCharacteristic.RunasDefensivas: return "RunaDefensivas_VFX";
            case ItemCharacteristic.RunasInvocacion: return "RunaInvocacion_VFX";
            case ItemCharacteristic.RunasMalignas: return "RunaMalignas_VFX";
            
            default: return null;
        }
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

    // Reemplazar solo el contenido visual
    private void ReplaceVisual(GameObject item, MagicItemDataSO data)
    {
        // Buscar la ruta Visuales/Mesh
        Transform visualesTransform = item.transform.Find("Visuales");
        if (visualesTransform == null)
        {
            Debug.LogError("[ItemSpawner] No se encontró Visuales en ItemPrueba");
            return;
        }

        Transform meshTransform = visualesTransform.Find("Mesh");
        if (meshTransform == null)
        {
            Debug.LogError("[ItemSpawner] No se encontró Visuales/Mesh en ItemPrueba");
            return;
        }

        // Si hay visualPrefab custom, reemplazar todo el contenido de Mesh
        if (data.visualPrefab != null)
        {
            // Destruir el placeholder (Prueba)
            foreach (Transform child in meshTransform)
            {
                Destroy(child.gameObject);
            }

            // Instanciar el prefab custom como hijo de Mesh
            GameObject visualInstance = Instantiate(data.visualPrefab, meshTransform);
            visualInstance.transform.localPosition = Vector3.zero;
            visualInstance.transform.localRotation = Quaternion.identity;
            visualInstance.transform.localScale = Vector3.one;

            Debug.Log($"[ItemSpawner] Visual custom aplicado: {data.visualPrefab.name}");
        }
        else
        {
            // Si NO hay customPrefab, usar placeholder y colorearlo
            ApplyClassificationMaterial(meshTransform.gameObject, data.classification);
            Debug.Log($"[ItemSpawner] Placeholder coloreado: {data.classification}");
        }
    }



    //Colorear el placeholder (hijo de Mesh)
    private void ApplyClassificationMaterial(GameObject meshContainer, MagicItemDataSO.ItemClassification classification)
    {
        // Buscar TODOS los MeshRenderers dentro de Visuales/Mesh
        MeshRenderer[] renderers = meshContainer.GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("[ItemSpawner] No se encontraron MeshRenderers en Visuales/Mesh");
            return;
        }

        Material materialToApply = null;

        switch (classification)
        {
            case MagicItemDataSO.ItemClassification.Contenible:
                materialToApply = matContenible;
                break;

            case MagicItemDataSO.ItemClassification.Destruible:
                materialToApply = matDestruible;
                break;

            case MagicItemDataSO.ItemClassification.Vendible:
                materialToApply = matVendible;
                break;
        }

        // Aplicar material a todos los renderers
        foreach (var renderer in renderers)
        {
            renderer.material = materialToApply;
        }

        Debug.Log($"[ItemSpawner] Material aplicado a {renderers.Length} renderers");
    }
}