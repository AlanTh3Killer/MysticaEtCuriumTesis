using UnityEngine;

public class ContainerPlayerPrompt : MonoBehaviour
{
    public ItemContainer container;
    public GameObject promptUI;

    private bool playerInRange = false;
    private ItemInteraction playerInteraction;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInteraction = other.GetComponent<ItemInteraction>();
            if (playerInteraction != null)
            {
                playerInRange = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            UpdatePromptVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptUI != null)
                promptUI.SetActive(false);

            playerInRange = false;
            playerInteraction = null;
        }

    }

    // ✅ FIX: Mover la lógica a Update para que se actualice constantemente
    private void Update()
    {
        if (!playerInRange || playerInteraction == null)
        {
            if (promptUI != null && promptUI.activeSelf)
                promptUI.SetActive(false);
            return;
        }

        // Actualizar visibilidad del prompt constantemente
        bool hasItem = playerInteraction.HasItemInRightHand();
        if (promptUI != null)
            promptUI.SetActive(hasItem);

        // Detectar input para depositar
        if (Input.GetKeyDown(KeyCode.E) && hasItem)
        {
            playerInteraction.DepositHeldItemIntoContainer(container);
        }
    }

    private void UpdatePromptVisibility()
    {
        if (promptUI == null)
        {
            Debug.LogError("[Container] promptUI es NULL en UpdatePromptVisibility!");
            return;
        }

        if (playerInteraction == null)
        {
            Debug.LogError("[Container] playerInteraction es NULL en UpdatePromptVisibility!");
            return;
        }

        bool hasItem = playerInteraction.HasItemInRightHand();

        Debug.Log($"[Container] UpdatePromptVisibility - HasItem: {hasItem}, UI activo antes: {promptUI.activeSelf}");

        promptUI.SetActive(hasItem);

        Debug.Log($"[Container] UI activo después: {promptUI.activeSelf}");
    }
}
