using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    public DialogueSystem dialogue;

    public void PlayDialogue()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("No hay DialogueSystem asignado en el trigger.");
            return;
        }

        if (!DialogueSystem.DialogoActivo)
            dialogue.IniciarDialogo();
    }

    // ====== MÉTODOS QUE ESPERAN OTROS SCRIPTS ======

    public void NotifyCorrect()
    {
        Debug.Log("[SimpleDialogueTrigger] Correcto detectado");
        PlayDialogue();
    }

    public void NotifyError()
    {
        Debug.Log("[SimpleDialogueTrigger] Error detectado");
        PlayDialogue();
    }

    public void NotifyInspect()
    {
        Debug.Log("[SimpleDialogueTrigger] Inspección iniciada");
        PlayDialogue();
    }
}
