using UnityEngine;
using UnityEngine.UI;

public class ItemInteraction : MonoBehaviour
{
    [Header("Interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private Transform handTransform;
    [SerializeField] private GameObject pickupHintUI;

    private GameObject currentItem = null;

    void Update()
    {
        DetectItem();

        if (Input.GetKeyDown(KeyCode.E) && currentItem != null)
        {
            PickUpItem(currentItem);
        }
    }

    void DetectItem()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, itemLayer))
        {
            if (hit.collider.CompareTag("Item"))
            {
                currentItem = hit.collider.gameObject;
                if (pickupHintUI != null && !pickupHintUI.activeSelf)
                    pickupHintUI.SetActive(true);
                return;
            }
        }

        currentItem = null;
        if (pickupHintUI != null && pickupHintUI.activeSelf)
            pickupHintUI.SetActive(false);
    }

    void PickUpItem(GameObject item)
    {
        // Desactiva colision y fisicas para que no estorbe
        Collider col = item.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Lo convierte en hijo de la mano y lo posiciona
        item.transform.SetParent(handTransform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // Oculta el hint
        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
    }
}
