using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnifiingGlassTool : MonoBehaviour
{
    [Header("UI")]
    public RawImage lupaRawImage;        // asignar en inspector: RawImage del canvas
    public Vector2 lupaSize = new Vector2(256, 256);

    [Header("Cámara secundaria")]
    public Camera auraCamera;           // asignar en inspector (cámara secundaria)
    public int renderTextureSize = 512;

    [Header("Ajustes")]
    public float zoomFov = 20f;         // FOV cuando se usa la lupa
    public float normalFov = 60f;       // FOV base (copiado de main camera si quieres)
    public LayerMask auraLayer;         // layer que renderiza la auraCamera

    private RenderTexture rt;
    private bool isUsing = false;
    private Camera mainCam;
    private List<AuraComponent> allAuras = new List<AuraComponent>();

    private void Start()
    {
        mainCam = Camera.main;

        rt = new RenderTexture(renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
        rt.Create();

        if (auraCamera != null)
        {
            auraCamera.targetTexture = rt;
            auraCamera.enabled = false;
            auraCamera.cullingMask = auraLayer;
        }

        if (lupaRawImage != null)
        {
            lupaRawImage.texture = rt;
            lupaRawImage.gameObject.SetActive(false);
            lupaRawImage.rectTransform.sizeDelta = lupaSize;
        }

        // get all AuraComponent in scene
        var found = FindObjectsByType<AuraComponent>(FindObjectsSortMode.None);
        allAuras.AddRange(found);
    }

    private void Update()
    {
        if (!isUsing) return;

        // posicionar la lupa UI en el cursor
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lupaRawImage.canvas.transform as RectTransform,
            Input.mousePosition,
            lupaRawImage.canvas.worldCamera,
            out pos);
        lupaRawImage.rectTransform.anchoredPosition = pos;

        // mantener la auraCamera orientada como la cámara principal (o imitar el ray)
        if (auraCamera != null && mainCam != null)
        {
            auraCamera.transform.position = mainCam.transform.position;
            auraCamera.transform.rotation = mainCam.transform.rotation;
            auraCamera.fieldOfView = zoomFov;
        }
    }

    // Llamar desde ItemInteraction cuando el jugador presiona el uso de la lupa
    public void UseStart()
    {
        if (isUsing) return;
        isUsing = true;

        // activar RT/camera/UI
        if (auraCamera != null) auraCamera.enabled = true;
        if (lupaRawImage != null) lupaRawImage.gameObject.SetActive(true);

        // activar aura visual en los objetos (puedes filtrar por distancia si lo deseas)
        foreach (var a in allAuras)
            a.SetAuraActive(true);
    }

    public void UseHold()
    {
        // en el MVP no necesitamos hacer nada extra, Update se encarga de posicionar
    }

    public void UseRelease()
    {
        if (!isUsing) return;
        isUsing = false;

        if (auraCamera != null) auraCamera.enabled = false;
        if (lupaRawImage != null) lupaRawImage.gameObject.SetActive(false);

        // desactivar aura en objetos
        foreach (var a in allAuras)
            a.SetAuraActive(false);
    }
}
