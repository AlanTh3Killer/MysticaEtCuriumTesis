using UnityEngine;

public class SimpleTutorialDirector : MonoBehaviour
{
    public SimpleDialogueTrigger dialogueWhenInspect;

    public void NotifyInspectionStarted()
    {
        if (dialogueWhenInspect != null)
            dialogueWhenInspect.PlayDialogue();
    }
}