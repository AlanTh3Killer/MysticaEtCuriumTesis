using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [Header("Accepted classification")]
    public MagicItemDataSO.ItemClassification acceptedClassification;

    [Header("Ejection force")]
    public float ejectionForce = 5f;

    [Header("Destroy delay")]
    public float destroyDelay = 2f;

    [Header("Feedback Visual")]
    public GameObject correctFeedbackVFX;   // Part�culas verdes
    public GameObject incorrectFeedbackVFX; // Part�culas rojas

    private TrustSystem trustSystem;
    private ItemSpawner spawner;

    private void Start()
    {
        spawner = FindFirstObjectByType<ItemSpawner>();
        trustSystem = FindFirstObjectByType<TrustSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        MagicItemBehaviour item = other.GetComponent<MagicItemBehaviour>();
        if (item != null)
        {
            ProcessItem(item.gameObject, item.data);
            return;
        }

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized + Vector3.up * 0.5f;
            rb.AddForce(dir * ejectionForce, ForceMode.Impulse);
            Debug.Log("[ItemContainer] Object without script ejected: " + other.name);
        }
    }

    public void ProcessItemManual(GameObject obj)
    {
        MagicItemBehaviour item = obj.GetComponent<MagicItemBehaviour>();

        if (item != null && item.data != null)
        {
            bool correct = ValidateClassification(item.data);

            if (correct)
            {
                if (trustSystem != null) trustSystem.RegistrarAcierto();

                if (GrimorioManager.Instance != null)
                    GrimorioManager.Instance.UnlockEntry(item.data);

                // ✅ USAR EL FEEDBACK MANAGER
                if (FeedbackManager.Instance != null)
                    FeedbackManager.Instance.ShowCorrectFeedback(transform.position);

                Debug.Log("✓ Item clasificado correctamente: " + item.data.itemName);
            }
            else
            {
                if (trustSystem != null) trustSystem.RegistrarError();

                // ✅ USAR EL FEEDBACK MANAGER
                if (FeedbackManager.Instance != null)
                    FeedbackManager.Instance.ShowIncorrectFeedback(transform.position);
                Debug.Log("✗ Item clasificado incorrectamente: " + item.data.itemName);
            }

            StartCoroutine(DestroyAfterDelay(obj, destroyDelay));
        }
        else
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (obj.transform.position - transform.position).normalized + Vector3.up * 0.5f;
                rb.AddForce(dir * ejectionForce, ForceMode.Impulse);
            }
            Debug.Log("Objeto sin script expulsado: " + obj.name);
        }
    }

    private void ProcessItem(GameObject obj, MagicItemDataSO data)
    {
        if (data == null)
        {
            Debug.LogWarning("[ItemContainer] Item has no data.");
            return;
        }

        bool correct = ValidateClassification(data);

        if (correct)
        {
            if (trustSystem != null) trustSystem.RegistrarAcierto();

            if (GrimorioManager.Instance != null)
                GrimorioManager.Instance.UnlockEntry(data);

            // ✅ USAR EL FEEDBACK MANAGER
            if (FeedbackManager.Instance != null)
                FeedbackManager.Instance.ShowCorrectFeedback(transform.position);

            Debug.Log("[ItemContainer] ? Correct: " + data.itemName);
        }
        else
        {
            if (trustSystem != null) trustSystem.RegistrarError();

            // ✅ USAR EL FEEDBACK MANAGER
            if (FeedbackManager.Instance != null)
                FeedbackManager.Instance.ShowIncorrectFeedback(transform.position);

            Debug.Log("[ItemContainer] ? Incorrect: " + data.itemName);
        }

        StartCoroutine(DestroyAfterDelay(obj, destroyDelay));
    }

    //  NUEVA L�GICA DE VALIDACI�N
    private bool ValidateClassification(MagicItemDataSO data)
    {
        // Si no hay tracker, validar por clasificaci�n directa (modo legacy)
        if (InspectionTracker.Instance == null)
        {
            return data.classification == acceptedClassification;
        }

        // Validar bas�ndose en caracter�sticas descubiertas
        int correctCount;
        bool isValid = InspectionTracker.Instance.ValidateClassification(acceptedClassification, out correctCount);

        Debug.Log($"[ItemContainer] Validaci�n - Contenedor: {acceptedClassification} | Correcto: {isValid} | Caracter�sticas v�lidas: {correctCount}");

        return isValid;
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
        }

        if (spawner != null)
        {
            spawner.NotifyItemDestroyed();
        }
    }
}
