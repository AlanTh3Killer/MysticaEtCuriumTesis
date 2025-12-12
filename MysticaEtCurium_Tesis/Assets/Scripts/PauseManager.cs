using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool JuegoPausado = false;

    [Header("Referencias")]
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private ItemInteraction itemInteraction;
    [SerializeField] private DialogueSystem dialogueSystem;

    private bool dialogoActivoAntesDePausa = false;

    private void Start()
    {
        if (menuPausa != null)
            menuPausa.SetActive(false);

        // ✅ FIX: Asegurar cursor desbloqueado si estamos en menú
        if (SceneManager.GetActiveScene().name.Contains("Menu") ||
            SceneManager.GetActiveScene().name.Contains("MainMenu"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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

        if (dialogueSystem != null)
        {
            dialogoActivoAntesDePausa = dialogueSystem.dialoguePanel.activeSelf;
            if (dialogoActivoAntesDePausa)
                dialogueSystem.dialoguePanel.SetActive(false);
        }

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerCameraController != null) playerCameraController.enabled = false;
        if (itemInteraction != null) itemInteraction.enabled = false;
        if (dayNightCycle != null) dayNightCycle.enabled = false;

        // ✅ FIX: Desbloquear cursor INMEDIATAMENTE
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;

        if (menuPausa != null)
            menuPausa.SetActive(false);

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerCameraController != null) playerCameraController.enabled = true;
        if (itemInteraction != null) itemInteraction.enabled = true;
        if (dayNightCycle != null) dayNightCycle.enabled = true;

        if (dialogueSystem != null && dialogoActivoAntesDePausa)
        {
            dialogueSystem.dialoguePanel.SetActive(true);
            dialogoActivoAntesDePausa = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ✅ NUEVO: Método para salir al menú principal
    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f; // ← CRÍTICO: Restaurar tiempo
        JuegoPausado = false;

        // Desbloquear cursor ANTES de cambiar escena
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu"); // ← Cambia por el nombre de tu escena de menú
    }
}
