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

    private TrustSystem trustSystem;

    private void Start()
    {
        // Buscar el TrustSystem en escena al iniciar
        trustSystem = FindObjectOfType<TrustSystem>();

        if (trustSystem == null)
            Debug.LogWarning("[ItemContainer] No se encontró el TrustSystem en la escena. No se registrarán puntos.");
    }

    // Este método lo llamará el hijo (ContainerTrigger) cuando reciba un OnTriggerEnter
    public void OnObjectEntered(Collider other)
    {
        MagicItemBehaviour itemBehaviour = other.GetComponent<MagicItemBehaviour>();
        if (itemBehaviour == null)
        {
            Debug.Log($"[ItemContainer:{name}] '{other.name}' no tiene MagicItemBehaviour — ignorado.");
            return;
        }

        MagicItemDataSO data = itemBehaviour.data;
        if (data == null)
        {
            Debug.LogWarning($"[ItemContainer:{name}] '{other.name}' no tiene MagicItemDataSO asignado.");
            return;
        }

        bool esCorrecto = data.classification == acceptedClassification;

        if (esCorrecto)
        {
            Debug.Log($"[ItemContainer:{name}] '{data.itemName}' correctamente colocado ({data.classification}).");
            if (correctEffect) correctEffect.Play();
            if (audioSource && correctSound) audioSource.PlayOneShot(correctSound);

            // Registrar acierto
            if (trustSystem != null)
                trustSystem.RegistrarAcierto();

            // Desactivar objeto o marcarlo como procesado
            // other.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[ItemContainer:{name}] X '{data.itemName}' colocado incorrectamente ({data.classification} no es igual {acceptedClassification}).");
            if (incorrectEffect) incorrectEffect.Play();
            if (audioSource && incorrectSound) audioSource.PlayOneShot(incorrectSound);

            // Registrar error
            if (trustSystem != null)
                trustSystem.RegistrarError();
        }
    }
}
