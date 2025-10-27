using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ContainerTrigger : MonoBehaviour
{
    [Tooltip("Referencia al script ItemContainer en el padre.")]
    public ItemContainer parentContainer;

    private void Reset()
    {
        // Asegura que el collider sea trigger por defecto
        Collider col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col && !col.isTrigger)
            col.isTrigger = true;

        if (parentContainer == null)
            parentContainer = GetComponentInParent<ItemContainer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Reenvía al script del padre
        if (parentContainer != null)
            parentContainer.OnObjectEntered(other);
    }
}
