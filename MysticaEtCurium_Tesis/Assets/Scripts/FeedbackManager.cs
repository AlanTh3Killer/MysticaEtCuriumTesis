using UnityEngine;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Prefabs de Feedback")]
    public GameObject correctParticlesPrefab;      // Partículas verdes
    public GameObject incorrectParticlesPrefab;    // Partículas rojas
    public GameObject discoveryParticlesPrefab;    // Partículas doradas (cuando descubres característica)
    public GameObject spawnParticlesPrefab;        // Partículas azules (cuando aparece objeto nuevo)

    [Header("Audio")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip discoverySound;
    public AudioClip spawnSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Feedback de clasificación correcta
    public void ShowCorrectFeedback(Vector3 position)
    {
        if (correctParticlesPrefab != null)
        {
            GameObject particles = Instantiate(correctParticlesPrefab, position, Quaternion.identity);
            Destroy(particles, 3f);
        }

        if (correctSound != null && audioSource != null)
            audioSource.PlayOneShot(correctSound);
    }

    // Feedback de clasificación incorrecta
    public void ShowIncorrectFeedback(Vector3 position)
    {
        if (incorrectParticlesPrefab != null)
        {
            GameObject particles = Instantiate(incorrectParticlesPrefab, position, Quaternion.identity);
            Destroy(particles, 3f);
        }

        if (incorrectSound != null && audioSource != null)
            audioSource.PlayOneShot(incorrectSound);
    }

    // Feedback de descubrimiento de característica
    public void ShowDiscoveryFeedback(Vector3 position)
    {
        if (discoveryParticlesPrefab != null)
        {
            GameObject particles = Instantiate(discoveryParticlesPrefab, position, Quaternion.identity);
            Destroy(particles, 2f);
        }

        if (discoverySound != null && audioSource != null)
            audioSource.PlayOneShot(discoverySound);
    }

    // Feedback de spawn de nuevo objeto
    public void ShowSpawnFeedback(Vector3 position)
    {
        if (spawnParticlesPrefab != null)
        {
            GameObject particles = Instantiate(spawnParticlesPrefab, position, Quaternion.identity);
            Destroy(particles, 2f);
        }

        if (spawnSound != null && audioSource != null)
            audioSource.PlayOneShot(spawnSound);
    }

    // Hacer que un objeto brille temporalmente
    public void HighlightObject(GameObject obj, float duration = 0.5f)
    {
        StartCoroutine(HighlightCoroutine(obj, duration));
    }

    private IEnumerator HighlightCoroutine(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Material[] originalMaterials = new Material[renderers.Length];

        // Guardar materiales originales
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                originalMaterials[i] = renderers[i].material;
        }

        // Aplicar emisión
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 4f, 1f);

            foreach (var renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.white * t);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restaurar materiales
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && originalMaterials[i] != null)
            {
                renderers[i].material = originalMaterials[i];
            }
        }
    }
}
