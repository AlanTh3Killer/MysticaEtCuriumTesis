using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ContainerTrigger : MonoBehaviour
{
    private ItemContainer parentContainer;

    private void Start()
    {
        parentContainer = GetComponentInParent<ItemContainer>();
        if (parentContainer == null)
        {
            Debug.LogError("[ContainerTrigger] No se encontró ItemContainer en el padre");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentContainer != null)
        {
            // Reenvía el evento al contenedor padre
            parentContainer.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
        }
    }
}
