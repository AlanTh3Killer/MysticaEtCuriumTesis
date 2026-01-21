using UnityEngine;

public class ThrownObjectMarker : MonoBehaviour
{
    public bool IsThrown { get; private set; } = true;

    public void StartCooldown(float duration)
    {
        Invoke("RemoveMarker", duration);
    }

    private void RemoveMarker()
    {
        IsThrown = false;
        Destroy(this); // Se autodestruye después del cooldown
    }
}