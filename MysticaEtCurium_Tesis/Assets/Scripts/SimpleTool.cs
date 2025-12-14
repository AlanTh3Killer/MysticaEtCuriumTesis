using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SimpleTool : MonoBehaviour
{
    [Header("Configuraci�n de herramienta")]
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

        List<ItemCharacteristic> detected = new List<ItemCharacteristic>();

        foreach (var characteristic in item.data.characteristics)
        {
            if (canDetect.Contains(characteristic))
            {
                detected.Add(characteristic);
            }
        }

        if (detected.Count > 0 && InspectionTracker.Instance != null)
        {
            InspectionTracker.Instance.RegisterDiscoveredCharacteristics(detected);
        }

        if (detected.Count > 0)
        {
            string message = $"<b>{toolName} detectó:</b>\n\n";
            foreach (var c in detected)
            {
                message += $"✓ {GetCharacteristicName(c)}\n";
            }
            ShowMessage(message);

            // ✅ FIX: Solo mostrar feedback en el OBJETO, no en la herramienta
            if (FeedbackManager.Instance != null)
            {
                FeedbackManager.Instance.ShowDiscoveryFeedback(item.transform.position);
                FeedbackManager.Instance.HighlightObject(item.gameObject, 0.3f);
            }

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
            case ItemCharacteristic.SonidoRitmico: return "Sonido r�tmico";
            case ItemCharacteristic.AuraNaranja: return "Aura naranja";
            case ItemCharacteristic.RunasInvocacion: return "Runas de invocaci�n";
            case ItemCharacteristic.RunasDefensivas: return "Runas defensivas";
            case ItemCharacteristic.EnergiaInestable: return "Energ�a inestable";
            case ItemCharacteristic.VocesEspectrales: return "Voces espectrales";
            case ItemCharacteristic.VocesDemoniacas: return "Voces demon�acas";
            case ItemCharacteristic.AuraRoja: return "Aura roja";
            case ItemCharacteristic.AuraOscura: return "Aura oscura";
            case ItemCharacteristic.RunasMalignas: return "Runas malignas";
            case ItemCharacteristic.LlamasDemoniacas: return "Llamas demon�acas";
            case ItemCharacteristic.MovimientoEspontaneo: return "Movimiento espont�neo";
            default: return c.ToString();
        }
    }
}
