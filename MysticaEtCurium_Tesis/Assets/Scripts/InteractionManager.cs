using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    //public static InteractionManager Instance;

    //private void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else Destroy(gameObject);
    //}

    //public void InteractWith(GameObject target)
    //{
    //    if (target == null) return;

    //    // Detecta el tipo de interacci�n por tag o componente
    //    if (target.CompareTag("NPC"))
    //    {
    //        DialogueSystem dialogue = target.GetComponent<DialogueSystem>();
    //        if (dialogue != null)
    //            dialogue.StartDialogue();
    //    }
    //    else if (target.CompareTag("Item") || target.CompareTag("Herramienta"))
    //    {
    //        // Aqu� simplemente llama a tu ItemInteraction actual
    //        ItemInteraction item = FindFirstObjectByType<ItemInteraction>();
    //        if (item != null)
    //            item.HandleItemInteraction(target);
    //    }
    //    else
    //    {
    //        Debug.Log("No hay interacci�n definida para este objeto.");
    //    }
    //}
}
