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
        Time.timeScale = 0f; // Detiene física, animaciones y deltaTime

        if (menuPausa != null)
            menuPausa.SetActive(true);

        // Desactivar control del jugador
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerCameraController != null)
            playerCameraController.enabled = false;

        // Desactivar interacción con objetos
        if (itemInteraction != null)
            itemInteraction.enabled = false;

        // Detener ciclo día/noche
        if (dayNightCycle != null)
            dayNightCycle.enabled = false;

        // Mostrar y desbloquear el cursor
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

        // Reactivar interacción con objetos
        if (itemInteraction != null)
            itemInteraction.enabled = true;

        // Reactivar ciclo día/noche
        if (dayNightCycle != null)
            dayNightCycle.enabled = true;

        // Ocultar y bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
