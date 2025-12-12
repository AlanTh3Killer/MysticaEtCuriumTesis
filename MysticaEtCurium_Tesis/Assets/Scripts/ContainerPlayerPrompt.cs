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
                UpdatePromptVisibility();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //  FIX: Actualizar constantemente mientras el jugador está en rango
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

    private void Update()
    {
        if (!playerInRange || playerInteraction == null) return;

        // Player presses E to deposit
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInteraction.DepositHeldItemIntoContainer(container);
        }
    }

    //  NUEVO: Mostrar prompt solo si tiene objeto en mano
    private void UpdatePromptVisibility()
    {
        if (promptUI == null || playerInteraction == null) return;

        bool hasItem = playerInteraction.HasItemInRightHand();
        promptUI.SetActive(hasItem);
    }
}
