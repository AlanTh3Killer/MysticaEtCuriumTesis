using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrustSystem : MonoBehaviour
{
    [Header("Configuraci�n de puntos")]
    [SerializeField] private int puntosActuales = 0;
    [SerializeField] private int puntosPorAcierto = 25;
    [SerializeField] private int puntosPorError = 15;

    [Header("Umbrales de nivel (editable)")]
    [SerializeField] private int umbralNovato = 0;
    [SerializeField] private int umbralAprendiz = 100;
    [SerializeField] private int umbralCompetente = 250;
    [SerializeField] private int umbralRedimido = 500;

    [Header("UI (Placeholder)")]
    [SerializeField] private Slider barraConfianza;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    [SerializeField] private TextMeshProUGUI textoNivel;
    [SerializeField] private TextMeshProUGUI textoAciertos;
    [SerializeField] private TextMeshProUGUI textoErrores;
    [SerializeField] private TextMeshProUGUI textoBalance;
    [SerializeField] private TextMeshProUGUI textoValorAcierto;
    [SerializeField] private TextMeshProUGUI textoValorFallo;

    [Header("Panel de puntuaci�n")]
    [SerializeField] private GameObject panelPuntuacion;
    [SerializeField] private float tiempoVisible = 5f;

    private int totalAciertos = 0;
    private int totalErrores = 0;
    private bool mostrandoPanel = false;

    public enum NivelConfianza { Novato, Aprendiz, Competente, Redimido }
    public NivelConfianza nivelActual { get; private set; } = NivelConfianza.Novato;

    private void Start()
    {
        if (panelPuntuacion != null)
            panelPuntuacion.SetActive(false);

        ActualizarNivelDesdePuntos();
        ActualizarUI();
        Debug.Log($"[TrustSystem] Start -> Nivel: {nivelActual}, Puntos: {puntosActuales}");
    }

    private bool firstCorrectTriggered = false;

    public void RegistrarAcierto()
    {
        totalAciertos++;
        puntosActuales += puntosPorAcierto;
        if (puntosActuales < 0) puntosActuales = 0;

        ActualizarNivelDesdePuntos();
        ActualizarUI();

        // ✅ FIX: Solo llamar a UNO u OTRO, no ambos
        if (!firstCorrectTriggered)
        {
            FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyFirstCorrect();
            firstCorrectTriggered = true;
        }
        else
        {
            FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyCorrect();
        }

        Debug.Log("[TrustSystem] Acierto registrado");
    }

    public void RegistrarError()
    {
        FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyError();

        totalErrores++;
        puntosActuales -= puntosPorError;
        if (puntosActuales < 0) puntosActuales = 0;

        ActualizarNivelDesdePuntos();
        ActualizarUI();

        Debug.Log($"[TrustSystem] Error -{puntosPorError} -> Puntos: {puntosActuales} | Nivel: {nivelActual} | Errores: {totalErrores}");
    }

    private void ActualizarNivelDesdePuntos()
    {
        NivelConfianza nuevoNivel = nivelActual;

        if (puntosActuales >= umbralRedimido)
            nuevoNivel = NivelConfianza.Redimido;
        else if (puntosActuales >= umbralCompetente)
            nuevoNivel = NivelConfianza.Competente;
        else if (puntosActuales >= umbralAprendiz)
            nuevoNivel = NivelConfianza.Aprendiz;
        else
            nuevoNivel = NivelConfianza.Novato;

        if (nuevoNivel != nivelActual)
        {
            nivelActual = nuevoNivel;
            Debug.Log($"[TrustSystem] CambioNivel -> Ahora: {nivelActual}");
        }
    }

    private void ActualizarUI()
    {
        if (textoPuntos != null) textoPuntos.text = $"Puntos totales: {puntosActuales}";
        if (textoNivel != null) textoNivel.text = $"Nivel: {nivelActual}";
        if (textoAciertos != null) textoAciertos.text = $"Aciertos: {totalAciertos}";
        if (textoErrores != null) textoErrores.text = $"Errores: {totalErrores}";

        // Calcular valores totales
        int totalGanado = totalAciertos * puntosPorAcierto;
        int totalPerdido = totalErrores * puntosPorError;
        int balance = totalGanado - totalPerdido;

        if (textoBalance != null)
            textoBalance.text = $"Balance diario: {balance}";

        //  Aqu� est� el cambio importante:
        // Mostrar el valor TOTAL ganado y perdido
        if (textoValorAcierto != null)
            textoValorAcierto.text = $"+{totalGanado}";
        if (textoValorFallo != null)
            textoValorFallo.text = $"-{totalPerdido}";

        if (barraConfianza != null)
        {
            float progreso = CalcularProgresoNormalizado();
            barraConfianza.value = Mathf.Clamp01(progreso);
        }
    }

    private float CalcularProgresoNormalizado()
    {
        int min = 0;
        int max = 1;

        switch (nivelActual)
        {
            case NivelConfianza.Novato:
                min = umbralNovato;
                max = umbralAprendiz;
                break;
            case NivelConfianza.Aprendiz:
                min = umbralAprendiz;
                max = umbralCompetente;
                break;
            case NivelConfianza.Competente:
                min = umbralCompetente;
                max = umbralRedimido;
                break;
            case NivelConfianza.Redimido:
                return 1f;
        }

        if (max - min <= 0) return 1f;

        float normalized = (float)(puntosActuales - min) / (float)(max - min);
        return normalized;
    }

    public void MostrarPanelFinal()
    {
        if (panelPuntuacion == null || mostrandoPanel) return;

        // Buscar y desactivar scripts del jugador
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        PlayerCameraController pc = FindFirstObjectByType<PlayerCameraController>();
        ItemInteraction ii = FindFirstObjectByType<ItemInteraction>();

        if (pm != null) pm.enabled = false;
        if (pc != null) pc.enabled = false;
        if (ii != null) ii.enabled = false;

        Debug.Log("[TrustSystem] Jugador bloqueado. Mostrando panel de puntuaci�n...");

        ActualizarNivelDesdePuntos();
        ActualizarUI();
        StartCoroutine(MostrarPanelTemporal(pm, pc, ii));
    }

    private IEnumerator MostrarPanelTemporal(PlayerMovement pm, PlayerCameraController pc, ItemInteraction ii)
    {
        mostrandoPanel = true;
        panelPuntuacion.SetActive(true);

        yield return new WaitForSecondsRealtime(tiempoVisible);

        // Reactivar control del jugador
        if (pm != null) pm.enabled = true;
        if (pc != null) pc.enabled = true;
        if (ii != null) ii.enabled = true;

        panelPuntuacion.SetActive(false);
        mostrandoPanel = false;

        Debug.Log("[TrustSystem] Panel ocultado. Jugador desbloqueado.");
    }

    public int ObtenerPuntosActuales() => puntosActuales;
    public int ObtenerAciertos() => totalAciertos;
    public int ObtenerErrores() => totalErrores;
    public NivelConfianza ObtenerNivel() => nivelActual;
}
