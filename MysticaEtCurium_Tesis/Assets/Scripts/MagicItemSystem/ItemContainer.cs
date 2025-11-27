using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [Header("Accepted classification")]
    public MagicItemDataSO.ItemClassification acceptedClassification;

    [Header("Ejection force for objects without MagicItemBehaviour")]
    public float ejectionForce = 5f;

    [Header("Destroy delay for items")]
    public float destroyDelay = 2f;

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
            if (item.data.classification == acceptedClassification)
            {
                if (trustSystem != null) trustSystem.RegistrarAcierto();
                GrimorioManager.Instance.UnlockEntry(item.data.grimorioId);
                Debug.Log("Item correcto: " + item.data.itemName);
            }
            else
            {
                if (trustSystem != null) trustSystem.RegistrarError();
                Debug.Log("Item incorrecto: " + item.data.itemName);
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

        if (data.classification == acceptedClassification)
        {
            if (trustSystem != null) trustSystem.RegistrarAcierto();
            GrimorioManager.Instance.UnlockEntry(data.grimorioId);
            Debug.Log("[ItemContainer] Correct item: " + data.itemName);
        }
        else
        {
            if (trustSystem != null) trustSystem.RegistrarError();
            Debug.Log("[ItemContainer] Incorrect item: " + data.itemName);
        }

        StartCoroutine(DestroyAfterDelay(obj, destroyDelay));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
        }

        // Notify the spawner to create a new item
        if (spawner != null)
        {
            spawner.NotifyItemDestroyed();
        }
    }
}
