using UnityEngine;

public class SuccessFailsTest : MonoBehaviour
{
    private TrustSystem trustSystem;

    private void Start()
    {
        trustSystem = FindObjectOfType<TrustSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            trustSystem.RegistrarAcierto();
            Debug.Log("[TEST] Se ha sumado puntuaci�n (acierto)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            trustSystem.RegistrarError();
            Debug.Log("[TEST] Se ha restado puntuaci�n (error)");
        }
    }
}
