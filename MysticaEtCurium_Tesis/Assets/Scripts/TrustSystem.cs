using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrustSystem : MonoBehaviour
{
    [Header("Configuración de puntos")]
    [SerializeField] private int puntosActuales = 0;
    [SerializeField] private int puntosPorAcierto = 25;
    [SerializeField] private int puntosPorError = 15;

    [Header("UI (Placeholder)")]
    [SerializeField] private Slider barraConfianza;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    [SerializeField] private TextMeshProUGUI textoNivel;
    [SerializeField] private TextMeshProUGUI textoAciertos;
    [SerializeField] private TextMeshProUGUI textoErrores;
    [SerializeField] private TextMeshProUGUI textoBalance;

    private int totalAciertos = 0;
    private int totalErrores = 0;

    public enum NivelConfianza
    {
        Novato,
        Aprendiz,
        Competente,
        Redimido
    }

    public NivelConfianza nivelActual { get; private set; } = NivelConfianza.Novato;

    [Header("Panel de puntuación")]
    [SerializeField] private GameObject panelPuntuacion;
    [SerializeField] private float tiempoVisible = 5f;
    private bool mostrandoPanel = false;

    private void Start()
    {
        ActualizarNivel();
        ActualizarUI();
        Debug.Log($"[TrustSystem] Nivel inicial: {nivelActual} | Puntos: {puntosActuales}");
    }

    // Llamar cuando el jugador clasifique correctamente
    public void SumarPuntos()
    {
        puntosActuales += puntosPorAcierto;
        totalAciertos++;
        ActualizarNivel();
        ActualizarUI();
        Debug.Log($"[TrustSystem] +{puntosPorAcierto} puntos. Total: {puntosActuales}. Nivel: {nivelActual}");
    }

    // Llamar cuando el jugador falle una clasificación
    public void RestarPuntos()
    {
        puntosActuales -= puntosPorError;
        if (puntosActuales < 0) puntosActuales = 0;
        totalErrores++;
        ActualizarNivel();
        ActualizarUI();
        Debug.Log($"[TrustSystem] -{puntosPorError} puntos. Total: {puntosActuales}. Nivel: {nivelActual}");
    }

    private void ActualizarNivel()
    {
        NivelConfianza nuevoNivel = nivelActual;

        if (puntosActuales < 100)
            nuevoNivel = NivelConfianza.Novato;
        else if (puntosActuales < 250)
            nuevoNivel = NivelConfianza.Aprendiz;
        else if (puntosActuales < 500)
            nuevoNivel = NivelConfianza.Competente;
        else
            nuevoNivel = NivelConfianza.Redimido;

        if (nuevoNivel != nivelActual)
        {
            nivelActual = nuevoNivel;
            Debug.Log($"[TrustSystem] Cambio de nivel  Ahora eres {nivelActual}");
        }
    }

    private void ActualizarUI()
    {
        // Calcular progreso de barra
        if (barraConfianza != null)
        {
            float progreso = 0f;

            switch (nivelActual)
            {
                case NivelConfianza.Novato:
                    progreso = (float)puntosActuales / 100f;
                    break;
                case NivelConfianza.Aprendiz:
                    progreso = (float)(puntosActuales - 100) / 150f;
                    break;
                case NivelConfianza.Competente:
                    progreso = (float)(puntosActuales - 250) / 250f;
                    break;
                case NivelConfianza.Redimido:
                    progreso = 1f;
                    break;
            }

            barraConfianza.value = Mathf.Clamp01(progreso);
        }

        // Actualizar textos
        if (textoPuntos != null)
            textoPuntos.text = $"Puntos totales: {puntosActuales}";

        if (textoNivel != null)
            textoNivel.text = $"Nivel: {nivelActual}";

        if (textoAciertos != null)
            textoAciertos.text = $"Aciertos: {totalAciertos} (+{puntosPorAcierto} cada uno)";

        if (textoErrores != null)
            textoErrores.text = $"Errores: {totalErrores} (-{puntosPorError} cada uno)";

        if (textoBalance != null)
        {
            int balance = (totalAciertos * puntosPorAcierto) - (totalErrores * puntosPorError);
            textoBalance.text = $"Balance diario: {balance}";
        }
    }

    // Métodos públicos
    public NivelConfianza ObtenerNivelActual() => nivelActual;
    public int ObtenerPuntosActuales() => puntosActuales;
    public int ObtenerAciertos() => totalAciertos;
    public int ObtenerErrores() => totalErrores;

    // Llamado desde el ciclo día/noche al terminar el día
    public void MostrarPanelFinal()
    {
        if (panelPuntuacion == null || mostrandoPanel) return;
        StartCoroutine(MostrarPanelTemporal());
    }

    private System.Collections.IEnumerator MostrarPanelTemporal()
    {
        mostrandoPanel = true;

        // Activar panel
        panelPuntuacion.SetActive(true);
        Debug.Log("[TrustSystem] Mostrando panel de puntuación...");

        // Esperar unos segundos
        yield return new WaitForSeconds(tiempoVisible);

        // Ocultar panel
        panelPuntuacion.SetActive(false);
        mostrandoPanel = false;
        Debug.Log("[TrustSystem] Panel ocultado. Inicia nuevo día.");
    }
}
