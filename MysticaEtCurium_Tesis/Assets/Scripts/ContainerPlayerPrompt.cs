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

        //// DEBUG: identificar dónde está este script
        //Debug.Log($"[ContainerPlayerPrompt] Activo en: {gameObject.name} | Container: {(container != null ? container.name : "NULL")} | PromptUI: {(promptUI != null ? promptUI.name : "NULL")}", gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ContainerPlayerPrompt] TriggerEnter con: {other.gameObject.name}, tag: {other.tag}", gameObject);

        if (other.CompareTag("Player"))
        {
            playerInteraction = other.GetComponent<ItemInteraction>();
            Debug.Log($"[ContainerPlayerPrompt] Player detectado. ItemInteraction encontrado: {playerInteraction != null}", gameObject);
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

        bool hasItem = playerInteraction.HasItemInRightHand();
        Debug.Log($"[ContainerPlayerPrompt] Update - playerInRange: {playerInRange}, hasItem: {hasItem}, promptActivo: {promptUI.activeSelf}", gameObject); // ← temporal
        if (promptUI != null)
            promptUI.SetActive(hasItem);

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
