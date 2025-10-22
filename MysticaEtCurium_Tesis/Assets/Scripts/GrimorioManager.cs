using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrimorioManager : MonoBehaviour
{
    [Header("Panel Principal del Grimorio")]
    public GameObject grimorioPanel;

    [Header("Paginas individuales (en orden)")]
    public GameObject[] paginas; // Cada página individual (izquierda y derecha)

    [Header("Textos de número de página (izquierda y derecha)")]
    public TextMeshProUGUI leftPageNumberTxt;
    public TextMeshProUGUI rightPageNumberTxt;

    [Header("Botones de navegación")]
    public Button prevPageButton;
    public Button nextPageButton;

    [Header("Referencias del jugador")]
    private PlayerMovement playerMovement;
    private PlayerCameraController playerCamera;
    private ItemInteraction itemInteraction;

    private int paginaActual = 0; // índice base del par de páginas
    private bool grimorioActivo = false;

    void Start()
    {
        grimorioPanel.SetActive(false);

        // Configurar botones
        if (prevPageButton != null) prevPageButton.onClick.AddListener(PaginaAnterior);
        if (nextPageButton != null) nextPageButton.onClick.AddListener(SiguientePagina);

        // Buscar referencias del jugador
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
            paginaActual = Mathf.Max(0, paginas.Length - (paginas.Length % 2 == 0 ? 2 : 1));

        ActualizarPaginas();
    }

    void SiguientePagina()
    {
        paginaActual += 2;
        if (paginaActual >= paginas.Length)
            paginaActual = 0;

        ActualizarPaginas();
    }

    void ActualizarPaginas()
    {
        // Desactivar todas las páginas primero
        for (int i = 0; i < paginas.Length; i++)
        {
            paginas[i].SetActive(false);
        }

        // Activar las dos actuales (izquierda y derecha)
        paginas[paginaActual].SetActive(true);
        if (paginaActual + 1 < paginas.Length)
            paginas[paginaActual + 1].SetActive(true);

        // Actualizar textos de número de página
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
}
