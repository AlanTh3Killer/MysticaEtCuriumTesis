using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrimorioManager : MonoBehaviour
{
    public static GrimorioManager Instance { get; private set; }

    [Header("Panel Principal del Grimorio")]
    public GameObject grimorioPanel;

    [Header("UI de p�gina")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemClassificationText;
    public TextMeshProUGUI characteristicsText;
    public TextMeshProUGUI pageNumberText;

    [Header("Objetos descubiertos")]
    private System.Collections.Generic.List<MagicItemDataSO> discoveredItems = new System.Collections.Generic.List<MagicItemDataSO>();
    private int currentIndex = 0;

    private PlayerMovement playerMovement;
    private PlayerCameraController playerCamera;
    private ItemInteraction itemInteraction;
    private bool grimorioActivo = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        grimorioPanel.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerCamera = player.GetComponentInChildren<PlayerCameraController>();
            itemInteraction = player.GetComponent<ItemInteraction>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!grimorioActivo)
                AbrirGrimorio();
            else
                CerrarGrimorio();
        }
    }

    void AbrirGrimorio()
    {
        grimorioPanel.SetActive(true);
        grimorioActivo = true;

        if (playerMovement) playerMovement.enabled = false;
        if (playerCamera) playerCamera.enabled = false;
        if (itemInteraction) itemInteraction.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowCurrentPage();
    }

    void CerrarGrimorio()
    {
        grimorioPanel.SetActive(false);
        grimorioActivo = false;

        if (playerMovement) playerMovement.enabled = true;
        if (playerCamera) playerCamera.enabled = true;
        if (itemInteraction) itemInteraction.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // M�TODOS P�BLICOS PARA TU SISTEMA DE BOTONES
    public void GrimorioNextPage()
    {
        if (discoveredItems.Count == 0) return;

        currentIndex++;
        if (currentIndex >= discoveredItems.Count)
            currentIndex = 0;

        ShowCurrentPage();
    }

    public void GrimorioPreviousPage()
    {
        if (discoveredItems.Count == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = discoveredItems.Count - 1;

        ShowCurrentPage();
    }

    void ShowCurrentPage()
    {
        if (discoveredItems.Count == 0)
        {
            if (itemNameText != null) itemNameText.text = "Grimorio Vac�o";
            if (itemDescriptionText != null) itemDescriptionText.text = "Clasifica objetos correctamente para desbloquear entradas.";
            if (itemClassificationText != null) itemClassificationText.text = "";
            if (characteristicsText != null) characteristicsText.text = "";
            if (pageNumberText != null) pageNumberText.text = "0 / 0";
            return;
        }

        MagicItemDataSO item = discoveredItems[currentIndex];

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        if (itemClassificationText != null)
            itemClassificationText.text = $"<b>Clasificaci�n:</b> {item.classification}";

        // Mostrar caracter�sticas
        if (characteristicsText != null)
        {
            string chars = "<b>Caracter�sticas identificables:</b>\n\n";
            foreach (var c in item.characteristics)
            {
                chars += $"� {GetCharacteristicName(c)}\n";
            }
            characteristicsText.text = chars;
        }

        if (pageNumberText != null)
            pageNumberText.text = $"{currentIndex + 1} / {discoveredItems.Count}";
    }

    // M�todo p�blico para desbloquear entrada
    public void UnlockEntry(MagicItemDataSO item)
    {
        if (item == null) return;

        if (!discoveredItems.Contains(item))
        {
            discoveredItems.Add(item);
            Debug.Log($"[Grimorio] Entrada desbloqueada: {item.itemName}");
        }
    }

    // Sobrecarga para mantener compatibilidad con c�digo existente
    public void UnlockEntry(int grimorioId)
    {
        MagicItemDataSO[] allItems = Resources.LoadAll<MagicItemDataSO>("");

        foreach (var item in allItems)
        {
            if (item.grimorioId == grimorioId)
            {
                UnlockEntry(item);
                return;
            }
        }

        Debug.LogWarning($"[Grimorio] No se encontr� item con grimorioId: {grimorioId}");
    }

    private string GetCharacteristicName(ItemCharacteristic c)
    {
        switch (c)
        {
            case ItemCharacteristic.RunasBenignasVisibles: return "Runas benignas visibles";
            case ItemCharacteristic.SonidoArcanoNormal: return "Sonido arcano normal";
            case ItemCharacteristic.SinRunas: return "Sin runas";
            case ItemCharacteristic.AuraBlanca: return "Aura blanca";
            case ItemCharacteristic.SinAura: return "Sin aura";
            case ItemCharacteristic.SonidoRitmico: return "Sonido r�tmico";
            case ItemCharacteristic.AuraNaranja: return "Aura naranja";
            case ItemCharacteristic.RunasInvocacion: return "Runas de invocaci�n";
            case ItemCharacteristic.RunasDefensivas: return "Runas defensivas";
            case ItemCharacteristic.EnergiaInestable: return "Energ�a inestable";
            case ItemCharacteristic.VocesEspectrales: return "Voces espectrales";
            case ItemCharacteristic.VocesDemoniacas: return "Voces demon�acas";
            case ItemCharacteristic.AuraRoja: return "Aura roja";
            case ItemCharacteristic.AuraOscura: return "Aura oscura";
            case ItemCharacteristic.RunasMalignas: return "Runas malignas";
            case ItemCharacteristic.LlamasDemoniacas: return "Llamas demon�acas";
            case ItemCharacteristic.MovimientoEspontaneo: return "Movimiento espont�neo";
            default: return c.ToString();
        }
    }
}
