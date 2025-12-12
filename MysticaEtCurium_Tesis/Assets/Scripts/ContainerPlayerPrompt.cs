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
        {
            promptUI.SetActive(false);
            Debug.Log($"[Container] PromptUI inicializado: {promptUI.name}");
        }
        else
        {
            Debug.LogError("[Container] PromptUI es NULL!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Container] Trigger Enter - Objeto: {other.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Container] ¡Player detectado!");

            playerInteraction = other.GetComponent<ItemInteraction>();

            if (playerInteraction != null)
            {
                Debug.Log("[Container] ItemInteraction encontrado");
                playerInRange = true;
                UpdatePromptVisibility();
            }
            else
            {
                Debug.LogError("[Container] ItemInteraction NO encontrado en player!");
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
        Debug.Log($"[Container] Trigger Exit - Objeto: {other.name}");

        if (other.CompareTag("Player"))
        {
            if (promptUI != null)
                promptUI.SetActive(false);

            playerInRange = false;
            playerInteraction = null;
        }
    }

    private void Update()
    {
        if (!playerInRange || playerInteraction == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInteraction.HasItemInRightHand())
            {
                Debug.Log("[Container] Depositando item...");
                playerInteraction.DepositHeldItemIntoContainer(container);
            }
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
