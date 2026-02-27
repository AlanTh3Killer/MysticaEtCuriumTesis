using System.Collections.Generic;
using UnityEngine;

public class VisualCharacteristicsApplier : MonoBehaviour
{
    [Header("Prefabs de Auras")]
    public GameObject auraBlancaPrefab;
    public GameObject auraNaranjaPrefab;
    public GameObject auraRojaPrefab;
    public GameObject auraOscuraPrefab;

    [Header("Prefabs de Runas")]
    public GameObject runaBenignaPrefab;
    public GameObject runaMalignaPrefab;
    public GameObject runaInvocacionPrefab;
    public GameObject runaDefensivaPrefab;

    [Header("Prefabs de Sonidos (Opcional)")]
    public GameObject sonidoArcanoVisualPrefab;
    public GameObject sonidoRitmicoVisualPrefab;
    public GameObject vocesEspectralesVisualPrefab;
    public GameObject vocesDemoniacasVisualPrefab;

    // Diccionario: característica ? efecto instanciado
    private Dictionary<ItemCharacteristic, GameObject> effectMap
        = new Dictionary<ItemCharacteristic, GameObject>();

    private Transform fvxContainer;

    public void ApplyCharacteristics(MagicItemDataSO data, GameObject targetObject)
    {
        Transform visuales = targetObject.transform.Find("Visuales");
        if (visuales == null) { Debug.LogError("[VCA] No se encontró Visuales"); return; }

        fvxContainer = visuales.Find("VFX");
        if (fvxContainer == null) { Debug.LogError("[VCA] No se encontró VFX"); return; }

        ClearEffects();

        foreach (var characteristic in data.characteristics)
        {
            GameObject effectPrefab = GetEffectPrefab(characteristic);
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, fvxContainer);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localRotation = Quaternion.identity;
                effect.transform.localScale = Vector3.one;

                effect.SetActive(false); // ? DESACTIVADO al inicio

                effectMap[characteristic] = effect;
            }
        }

        Debug.Log($"[VCA] {effectMap.Count} efectos registrados (desactivados)");
    }

    // Llamado por SimpleTool cuando detecta características
    public void RevealCharacteristics(List<ItemCharacteristic> characteristics)
    {
        foreach (var c in characteristics)
        {
            if (effectMap.TryGetValue(c, out GameObject effect))
            {
                effect.SetActive(true);
                Debug.Log($"[VCA] Efecto revelado: {c}");
            }
        }
    }

    private GameObject GetEffectPrefab(ItemCharacteristic characteristic)
    {
        switch (characteristic)
        {
            case ItemCharacteristic.AuraBlanca: return auraBlancaPrefab;
            case ItemCharacteristic.AuraNaranja: return auraNaranjaPrefab;
            case ItemCharacteristic.AuraRoja: return auraRojaPrefab;
            case ItemCharacteristic.AuraOscura: return auraOscuraPrefab;
            case ItemCharacteristic.RunasBenignasVisibles: return runaBenignaPrefab;
            case ItemCharacteristic.RunasMalignas: return runaMalignaPrefab;
            case ItemCharacteristic.RunasInvocacion: return runaInvocacionPrefab;
            case ItemCharacteristic.RunasDefensivas: return runaDefensivaPrefab;
            case ItemCharacteristic.SonidoArcanoNormal: return sonidoArcanoVisualPrefab;
            case ItemCharacteristic.SonidoRitmico: return sonidoRitmicoVisualPrefab;
            case ItemCharacteristic.VocesEspectrales: return vocesEspectralesVisualPrefab;
            case ItemCharacteristic.VocesDemoniacas: return vocesDemoniacasVisualPrefab;
            default: return null;
        }
    }

    private void ClearEffects()
    {
        foreach (var kvp in effectMap)
            if (kvp.Value != null) Destroy(kvp.Value);
        effectMap.Clear();
    }

    private void OnDestroy() { ClearEffects(); }
}
