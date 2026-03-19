using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GrimorioManager : MonoBehaviour
{
    public static GrimorioManager Instance { get; private set; }

    [Header("Panel Principal del Grimorio")]
    public GameObject grimorioPanel;

    [Header("Pestañas - Panels")]
    public GameObject panelGrimorio;        // contenido del grimorio
    public GameObject panelObservaciones;   // marcar características visibles
    public GameObject panelManual;          // consejos y controles

    [Header("Botones de Pestañas")]
    public Button btnPestañaGrimorio;
    public Button btnPestañaObservaciones;
    public Button btnPestañaManual;

    [Header("UI Grimorio")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemClassificationText;
    public TextMeshProUGUI characteristicsText;
    public TextMeshProUGUI pageNumberText;

    [Header("UI Observaciones")]
    public GameObject observacionesContenido;   // activo cuando hay objeto en mesa
    public GameObject observacionesSinObjeto;   // mensaje "no hay objeto en mesa"
    public Transform listaObservaciones;        // parent donde van los toggles
    public GameObject togglePrefab;             // prefab de Toggle con TMP_Text

    [Header("UI Manual")]
    public TextMeshProUGUI manualText;

    [Header("Objetos descubiertos")]
    private List<MagicItemDataSO> discoveredItems = new List<MagicItemDataSO>();
    private int currentIndex = 0;

    // Características que son visibles sin herramienta
    private static readonly List<ItemCharacteristic> caracteristicasVisibles = new List<ItemCharacteristic>
    {
        ItemCharacteristic.LlamasDemoniacas,
        ItemCharacteristic.MovimientoEspontaneo
    };

    private List<Toggle> togglesActivos = new List<Toggle>();

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

        // Configurar texto del manual
        if (manualText != null)
            manualText.text = GetTextoManual();
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
        if (PauseManager.PrioridadActual > 1) return; // pausa o panel final tienen prioridad

        PauseManager.PrioridadActual = 1;
        grimorioPanel.SetActive(true);
        grimorioActivo = true;

        if (playerMovement) playerMovement.enabled = false;
        if (playerCamera) playerCamera.enabled = false;
        if (itemInteraction) itemInteraction.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Abrir siempre en pestaña Grimorio
        MostrarPestañaGrimorio();
    }

    public void CerrarGrimorio()
    {
        PauseManager.PrioridadActual = 0;
        grimorioPanel.SetActive(false);
        grimorioActivo = false;

        if (playerMovement) playerMovement.enabled = true;
        if (playerCamera) playerCamera.enabled = true;
        if (itemInteraction) itemInteraction.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ── PESTAÑAS ──────────────────────────────────────

    public void MostrarPestañaGrimorio()
    {
        panelGrimorio.SetActive(true);
        panelObservaciones.SetActive(false);
        panelManual.SetActive(false);
        ShowCurrentPage();
    }

    public void MostrarPestañaObservaciones()
    {
        panelGrimorio.SetActive(false);
        panelObservaciones.SetActive(true);
        panelManual.SetActive(false);
        RefrescarObservaciones();
    }

    public void MostrarPestañaManual()
    {
        panelGrimorio.SetActive(false);
        panelObservaciones.SetActive(false);
        panelManual.SetActive(true);
    }

    // ── OBSERVACIONES ─────────────────────────────────

    void RefrescarObservaciones()
    {
        // Limpiar toggles anteriores
        foreach (var t in togglesActivos)
            if (t != null) Destroy(t.gameObject);
        togglesActivos.Clear();

        MagicItemBehaviour itemActual = InspectionTracker.Instance?.GetCurrentItem();

        if (itemActual == null)
        {
            if (observacionesContenido != null) observacionesContenido.SetActive(false);
            if (observacionesSinObjeto != null) observacionesSinObjeto.SetActive(true);
            return;
        }

        if (observacionesContenido != null) observacionesContenido.SetActive(true);
        if (observacionesSinObjeto != null) observacionesSinObjeto.SetActive(false);

        // Filtrar solo las características visibles que tiene el objeto
        foreach (var caracteristica in itemActual.data.characteristics)
        {
            if (!caracteristicasVisibles.Contains(caracteristica)) continue;

            // Crear toggle
            GameObject toggleGO = Instantiate(togglePrefab, listaObservaciones);
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            TMP_Text label = toggleGO.GetComponentInChildren<TMP_Text>();

            if (label != null)
                label.text = GetCharacteristicName(caracteristica);

            // Marcar si ya fue descubierta
            bool yaDescubierta = itemActual.inspectedCharacteristics.Contains(caracteristica);
            toggle.isOn = yaDescubierta;
            toggle.interactable = !yaDescubierta; // si ya está marcada, no se puede desmarcar

            // Capturar valor para el lambda
            ItemCharacteristic c = caracteristica;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    InspectionTracker.Instance?.RegisterDiscoveredCharacteristics(
                        new List<ItemCharacteristic> { c }
                    );
                    toggle.interactable = false; // bloquear después de marcar
                    Debug.Log($"[Grimorio] Característica visible registrada: {c}");
                }
            });

            togglesActivos.Add(toggle);
        }

        // Si el objeto no tiene características visibles
        if (togglesActivos.Count == 0 && observacionesSinObjeto != null)
        {
            observacionesContenido.SetActive(false);
            observacionesSinObjeto.SetActive(true);
            // Cambiar el texto para indicar que no hay nada visible
            TMP_Text msg = observacionesSinObjeto.GetComponentInChildren<TMP_Text>();
            if (msg != null) msg.text = "Este objeto no tiene características visibles a simple vista.";
        }
    }

    // ── GRIMORIO (páginas) ────────────────────────────

    public void GrimorioNextPage()
    {
        if (discoveredItems.Count == 0) return;
        currentIndex = (currentIndex + 1) % discoveredItems.Count;
        ShowCurrentPage();
    }

    public void GrimorioPreviousPage()
    {
        if (discoveredItems.Count == 0) return;
        currentIndex = (currentIndex - 1 + discoveredItems.Count) % discoveredItems.Count;
        ShowCurrentPage();
    }

    void ShowCurrentPage()
    {
        if (discoveredItems.Count == 0)
        {
            if (itemNameText != null) itemNameText.text = "Grimorio Vacío";
            if (itemDescriptionText != null) itemDescriptionText.text = "Clasifica objetos correctamente para desbloquear entradas.";
            if (itemClassificationText != null) itemClassificationText.text = "";
            if (characteristicsText != null) characteristicsText.text = "";
            if (pageNumberText != null) pageNumberText.text = "0 / 0";
            return;
        }

        MagicItemDataSO item = discoveredItems[currentIndex];

        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemDescriptionText != null) itemDescriptionText.text = item.description;
        if (itemClassificationText != null) itemClassificationText.text = $"<b>Clasificación:</b> {item.classification}";

        if (characteristicsText != null)
        {
            string chars = "<b>Características identificables:</b>\n\n";
            foreach (var c in item.characteristics)
                chars += $"• {GetCharacteristicName(c)}\n";
            characteristicsText.text = chars;
        }

        if (pageNumberText != null)
            pageNumberText.text = $"{currentIndex + 1} / {discoveredItems.Count}";
    }

    // ── MANUAL ────────────────────────────────────────

    string GetTextoManual()
    {
        return
            "<b>CONTROLES</b>\n\n" +
            "WASD — Moverse\n" +
            "Mouse — Mover cámara\n" +
            "E — Tomar / soltar objeto\n" +
            "Q — Tomar / soltar herramienta\n" +
            "F — Entrar / salir mesa de inspección\n" +
            "Click Izq — Colocar objeto en mesa\n" +
            "Click Der — Usar herramienta en objeto\n" +
            "G — Arrojar objeto o herramienta\n" +
            "Tab — Abrir / cerrar Grimorio\n\n" +
            "<b>CLASIFICACIÓN</b>\n\n" +
            "Necesitas identificar mínimo 3 características.\n" +
            "El objeto pertenece al tipo con más coincidencias.\n\n" +
            "<b>VENDIBLE</b> → Aura blanca, Sin aura, Sin runas,\n" +
            "Runas benignas, Sonido arcano\n\n" +
            "<b>CONTENIBLE</b> → Aura naranja, Energía inestable,\n" +
            "Runas de invocación, Runas defensivas, Sonido rítmico\n\n" +
            "<b>DESTRUIBLE</b> → Aura roja/oscura, Runas malignas,\n" +
            "Voces espectrales/demoníacas,\n" +
            "Llamas demoníacas, Movimiento espontáneo";
    }

    // ── UNLOCK ────────────────────────────────────────

    public void UnlockEntry(MagicItemDataSO item)
    {
        if (item == null) return;
        if (!discoveredItems.Contains(item))
        {
            discoveredItems.Add(item);
            Debug.Log($"[Grimorio] Entrada desbloqueada: {item.itemName}");
        }
    }

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
        Debug.LogWarning($"[Grimorio] No se encontró item con grimorioId: {grimorioId}");
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
            case ItemCharacteristic.SonidoRitmico: return "Sonido rítmico";
            case ItemCharacteristic.AuraNaranja: return "Aura naranja";
            case ItemCharacteristic.RunasInvocacion: return "Runas de invocación";
            case ItemCharacteristic.RunasDefensivas: return "Runas defensivas";
            case ItemCharacteristic.EnergiaInestable: return "Energía inestable";
            case ItemCharacteristic.VocesEspectrales: return "Voces espectrales";
            case ItemCharacteristic.VocesDemoniacas: return "Voces demoníacas";
            case ItemCharacteristic.AuraRoja: return "Aura roja";
            case ItemCharacteristic.AuraOscura: return "Aura oscura";
            case ItemCharacteristic.RunasMalignas: return "Runas malignas";
            case ItemCharacteristic.LlamasDemoniacas: return "Llamas demoníacas";
            case ItemCharacteristic.MovimientoEspontaneo: return "Movimiento espontáneo";
            default: return c.ToString();
        }
    }
}