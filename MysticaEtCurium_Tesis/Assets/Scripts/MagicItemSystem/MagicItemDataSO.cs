using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMagicItem", menuName = "Mystica/Magic Item")]
public class MagicItemDataSO : ScriptableObject
{
    public string itemName;

    [TextArea] public string description;

    public enum ItemClassification
    {
        Vendible,
        Contenible,
        Destruible
    }

    [Header("Clasificaci�n del objeto")]
    public ItemClassification classification;

    [Header("Caracter�sticas del objeto")]
    public List<ItemCharacteristic> characteristics = new List<ItemCharacteristic>();

    [Header("Condici�n previa")]
    public bool isFrozen;
    public bool isBurning;
    public bool hasVines;
}
