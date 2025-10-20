using TMPro;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float dayDurationSeconds = 600f; // 10 minutos
    [SerializeField] private TMP_Text timerText;

    [Header("Main Light (Directional)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Color dayColor = new Color(1f, 0.956f, 0.839f);
    [SerializeField] private Color nightColor = new Color(0.4f, 0.55f, 0.9f);
    [SerializeField] private AnimationCurve lightIntensityCurve;

    [Header("Spotlight Settings (ventana)")]
    [SerializeField] private Light windowSpotlight;
    [SerializeField] private Color spotDayColor = new Color(1f, 0.9f, 0.7f);
    [SerializeField] private Color spotNightColor = new Color(0.3f, 0.4f, 0.8f);
    [SerializeField] private AnimationCurve spotIntensityCurve;
    [SerializeField] private float baseSpotIntensity = 150f;

    [Header("Debug Gizmo Settings")]
    [SerializeField] private float gizmoRayLength = 5f;
    [SerializeField] private Color gizmoRayColor = Color.yellow;

    private float timeRemaining;
    private bool dayActive = true;

    [SerializeField] private TrustSystem trustSystem; // Arrastra aquí el script en el inspector

    private void Start()
    {
        if (mainLight == null)
            mainLight = RenderSettings.sun;

        timeRemaining = dayDurationSeconds;

        if (lightIntensityCurve == null || lightIntensityCurve.length == 0)
        {
            lightIntensityCurve = new AnimationCurve(
                new Keyframe(0f, 1.2f),
                new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0.3f));
        }

        if (spotIntensityCurve == null || spotIntensityCurve.length == 0)
        {
            spotIntensityCurve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.7f, 0.6f),
                new Keyframe(1f, 0.3f));
        }
    }

    private void Update()
    {
        if (!dayActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndOfDay();
        }

        UpdateTimerUI();
        UpdateMainLight();
        UpdateSpotlight();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateMainLight()
    {
        if (mainLight == null) return;

        float t = 1f - (timeRemaining / dayDurationSeconds);
        mainLight.color = Color.Lerp(dayColor, nightColor, t);
        mainLight.intensity = lightIntensityCurve.Evaluate(t);
        mainLight.transform.rotation = Quaternion.Euler(Mathf.Lerp(50, -20, t), 170, 0);
    }

    private void UpdateSpotlight()
    {
        if (windowSpotlight == null) return;

        float t = 1f - (timeRemaining / dayDurationSeconds);
        windowSpotlight.color = Color.Lerp(spotDayColor, spotNightColor, t);
        windowSpotlight.intensity = baseSpotIntensity * spotIntensityCurve.Evaluate(t);
        // Mantiene la rotación del spotlight original (no se modifica en runtime)
    }

    private void EndOfDay()
    {
        dayActive = false;
        Debug.Log("El día ha terminado. Fin de jornada.");
        Debug.Log("[DayNightCycle] Día terminado, mostrando puntuación...");
        if (trustSystem != null)
            trustSystem.MostrarPanelFinal();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (windowSpotlight != null)
        {
            Gizmos.color = gizmoRayColor;
            Vector3 origin = windowSpotlight.transform.position;
            Vector3 direction = windowSpotlight.transform.forward * gizmoRayLength;

            Gizmos.DrawRay(origin, direction);
            Gizmos.DrawSphere(origin + direction, 0.05f);
        }
    }
#endif
}
