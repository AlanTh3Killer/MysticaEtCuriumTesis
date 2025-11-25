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
        Debug.Log("Player detected by container trigger");

        if (other.CompareTag("Player"))
        {
            playerInteraction = other.GetComponent<ItemInteraction>();
            if (playerInteraction != null && promptUI != null)
            {
                promptUI.SetActive(true);
                playerInRange = true;
            }
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

        if (!playerInRange) return;
        if (playerInteraction == null) return;

        // Player presses E to deposit
        if (Input.GetKeyDown(KeyCode.E))
        {
        Debug.Log("Pressed E inside container range");
            playerInteraction.DepositHeldItemIntoContainer(container);
        }
    }
}
