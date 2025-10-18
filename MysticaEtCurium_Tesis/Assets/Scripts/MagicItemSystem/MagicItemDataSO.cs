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

    [Header("Clasificación del objeto")]
    public ItemClassification classification;

    [Header("Características del objeto")]
    public List<ItemCharacteristic> characteristics = new List<ItemCharacteristic>();

    [Header("Condición previa")]
    public bool isFrozen;
    public bool isBurning;
    public bool hasVines;
}
