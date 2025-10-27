using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [Header("Configuración del Contenedor")]
    [Tooltip("Tipo de ítem que este contenedor acepta (Vendible, Contenible o Destruible).")]
    public MagicItemDataSO.ItemClassification acceptedClassification;

    [Header("Opcional: Efectos visuales o sonido")]
    public ParticleSystem correctEffect;
    public ParticleSystem incorrectEffect;
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip incorrectSound; 

    // Este método lo llamará el hijo (ContainerTrigger) cuando reciba un OnTriggerEnter
    public void OnObjectEntered(Collider other)
    {
        // Buscar el componente MagicItemBehaviour en el objeto entrante
        MagicItemBehaviour itemBehaviour = other.GetComponent<MagicItemBehaviour>();
        if (itemBehaviour == null)
        {
            Debug.Log($"[ItemContainer:{name}] '{other.name}' no tiene MagicItemBehaviour — ignorado.");
            return;
        }

        // Obtener la data del ScriptableObject
        MagicItemDataSO data = itemBehaviour.data;
        if (data == null)
        {
            Debug.LogWarning($"[ItemContainer:{name}] '{other.name}' no tiene MagicItemDataSO asignado en MagicItemBehaviour.");
            return;
        }

        // Comparar clasificaciones
        if (data.classification == acceptedClassification)
        {
            Debug.Log($"[ItemContainer:{name}]  '{data.itemName}' correctamente colocado. ({data.classification})");

            if (correctEffect) correctEffect.Play();
            if (audioSource && correctSound) audioSource.PlayOneShot(correctSound);

            // TODO: marcar objeto como procesado, sumar puntos, desactivar, etc.
            // Ejemplo simple: desactivar el objeto para que no vuelva a ser detectado
            // other.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[ItemContainer:{name}]  '{data.itemName}' colocado incorrectamente. ({data.classification} ? {acceptedClassification})");

            if (incorrectEffect) incorrectEffect.Play();
            if (audioSource && incorrectSound) audioSource.PlayOneShot(incorrectSound);

            // TODO: lógica de penalización o efectos negativos
        }
    }
}
