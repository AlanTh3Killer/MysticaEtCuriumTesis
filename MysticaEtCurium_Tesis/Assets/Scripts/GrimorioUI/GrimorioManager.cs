using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrimorioManager : MonoBehaviour
{
    public static GrimorioManager Instance { get; private set; }

    public GrimorioContentManager contentManager;

    [Header("Panel Principal del Grimorio")]
    public GameObject grimorioPanel;

    [Header("Paginas individuales (en orden)")]
    public GameObject[] paginas;

    [Header("Textos de numero de pagina (izquierda y derecha)")]
    public TextMeshProUGUI leftPageNumberTxt;
    public TextMeshProUGUI rightPageNumberTxt;

    [Header("Botones de navegacion")]
    public Button prevPageButton;
    public Button nextPageButton;

    [Header("Referencias del jugador")]
    private PlayerMovement playerMovement;
    private PlayerCameraController playerCamera;
    private ItemInteraction itemInteraction;

    private int paginaActual = 0;
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

        if (prevPageButton != null) prevPageButton.onClick.AddListener(PaginaAnterior);
        if (nextPageButton != null) nextPageButton.onClick.AddListener(SiguientePagina);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerCamera = player.GetComponentInChildren<PlayerCameraController>();
            itemInteraction = player.GetComponent<ItemInteraction>();
        }

        ActualizarPaginas();
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
        if (contentManager != null)
        {
            contentManager.ShowCurrentPages();
        }

        grimorioPanel.SetActive(true);
        grimorioActivo = true;

        if (playerMovement) playerMovement.enabled = false;
        if (playerCamera) playerCamera.enabled = false;
        if (itemInteraction) itemInteraction.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    void PaginaAnterior()
    {
        paginaActual -= 2;
        if (paginaActual < 0)
            paginaActual = Mathf.Max(0, paginas.Length - 2);

        ActualizarPaginas();
        if (contentManager != null)
            contentManager.PreviousPage();
    }

    void SiguientePagina()
    {
        paginaActual += 2;
        if (paginaActual >= paginas.Length)
            paginaActual = 0;

        ActualizarPaginas();
        if (contentManager != null)
            contentManager.NextPage();
    }

    void ActualizarPaginas()
    {
        for (int i = 0; i < paginas.Length; i++)
        {
            paginas[i].SetActive(false);
        }

        paginas[paginaActual].SetActive(true);
        if (paginaActual + 1 < paginas.Length)
            paginas[paginaActual + 1].SetActive(true);

        if (leftPageNumberTxt != null)
            leftPageNumberTxt.text = (paginaActual + 1).ToString();

        if (rightPageNumberTxt != null)
        {
            if (paginaActual + 1 < paginas.Length)
                rightPageNumberTxt.text = (paginaActual + 2).ToString();
            else
                rightPageNumberTxt.text = "-";
        }
    }

    // Metodo de ejemplo (puedes personalizarlo)
    public void UnlockEntry(int id)
    {
        if (contentManager == null)
        {
            Debug.LogWarning("No hay ContentManager asignado en GrimorioManager.");
            return;
        }

        MagicItemDataSO newEntry = FindEntryById(id);

        if (newEntry != null)
        {
            contentManager.AddDiscoveredItem(newEntry);
            contentManager.ShowCurrentPages();
            Debug.Log("Entrada desbloqueada: " + newEntry.itemName);
        }
        else
        {
            Debug.LogWarning("No se encontró una entrada de grimorio con ID: " + id);
        }
    }

    private MagicItemDataSO FindEntryById(int id)
    {
        MagicItemDataSO[] allItems = Resources.LoadAll<MagicItemDataSO>("");

        foreach (var item in allItems)
        {
            if (item.grimorioId == id)
                return item;
        }

        return null;
    }
}
