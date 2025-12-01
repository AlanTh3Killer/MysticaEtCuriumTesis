using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    public DialogueSystem dialogue;

    private bool firstCorrectShown = false;

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

    public void NotifyCorrect()
    {
        if (dialogue == null) return;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {
                "Buen trabajo.",
                "Este objeto estaba bien clasificado."
            }
        );
    }

    public void NotifyError()
    {
        if (dialogue == null) return;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {
                "No. Ese objeto era incorrecto.",
                "Presta mas atencion."
            }
        );
    }

    public void NotifyInspect()
    {
        if (dialogue == null) return;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {
                "Muy bien, inspecciona con cuidado.",
                "Usa la lupa si es necesario."
            }
        );
    }

    public void NotifyFirstCorrect()
    {
        if (firstCorrectShown) return;
        firstCorrectShown = true;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {
                "Veo que has acertado por primera vez.",
                "Bien. Sigue así."
            }
        );
    }
}
