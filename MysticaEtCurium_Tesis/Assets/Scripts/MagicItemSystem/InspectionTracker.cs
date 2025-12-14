using System.Collections.Generic;
using UnityEngine;

public class InspectionTracker : MonoBehaviour
{
    public static InspectionTracker Instance { get; private set; }

    [Header("Debug")]
    public bool showDebugInfo = true;

    // Caracter�sticas descubiertas del objeto actual
    private HashSet<ItemCharacteristic> discoveredCharacteristics = new HashSet<ItemCharacteristic>();
    private MagicItemBehaviour currentInspectedItem = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Llamar cuando el jugador coloca un objeto en la mesa de inspecci�n
    public void StartInspection(MagicItemBehaviour item)
    {
        currentInspectedItem = item;
        discoveredCharacteristics.Clear();

        if (showDebugInfo)
            Debug.Log($"[InspectionTracker] Iniciando inspecci�n de: {item.data.itemName}");
    }

    // Llamar cuando una herramienta descubre caracter�sticas
    public void RegisterDiscoveredCharacteristics(List<ItemCharacteristic> characteristics)
    {
        foreach (var c in characteristics)
        {
            if (!discoveredCharacteristics.Contains(c))
            {
                discoveredCharacteristics.Add(c);
                if (showDebugInfo)
                    Debug.Log($"[InspectionTracker] ? Caracter�stica descubierta: {c}");
            }
        }
    }

    // Validar si se puede clasificar el objeto
    public bool CanClassify(out string reason)
    {
        if (currentInspectedItem == null)
        {
            reason = "No hay objeto en inspecci�n";
            return false;
        }

        // NIVEL NOVATO: M�nimo 3 caracter�sticas
        int minRequired = 3;

        // Ajustar seg�n nivel del jugador (si tienes TrustSystem)
        TrustSystem trust = FindFirstObjectByType<TrustSystem>();
        if (trust != null)
        {
            switch (trust.ObtenerNivel())
            {
                case TrustSystem.NivelConfianza.Novato:
                    minRequired = 3;
                    break;
                case TrustSystem.NivelConfianza.Aprendiz:
                    minRequired = 3;
                    break;
                case TrustSystem.NivelConfianza.Competente:
                    minRequired = 4;
                    break;
                case TrustSystem.NivelConfianza.Redimido:
                    minRequired = 5;
                    break;
            }
        }

        if (discoveredCharacteristics.Count < minRequired)
        {
            reason = $"Necesitas descubrir al menos {minRequired} caracter�sticas. Tienes: {discoveredCharacteristics.Count}";
            return false;
        }

        reason = "OK";
        return true;
    }

    // Determinar la clasificaci�n correcta bas�ndose en caracter�sticas descubiertas
    public MagicItemDataSO.ItemClassification GetSuggestedClassification()
    {
        if (currentInspectedItem == null)
            return MagicItemDataSO.ItemClassification.Vendible;

        // Contar cu�ntas caracter�sticas coinciden con cada tipo
        int vendibleCount = 0;
        int contenibleCount = 0;
        int destruibleCount = 0;

        foreach (var discovered in discoveredCharacteristics)
        {
            if (IsVendibleCharacteristic(discovered)) vendibleCount++;
            if (IsContenibleCharacteristic(discovered)) contenibleCount++;
            if (IsDestruibleCharacteristic(discovered)) destruibleCount++;
        }

        // Retornar la clasificaci�n con m�s coincidencias
        if (destruibleCount >= vendibleCount && destruibleCount >= contenibleCount)
            return MagicItemDataSO.ItemClassification.Destruible;

        if (contenibleCount >= vendibleCount)
            return MagicItemDataSO.ItemClassification.Contenible;

        return MagicItemDataSO.ItemClassification.Vendible;
    }

    // Validar si la clasificaci�n elegida es correcta seg�n lo descubierto
    public bool ValidateClassification(MagicItemDataSO.ItemClassification chosen, out int correctCount)
    {
        correctCount = 0;

        if (currentInspectedItem == null) return false;

        // Contar cu�ntas caracter�sticas correctas del tipo elegido se descubrieron
        foreach (var discovered in discoveredCharacteristics)
        {
            bool isCorrect = false;

            switch (chosen)
            {
                case MagicItemDataSO.ItemClassification.Vendible:
                    isCorrect = IsVendibleCharacteristic(discovered);
                    break;
                case MagicItemDataSO.ItemClassification.Contenible:
                    isCorrect = IsContenibleCharacteristic(discovered);
                    break;
                case MagicItemDataSO.ItemClassification.Destruible:
                    isCorrect = IsDestruibleCharacteristic(discovered);
                    break;
            }

            if (isCorrect) correctCount++;
        }

        // Necesita al menos 3 caracter�sticas correctas del tipo elegido
        return correctCount >= 3;
    }

    public MagicItemBehaviour GetCurrentItem()
    {
        return currentInspectedItem;
    }

    public int GetDiscoveredCount()
    {
        return discoveredCharacteristics.Count;
    }

    public void ClearInspection()
    {
        currentInspectedItem = null;
        discoveredCharacteristics.Clear();
    }

    // Clasificaciones seg�n tu GDD
    private bool IsVendibleCharacteristic(ItemCharacteristic c)
    {
        return c == ItemCharacteristic.RunasBenignasVisibles ||
               c == ItemCharacteristic.SonidoArcanoNormal ||
               c == ItemCharacteristic.SinRunas ||
               c == ItemCharacteristic.AuraBlanca ||
               c == ItemCharacteristic.SinAura;
    }

    private bool IsContenibleCharacteristic(ItemCharacteristic c)
    {
        return c == ItemCharacteristic.SonidoRitmico ||
               c == ItemCharacteristic.AuraNaranja ||
               c == ItemCharacteristic.RunasInvocacion ||
               c == ItemCharacteristic.RunasDefensivas ||
               c == ItemCharacteristic.EnergiaInestable;
    }

    private bool IsDestruibleCharacteristic(ItemCharacteristic c)
    {
        return c == ItemCharacteristic.VocesEspectrales ||
               c == ItemCharacteristic.VocesDemoniacas ||
               c == ItemCharacteristic.AuraRoja ||
               c == ItemCharacteristic.AuraOscura ||
               c == ItemCharacteristic.RunasMalignas ||
               c == ItemCharacteristic.LlamasDemoniacas ||
               c == ItemCharacteristic.MovimientoEspontaneo;
    }
}
