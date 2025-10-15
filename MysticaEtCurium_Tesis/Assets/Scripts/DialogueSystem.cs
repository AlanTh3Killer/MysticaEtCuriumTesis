using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI de diálogo")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TMP_Text textoDialogo;
    [SerializeField] private Image iconoUI;

    [Header("Contenido")]
    [TextArea(2, 5)] public string[] lineas;
    public Sprite iconoPersonaje;

    private int indice = 0;
    private bool dialogoActivo = false;

    private void Start()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (iconoUI != null && iconoPersonaje != null)
            iconoUI.sprite = iconoPersonaje;
    }

    private void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.E))
        {
            SiguienteLinea();
        }
    }

    public void IniciarDialogo()
    {
        if (panelDialogo == null || textoDialogo == null || lineas.Length == 0) return;

        panelDialogo.SetActive(true);
        indice = 0;
        textoDialogo.text = lineas[indice];
        dialogoActivo = true;
    }

    void SiguienteLinea()
    {
        indice++;
        if (indice < lineas.Length)
        {
            textoDialogo.text = lineas[indice];
        }
        else
        {
            CerrarDialogo();
        }
    }

    void CerrarDialogo()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        dialogoActivo = false;
    }
}
