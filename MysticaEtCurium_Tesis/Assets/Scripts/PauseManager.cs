using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool JuegoPausado = false;

    [Header("Referencias")]
    [SerializeField] private GameObject menuPausa; // Panel del menú de pausa
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private ItemInteraction itemInteraction; // NUEVO: referencia al script de interacción
    [SerializeField] private DialogueSystem dialogueSystem;

    private bool dialogoActivoAntesDePausa = false;

    private void Start()
    {
        if (menuPausa != null)
            menuPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (JuegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        JuegoPausado = true;
        Time.timeScale = 0f;

        if (menuPausa != null)
            menuPausa.SetActive(true);

        // Guardamos si había un diálogo activo
        if (dialogueSystem != null)
        {
            dialogoActivoAntesDePausa = dialogueSystem.dialoguePanel.activeSelf;
            if (dialogoActivoAntesDePausa)
                dialogueSystem.dialoguePanel.SetActive(false);
        }

        // Desactivar control del jugador
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerCameraController != null)
            playerCameraController.enabled = false;

        // Desactivar interacción
        if (itemInteraction != null)
            itemInteraction.enabled = false;

        if (dayNightCycle != null)
            dayNightCycle.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;

        if (menuPausa != null)
            menuPausa.SetActive(false);

        // Reactivar control del jugador
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerCameraController != null)
            playerCameraController.enabled = true;

        if (itemInteraction != null)
            itemInteraction.enabled = true;

        if (dayNightCycle != null)
            dayNightCycle.enabled = true;

        // Si había diálogo activo antes de pausar, lo restauramos
        if (dialogueSystem != null && dialogoActivoAntesDePausa)
        {
            dialogueSystem.dialoguePanel.SetActive(true);
            dialogoActivoAntesDePausa = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
