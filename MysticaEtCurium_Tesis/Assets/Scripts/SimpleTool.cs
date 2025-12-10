using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SimpleTool : MonoBehaviour
{
    [Header("Configuración de herramienta")]
    public string toolName = "Herramienta";
    public List<ItemCharacteristic> canDetect = new List<ItemCharacteristic>();

    [Header("Referencias")]
    public GameObject detectionPanel;
    public TextMeshProUGUI detectionText;

    [Header("Audio (Opcional)")]
    public AudioClip discoverySound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UseToolOnObject(MagicItemBehaviour item)
    {
        if (item == null || item.data == null)
        {
            ShowMessage("No hay objeto para inspeccionar");
            return;
        }

        // Buscar características que esta herramienta puede detectar
        List<ItemCharacteristic> detected = new List<ItemCharacteristic>();

        foreach (var characteristic in item.data.characteristics)
        {
            if (canDetect.Contains(characteristic))
            {
                detected.Add(characteristic);
            }
        }

        //  REGISTRAR EN EL TRACKER
        if (detected.Count > 0 && InspectionTracker.Instance != null)
        {
            InspectionTracker.Instance.RegisterDiscoveredCharacteristics(detected);
        }

        // Mostrar resultado
        if (detected.Count > 0)
        {
            string message = $"<b>{toolName} detectó:</b>\n\n";
            foreach (var c in detected)
            {
                message += $"? {GetCharacteristicName(c)}\n";
            }
            ShowMessage(message);

            // Sonido de descubrimiento
            if (audioSource != null && discoverySound != null)
                audioSource.PlayOneShot(discoverySound);
        }
        else
        {
            ShowMessage($"{toolName}: No se detectó nada relevante");
        }
    }

    private void ShowMessage(string msg)
    {
        if (detectionPanel != null)
        {
            detectionPanel.SetActive(true);
            if (detectionText != null)
                detectionText.text = msg;

            CancelInvoke("HideMessage");
            Invoke("HideMessage", 3f);
        }
    }

    private void HideMessage()
    {
        if (detectionPanel != null)
            detectionPanel.SetActive(false);
    }

    private string GetCharacteristicName(ItemCharacteristic c)
    {
        switch (c)
        {
            case ItemCharacteristic.RunasBenignasVisibles: return "Runas benignas visibles";
            case ItemCharacteristic.SonidoArcanoNormal: return "Sonido arcano normal";
            case ItemCharacteristic.SinRunas: return "Sin runas";
            case ItemCharacteristic.AuraBlanca: return "Aura blanca";
            case ItemCharacteristic.SinAura: return "Sin aura";
            case ItemCharacteristic.SonidoRitmico: return "Sonido rítmico";
            case ItemCharacteristic.AuraNaranja: return "Aura naranja";
            case ItemCharacteristic.RunasInvocacion: return "Runas de invocación";
            case ItemCharacteristic.RunasDefensivas: return "Runas defensivas";
            case ItemCharacteristic.EnergiaInestable: return "Energía inestable";
            case ItemCharacteristic.VocesEspectrales: return "Voces espectrales";
            case ItemCharacteristic.VocesDemoníacas: return "Voces demoníacas";
            case ItemCharacteristic.AuraRoja: return "Aura roja";
            case ItemCharacteristic.AuraOscura: return "Aura oscura";
            case ItemCharacteristic.RunasMalignas: return "Runas malignas";
            case ItemCharacteristic.LlamasDemoníacas: return "Llamas demoníacas";
            case ItemCharacteristic.MovimientoEspontáneo: return "Movimiento espontáneo";
            default: return c.ToString();
        }
    }
}
