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
            trustSystem.SumarPuntos();
            Debug.Log("[TEST] Se ha sumado puntuación (acierto)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            trustSystem.RestarPuntos();
            Debug.Log("[TEST] Se ha restado puntuación (error)");
        }
    }
}
