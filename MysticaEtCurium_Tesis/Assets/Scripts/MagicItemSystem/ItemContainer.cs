using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [Header("Clasificacion que acepta este contenedor")]
    public MagicItemDataSO.ItemClassification acceptedClassification;

    [Header("Fuerza de expulsion para objetos sin script")]
    public float ejectionForce = 5f;

    [Header("Tiempo antes de destruir items validos o invalidos")]
    public float destroyDelay = 2f;

    private TrustSystem trustSystem;

    private void Start()
    {
        trustSystem = FindObjectOfType<TrustSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto tiene el script MagicItemBehaviour
        MagicItemBehaviour item = other.GetComponent<MagicItemBehaviour>();

        if (item != null)
        {
            MagicItemDataSO data = item.data;

            if (data != null)
            {
                // Verifica si la clasificacion coincide con el contenedor
                if (data.classification == acceptedClassification)
                {
                    if (trustSystem != null)
                    {
                        trustSystem.RegistrarAcierto();
                    }
                    Debug.Log("[ItemContainer] Objeto correcto: " + data.itemName);
                }
                else
                {
                    if (trustSystem != null)
                    {
                        trustSystem.RegistrarError();
                    }
                    Debug.Log("[ItemContainer] Objeto incorrecto: " + data.itemName);
                }

                // Destruir el objeto despues de un tiempo
                StartCoroutine(DestroyAfterDelay(other.gameObject, destroyDelay));
            }
            else
            {
                Debug.LogWarning("[ItemContainer] El objeto no tiene datos asignados en MagicItemBehaviour");
            }
        }
        else
        {
            // Si no tiene MagicItemBehaviour (ejemplo: herramientas)
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 ejectionDirection = (other.transform.position - transform.position).normalized + Vector3.up * 0.5f;
                rb.AddForce(ejectionDirection * ejectionForce, ForceMode.Impulse);
                Debug.Log("[ItemContainer] Objeto sin script expulsado: " + other.name);
            }
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
