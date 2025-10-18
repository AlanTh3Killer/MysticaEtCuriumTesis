using UnityEngine;

public class MagicItemBehaviour : MonoBehaviour
{
    [Header("Datos del objeto m�gico")]
    public MagicItemDataSO data;

    private void Start()
    {
        if (data != null)
        {
            Debug.Log($"Objeto cargado: {data.itemName}, tipo: {data.classification}");

            // Debug para listar caracter�sticas
            foreach (var c in data.characteristics)
                Debug.Log($" Caracter�stica: {c}");
        }
        else
        {
            Debug.LogWarning($"El objeto {name} no tiene asignado un MagicItemData.");
        }
    }

    // Esto se activar� al inspeccionar el objeto
    public void Inspect()
    {
        Debug.Log($"Inspeccionando {data.itemName}...");

        if (data.isBurning)
            Debug.Log("El objeto est� en llamas, requiere hechizo de hielo.");
        else if (data.isFrozen)
            Debug.Log("El objeto est� congelado, requiere runa de fuego.");
        else if (data.hasVines)
            Debug.Log("El objeto tiene vainas, requiere tijeras m�gicas.");
        else
            Debug.Log("El objeto est� listo para inspecci�n.");
    }
}
