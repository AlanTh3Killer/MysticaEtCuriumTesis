using System.Collections;
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

    [Header("Configuraci�n de di�logo")]
    [TextArea(3, 6)]
    public string[] lineasDialogo;

    private int indiceDialogo = 0;
    private bool dialogoActivo = false;

    // Referencias al jugador (para bloquear movimiento/c�mara)
    private PlayerMovement playerMovement;
    private PlayerCameraController cameraController;
    private ItemInteraction itemInteraction;

    private void Start()
    {
        itemInteraction = FindFirstObjectByType<ItemInteraction>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Buscar player y sus componentes
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            cameraController = player.GetComponentInChildren<PlayerCameraController>(); // m�s confiable
        }
        else
        {
            Debug.LogWarning("No se encontr� el objeto con tag 'Player' en la escena.");
        }

        //IniciarDialogo();

    }

    private void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.E))
        {
            if (isPaused) return; // Evita que avance el di�logo durante la pausa
            MostrarSiguienteLinea();
        }
    }

    public void IniciarDialogo()
    {
        if (itemInteraction != null)
            itemInteraction.enabled = false;

        if (dialogoActivo) return; // Evita reiniciar el di�logo si ya est� activo

        if (lineasDialogo == null || lineasDialogo.Length == 0)
        {
            Debug.LogWarning("El NPC no tiene l�neas de di�logo asignadas.");
            return;
        }

        // Aseguramos que empiece desde la primera l�nea real
        indiceDialogo = 0;
        dialogoActivo = true;
        dialoguePanel.SetActive(true);

        // Bloqueamos movimiento y c�mara
        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        else Debug.LogWarning("No se encontr� PlayerCameraController dentro del Player.");

        if (speakerName != null)
            speakerName.text = nombrePersonaje;

        // Mostramos la primera l�nea sin avanzar el �ndice todav�a
        MostrarLineaActual();
    }

    public void IniciarDialogoConLineas(string[] nuevasLineas)
    {
        if (itemInteraction != null)
            itemInteraction.enabled = false;

        if (dialogoActivo) return;

        lineasDialogo = nuevasLineas;
        indiceDialogo = 0;
        dialogoActivo = true;
        DialogueSystem.DialogoActivo = true;

        dialoguePanel.SetActive(true);

        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        if (speakerName != null)
            speakerName.text = nombrePersonaje;

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
        DialogueSystem.DialogoActivo = false;

        dialoguePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (cameraController != null)
            cameraController.enabled = true;

        if (itemInteraction != null)
            itemInteraction.enabled = true;

        Debug.Log("Diálogo terminado. Controles restaurados.");
    }

    public void ReproducirDialogoTemporal(string[] lines)
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(true);
        StartCoroutine(RunTemporaryDialogue(lines));
    }

    private IEnumerator RunTemporaryDialogue(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            dialogueText.text = lines[i];
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        dialoguePanel.SetActive(false);
    }
}
