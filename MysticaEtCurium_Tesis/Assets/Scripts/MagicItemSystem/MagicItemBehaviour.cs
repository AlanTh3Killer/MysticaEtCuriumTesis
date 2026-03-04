using UnityEngine;
using System.Collections.Generic;

public class MagicItemBehaviour : MonoBehaviour
{
    [Header("Datos del objeto mágico")]
    public MagicItemDataSO data;

    // Progreso de inspección guardado en el objeto
    [HideInInspector]
    public HashSet<ItemCharacteristic> inspectedCharacteristics = new HashSet<ItemCharacteristic>();
    
    private void Start()
    {
        if (data != null)
        {
            Debug.Log($"Objeto cargado: {data.itemName}, tipo: {data.classification}");

            // Debug para listar características
            foreach (var c in data.characteristics)
                Debug.Log($" Característica: {c}");
        }
        else
        {
            Debug.LogWarning($"El objeto {name} no tiene asignado un MagicItemData.");
        }
    }

    // Esto se activará al inspeccionar el objeto
    public void Inspect()
    {
        Debug.Log($"Inspeccionando {data.itemName}...");

        if (data.isBurning)
            Debug.Log("El objeto está en llamas, requiere hechizo de hielo.");
        else if (data.isFrozen)
            Debug.Log("El objeto está congelado, requiere runa de fuego.");
        else if (data.hasVines)
            Debug.Log("El objeto tiene vainas, requiere tijeras mágicas.");
        else
            Debug.Log("El objeto está listo para inspección.");
    }
}
