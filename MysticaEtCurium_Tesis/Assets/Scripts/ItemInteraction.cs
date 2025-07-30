using UnityEngine;
using UnityEngine.UI;

public class ItemInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionDistance = 3f;

    [Header("Manos")]
    [SerializeField] private Transform manoDerecha;
    [SerializeField] private Transform manoIzquierda;

    [Header("Capas")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask mesaLayer;

    [Header("UI Feedback")]
    [SerializeField] private GameObject itemFeedbackUI;
    [SerializeField] private GameObject herramientaFeedbackUI;
    [SerializeField] private GameObject iniciarInspeccionFeedbackUI;

    private GameObject objetoDetectado = null;
    private string tagDetectado = "";

    private GameObject itemEnManoDerecha = null;
    private GameObject herramientaEnManoIzquierda = null;

    [Header("Modo Inspeccion")]
    [SerializeField] private Transform posicionInspeccion;
    [SerializeField] private KeyCode inputInspeccion = KeyCode.F;
    [SerializeField] private float distanciaMaximaMesa = 3f;

    [SerializeField] private Transform puntoDeInspeccion;
    [SerializeField] private float velocidadRotacion = 100f;

    private GameObject objetoEnMesa;
    private bool objetoEnInspeccion = false;
    private bool recogidaDesdeInspeccion = false;


    public bool enModoInspeccion { get; private set; } = false;

    private CharacterController characterController; // si usas este

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Modo inspeccion
        if (Input.GetKeyDown(inputInspeccion))
        {
            if (enModoInspeccion)
            {
                SalirModoInspeccion();
            }
            else
            {
                EntrarModoInspeccion();
            }
        }

        if (Input.GetMouseButtonDown(0) && itemEnManoDerecha != null && !objetoEnInspeccion)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.CompareTag("PuntoInspeccion")) // Asegúrate que este tag esté bien asignado
                {
                    itemEnManoDerecha.transform.position = puntoDeInspeccion.position;
                    itemEnManoDerecha.transform.rotation = puntoDeInspeccion.rotation;
                    itemEnManoDerecha.transform.SetParent(puntoDeInspeccion);

                    Rigidbody rb = itemEnManoDerecha.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true;
                    }

                    objetoEnMesa = itemEnManoDerecha;
                    itemEnManoDerecha = null;
                    objetoEnInspeccion = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && objetoEnInspeccion)
        {
            RecogerObjeto(objetoEnMesa, manoDerecha, ref itemEnManoDerecha);
            objetoEnMesa = null;
            objetoEnInspeccion = false;
            recogidaDesdeInspeccion = true;
        }

        //Interaccion con objetos
        DetectarObjetoFrente();

        // MANO DERECHA (objetos)
        if (Input.GetKeyDown(KeyCode.E) && !recogidaDesdeInspeccion)
        {
            if (itemEnManoDerecha == null && objetoDetectado != null && tagDetectado.StartsWith("Item"))
            {
                RecogerObjeto(objetoDetectado, manoDerecha, ref itemEnManoDerecha);
            }
            else if (itemEnManoDerecha != null)
            {
                SoltarObjeto(itemEnManoDerecha, ref itemEnManoDerecha);
            }
        }

        // MANO IZQUIERDA (herramientas)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (herramientaEnManoIzquierda == null && objetoDetectado != null && tagDetectado == "Herramienta")
            {
                RecogerObjeto(objetoDetectado, manoIzquierda, ref herramientaEnManoIzquierda);
            }
            else if (herramientaEnManoIzquierda != null)
            {
                SoltarObjeto(herramientaEnManoIzquierda, ref herramientaEnManoIzquierda);
            }
        }

        // Colocar con click
        if (Input.GetMouseButtonDown(0) && itemEnManoDerecha != null)
        {
            ColocarEnMesa(itemEnManoDerecha, ref itemEnManoDerecha);
        }

        if (Input.GetMouseButtonDown(1) && herramientaEnManoIzquierda != null)
        {
            ColocarEnMesa(herramientaEnManoIzquierda, ref herramientaEnManoIzquierda);
        }

        if (enModoInspeccion && objetoEnInspeccion && objetoEnMesa != null)
        {
            float rotX = Input.GetAxis("Horizontal");
            float rotY = Input.GetAxis("Vertical");

            objetoEnMesa.transform.Rotate(Vector3.up, rotX * velocidadRotacion * Time.deltaTime, Space.World);
            objetoEnMesa.transform.Rotate(Vector3.right, -rotY * velocidadRotacion * Time.deltaTime, Space.World);
        }

        recogidaDesdeInspeccion = false;
    }

    #region Interaccion
    void DetectarObjetoFrente()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, itemLayer))
        {
            string tag = hit.collider.tag;

            if (tag.StartsWith("Item"))
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(true);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
                return;
            }
            else if (tag == "Herramienta")
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(true);
                return;
            }
            else if(tag == "PuntoInspeccion")
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(true);
                return;
            }
        }

        // No hay objeto válido enfrente
        objetoDetectado = null;
        tagDetectado = "";

        if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
        if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
        if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
    }

    void RecogerObjeto(GameObject obj, Transform mano, ref GameObject referencia)
    {
        referencia = obj;

        Collider col = obj.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        obj.transform.SetParent(mano);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
    }

    void SoltarObjeto(GameObject obj, ref GameObject referencia)
    {
        obj.transform.SetParent(null);

        Collider col = obj.GetComponent<Collider>();
        if (col) col.enabled = true;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true; 
            rb.linearVelocity = Vector3.zero;
        }

        obj.transform.position = cameraTransform.position + cameraTransform.forward * 0.75f;
        referencia = null;
    }

    void ColocarEnMesa(GameObject obj, ref GameObject referencia)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, mesaLayer))
        {
            Vector3 posicion = hit.point;
            Quaternion rotacion = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0);

            obj.transform.SetParent(null);
            obj.transform.position = posicion + Vector3.up * 0.05f; // levemente encima de la mesa
            obj.transform.rotation = rotacion;

            Collider col = obj.GetComponent<Collider>();
            if (col) col.enabled = true;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            referencia = null;
        }
    }
    #endregion

    #region ModoInspeccion
    void EntrarModoInspeccion()
    {
        enModoInspeccion = true;

        // Teletransportar al jugador a la posición de inspección
        transform.position = posicionInspeccion.position;
        transform.rotation = posicionInspeccion.rotation;

        // Bloquear movimiento
        if (characterController != null) characterController.enabled = false;

        // Activar UI / modo inspección
        Debug.Log("Modo inspección activado.");
        // Aquí luego podrías activar un script de rotación de objeto, etc.
    }

    void SalirModoInspeccion()
    {
        enModoInspeccion = false;

        // Restaurar movimiento
        if (characterController != null) characterController.enabled = true;

        // Desactivar UI / modo inspección
        Debug.Log("Modo inspección desactivado.");
    }
    #endregion

    #region DebugVisual
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (cameraTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * interactionDistance);
            Gizmos.DrawSphere(cameraTransform.position + cameraTransform.forward * interactionDistance, 0.05f);
        }
#endif
    }
    #endregion

}
