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

    [Header("Configuración de diálogo")]
    [TextArea(3, 6)]
    public string[] lineasDialogo;

    private int indiceDialogo = 0;
    private bool dialogoActivo = false;

    // Referencias al jugador
    private PlayerMovement playerMovement;
    private PlayerCameraController cameraController;
    private ItemInteraction itemInteraction;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Buscar player y sus componentes
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            cameraController = player.GetComponentInChildren<PlayerCameraController>();
            itemInteraction = player.GetComponent<ItemInteraction>();
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto con tag 'Player' en la escena.");
        }

        // SOLO UNA LLAMADA AL DIÁLOGO DE INTRO
        FindFirstObjectByType<SimpleDialogueTrigger>()?.IntroDialogue();
    }

    private void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.E))
        {
            if (isPaused) return;
            MostrarSiguienteLinea();
        }
    }

    public void IniciarDialogo()
    {
        if (dialogoActivo) return;

        if (lineasDialogo == null || lineasDialogo.Length == 0)
        {
            Debug.LogWarning("El NPC no tiene líneas de diálogo asignadas.");
            return;
        }

        indiceDialogo = 0;
        dialogoActivo = true;
        DialogoActivo = true;
        dialoguePanel.SetActive(true);

        // Bloquear controles
        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        if (itemInteraction != null) itemInteraction.enabled = false;

        if (speakerName != null)
            speakerName.text = nombrePersonaje;

        MostrarLineaActual();
    }

    public void IniciarDialogoConLineas(string[] nuevasLineas)
    {
        if (dialogoActivo) return;

        lineasDialogo = nuevasLineas;
        indiceDialogo = 0;
        dialogoActivo = true;
        DialogoActivo = true;

        dialoguePanel.SetActive(true);

        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        if (itemInteraction != null) itemInteraction.enabled = false;

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
        DialogoActivo = false;

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
