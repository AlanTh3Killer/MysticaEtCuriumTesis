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

    public void IntroDialogue()
    {
        if (dialogue == null) return;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {
                "Bueno, este sera el primer dia de prueba.",
                "Se que en teoria deberias saber que es lo que hay que hacer, pero dado a los recientes acontecimientos no voy a tomar riesgos.",
                "Puedes empezar con los items del suelo",
                "Los demas iran apareciendo cuando termines con alguno de los que ya estan",
                "Una vez que tomes un item, traelo de vuelta a la mesa de inspeccion.",
                "(F cerca de mesa de inspeccion.)"            }
        );
    }
    public void NotifyCorrect()
    {
        if (dialogue == null) return;

        dialogue.IniciarDialogoConLineas(
            new string[]
            {"Veo que has acertado por primera vez.",
                "Bien. Sigue así.",
                "Me tome la molestia de darte un cuaderno que toma notas.",
                "A ver si asi dejas de cometer errores.",
                "(Tab para abrir grimorio)"
                //"Buen trabajo.",
                //"Este objeto estaba bien clasificado."
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
                "Por ahora no te voy a presionar, ya tienes la solucion en tus manos.",
                "Pero la proxima vez tendras que usar tus herramientas para descubrir por ti mismo el tipo de objeto.",
                "(Click Izquierdo en el centro de la mesa para colocar objeto en modo inspeccion)",
                "(Click Derecho mientras se sostiene una herramienta para usarla en el objeto a inspeccionar)",
                "Una vez inspeccionado, debes llevar el objeto a uno de los contenedores de allá.",
                "Colocalo en el contenedor correcto",
                "(E para tomar el objeto inspeccionado)",
                "(F para salir del modo inspeccion)"
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
                "Bien. Sigue así.",
                "Me tome la molestia de darte un cuaderno que toma notas.",
                "A ver si asi dejas de cometer errores.",
                "(Tab para abrir grimorio)"
            }
        );
    }
}
