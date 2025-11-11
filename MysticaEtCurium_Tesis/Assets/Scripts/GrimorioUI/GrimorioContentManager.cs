using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GrimorioContentManager : MonoBehaviour
{
    [Header("Referencias a las dos páginas visibles")]
    public GameObject leftPage;
    public GameObject rightPage;

    [Header("Texto de página izquierda")]
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI leftDescriptionText;
    public TextMeshProUGUI leftClassificationText;
    public TextMeshProUGUI leftPageNumberText;

    [Header("Texto de página derecha")]
    public TextMeshProUGUI rightNameText;
    public TextMeshProUGUI rightDescriptionText;
    public TextMeshProUGUI rightClassificationText;
    public TextMeshProUGUI rightPageNumberText;

    [Header("Datos de objetos descubiertos")]
    public List<MagicItemDataSO> discoveredItems = new List<MagicItemDataSO>();

    private int currentPageIndex = 0; // índice del primer objeto de las dos páginas

    public void ShowCurrentPages()
    {
        // Limpiar texto por si hay menos objetos que páginas
        ClearPage(leftNameText, leftDescriptionText, leftClassificationText, leftPageNumberText);
        ClearPage(rightNameText, rightDescriptionText, rightClassificationText, rightPageNumberText);

        if (discoveredItems.Count == 0)
        {
            leftNameText.text = "Sin objetos descubiertos";
            return;
        }

        // Página izquierda
        if (currentPageIndex < discoveredItems.Count)
            DisplayItem(discoveredItems[currentPageIndex], leftNameText, leftDescriptionText, leftClassificationText, leftPageNumberText, currentPageIndex + 1);

        // Página derecha
        if (currentPageIndex + 1 < discoveredItems.Count)
            DisplayItem(discoveredItems[currentPageIndex + 1], rightNameText, rightDescriptionText, rightClassificationText, rightPageNumberText, currentPageIndex + 2);
    }

    public void NextPage()
    {
        if (currentPageIndex + 2 < discoveredItems.Count)
        {
            currentPageIndex += 2;
            ShowCurrentPages();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex - 2 >= 0)
        {
            currentPageIndex -= 2;
            ShowCurrentPages();
        }
    }

    public void AddDiscoveredItem(MagicItemDataSO newItem)
    {
        if (!discoveredItems.Contains(newItem))
        {
            discoveredItems.Add(newItem);
            Debug.Log("[GrimorioContentManager] Nuevo objeto agregado: " + newItem.itemName);
        }
    }

    private void DisplayItem(MagicItemDataSO item, TextMeshProUGUI nameText, TextMeshProUGUI descText, TextMeshProUGUI classText, TextMeshProUGUI pageText, int pageNumber)
    {
        nameText.text = item.itemName;
        descText.text = item.description;
        classText.text = "Clasificación: " + item.classification.ToString();
        pageText.text = "Página " + pageNumber;
    }

    private void ClearPage(TextMeshProUGUI nameText, TextMeshProUGUI descText, TextMeshProUGUI classText, TextMeshProUGUI pageText)
    {
        nameText.text = "";
        descText.text = "";
        classText.text = "";
        pageText.text = "";
    }
}
