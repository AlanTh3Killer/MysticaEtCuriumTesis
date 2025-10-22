using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    private bool isPaused = false;
    public static bool DialogoActivo = false;

    [Header("Referencias UI")]
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private string nombrePersonaje = "Morgana";

    [Header("Configuración de diálogo")]
    [TextArea(3, 6)]
    public string[] lineasDialogo;

    private int indiceDialogo = 0;
    private bool dialogoActivo = false;

    // Referencias al jugador (para bloquear movimiento/cámara)
    private PlayerMovement playerMovement;
    private PlayerCameraController cameraController;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Buscar player y sus componentes
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            cameraController = player.GetComponentInChildren<PlayerCameraController>(); // más confiable
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto con tag 'Player' en la escena.");
        }
    }

    private void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.E))
        {
            if (isPaused) return; // Evita que avance el diálogo durante la pausa
            MostrarSiguienteLinea();
        }
    }

    public void IniciarDialogo()
    {
        if (dialogoActivo) return; // Evita reiniciar el diálogo si ya está activo

        if (lineasDialogo == null || lineasDialogo.Length == 0)
        {
            Debug.LogWarning("El NPC no tiene líneas de diálogo asignadas.");
            return;
        }

        // Aseguramos que empiece desde la primera línea real
        indiceDialogo = 0;
        dialogoActivo = true;
        dialoguePanel.SetActive(true);

        // Bloqueamos movimiento y cámara
        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        else Debug.LogWarning("No se encontró PlayerCameraController dentro del Player.");

        if (speakerName != null)
            speakerName.text = nombrePersonaje;

        // Mostramos la primera línea sin avanzar el índice todavía
        MostrarLineaActual();
    }

    public void PausarDialogo()
    {
        isPaused = true;
    }

    public void ReanudarDialogo()
    {
        isPaused = false;
    }

    private void MostrarLineaActual()
    {
        if (indiceDialogo < lineasDialogo.Length)
        {
            if (dialogueText != null)
                dialogueText.text = lineasDialogo[indiceDialogo];
        }
    }

    void MostrarSiguienteLinea()
    {
        // Avanzamos solo cuando el jugador presiona E nuevamente
        indiceDialogo++;

        if (indiceDialogo < lineasDialogo.Length)
        {
            MostrarLineaActual();
        }
        else
        {
            TerminarDialogo();
        }
    }

    void TerminarDialogo()
    {
        dialogoActivo = false;
        dialoguePanel.SetActive(false);

        // Restauramos movimiento y cámara
        if (playerMovement != null) playerMovement.enabled = true;
        if (cameraController != null) cameraController.enabled = true;

        Debug.Log("Diálogo terminado.");
    }
}
