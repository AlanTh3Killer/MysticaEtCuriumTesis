using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteraction : MonoBehaviour
{
    [Header("UI de Inspección")]
    [SerializeField] private TextMeshProUGUI inspectionStatusText; // Texto que muestra características descubiertas
    [SerializeField] private GameObject contenedorFeedbackUI; // ← E para depositar en contenedor

    [Header("Configuración")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionDistance = 3f;

    [Header("Manos")]
    [SerializeField] private Transform manoDerecha;
    [SerializeField] private Transform manoIzquierda;

    [Header("Capas")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask mesaLayer;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private LayerMask contenedorLayer;

    [Header("UI Feedback")]
    [SerializeField] private GameObject itemFeedbackUI;
    [SerializeField] private GameObject herramientaFeedbackUI;
    [SerializeField] private GameObject iniciarInspeccionFeedbackUI;
    [SerializeField] private GameObject usarHerramientaFeedbackUI;   // ← NUEVO: click derecho
    [SerializeField] private GameObject salirInspeccionFeedbackUI;   // ← NUEVO: F esquina superior
    [SerializeField] private GameObject tomarObjetoInspeccionFeedbackUI;
    [SerializeField] private GameObject colocarEnMesaFeedbackUI;
    [SerializeField] private GameObject mesaTrabajoFeedbackUI; // ← feedback click derecho para mesa de trabajo

    private GameObject objetoDetectado = null;
    private string tagDetectado = "";

    private GameObject itemEnManoDerecha = null;
    private GameObject herramientaEnManoIzquierda = null;

    [Header("Modo Inspeccion")]
    [SerializeField] private Transform posicionInspeccion;
    [SerializeField] private KeyCode inputInspeccion = KeyCode.F;
    [SerializeField] private float distanciaMaximaMesa = 3f;        // ← para teleport
    [SerializeField] private float distanciaFeedbackMesa = 2f;      // ← NUEVO: para feedback visual
    [SerializeField] private GameObject wasdFeedbackUI; // ← WASD alrededor del objeto

    [SerializeField] private Transform puntoDeInspeccion;
    [SerializeField] private float velocidadRotacion = 100f;

    private GameObject objetoEnMesa;
    private bool objetoEnInspeccion = false;
    private bool recogidaDesdeInspeccion = false;

    [SerializeField] private MagnifiingGlassTool lupaTool;

    [Header("Física de lanzamiento")]
    [SerializeField] private float fuerzaArrojar = 10f;
    [SerializeField] private float umbralMovimiento = 0.2f;

    [Header("Mesa de Trabajo")]
    [SerializeField] private Transform mesaDeTrabajo;           // Transform del objeto mesa de trabajo
    [SerializeField] private LayerMask mesaTrabajoLayer;        // Layer exclusiva de la mesa de trabajo
    [SerializeField] private float distanciaMesaTrabajo = 3f;

    public bool enModoInspeccion { get; private set; } = false;

    private CharacterController characterController;
    private bool primeraVezInspeccion = true;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Asegurar que los feedbacks estén desactivados al inicio
        if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
        if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
        if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
    }

    void Update()
    {
        // No procesar feedbacks si hay menú con prioridad activo
        if (PauseManager.PrioridadActual > 0) return;
        if (DialogueSystem.DialogoActivo || PauseManager.PrioridadActual > 0)
            return;

        // MODO INSPECCION
        //if (Input.GetKeyDown(inputInspeccion))
        //{
        //    if (!enModoInspeccion)
        //    {
        //        // Verificar proximidad ANTES de triggear el diálogo
        //        if (!JugadorCercaDeMesa()) return; // ← salir si está lejos

        //        if (primeraVezInspeccion)
        //        {
        //            FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyInspect();
        //            primeraVezInspeccion = false;
        //        }
        //        EntrarModoInspeccion();
        //    }
        //    else
        //    {
        //        SalirModoInspeccion();
        //    }
        //}

        // MODO INSPECCION Alternativo
        if (Input.GetKeyDown(inputInspeccion))
        {
            if (!enModoInspeccion)
            {
                if (JugadorCercaDeMesa()) // ← solo si está cerca
                {
                    if (primeraVezInspeccion)
                    {
                        FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyInspect();
                        primeraVezInspeccion = false;
                    }
                    EntrarModoInspeccion();
                }
            }
            else
            {
                SalirModoInspeccion();
            }
        }

        // --- Colocar objeto en punto de inspección ---
        if (Input.GetMouseButtonDown(0) && itemEnManoDerecha != null && !objetoEnInspeccion)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.CompareTag("PuntoInspeccion"))
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

                    // ← AGREGAR ESTO: reactivar collider para que el raycast lo detecte
                    Collider col = objetoEnMesa.GetComponent<Collider>();
                    if (col != null) col.enabled = true;

                    //  NUEVO: Iniciar tracking de inspección
                    MagicItemBehaviour magicItem = objetoEnMesa.GetComponent<MagicItemBehaviour>();
                    if (magicItem != null && InspectionTracker.Instance != null)
                    {
                        InspectionTracker.Instance.StartInspection(magicItem);
                        //NUEVO: Llamar diálogo de inspección
                        //FindFirstObjectByType<SimpleDialogueTrigger>()?.NotifyInspect();
                    }
                }
            }
        }

        // --- Retomar objeto desde inspección ---
        if (Input.GetKeyDown(KeyCode.E) && objetoEnInspeccion && enModoInspeccion)
        {
            // ← AGREGAR: quitar parent antes de recoger
            if (objetoEnMesa != null)
                objetoEnMesa.transform.SetParent(null);

            RecogerObjeto(objetoEnMesa, manoDerecha, ref itemEnManoDerecha);
            objetoEnMesa = null;
            objetoEnInspeccion = false;
            recogidaDesdeInspeccion = true;
        }

        // --- DETECCIÓN DE OBJETO FRENTE ---
        DetectarObjetoFrente();

        // --- ARROJAR OBJETOS CON G ---
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (itemEnManoDerecha != null)
            {
                ArrojarObjeto(itemEnManoDerecha, ref itemEnManoDerecha);
            }
            else if (herramientaEnManoIzquierda != null)
            {
                ArrojarObjeto(herramientaEnManoIzquierda, ref herramientaEnManoIzquierda);
            }
        }

        // --- INTERACCIÓN GENERAL (E) - MANO DERECHA (objetos) ---
        if (Input.GetKeyDown(KeyCode.E) && !recogidaDesdeInspeccion)
        {
            if (itemEnManoDerecha == null && objetoDetectado != null && tagDetectado.StartsWith("Item"))
            {
                // ← AGREGAR: si el objeto detectado es el que está en mesa, limpiar estado
                if (objetoDetectado == objetoEnMesa)
                {
                    objetoEnMesa = null;
                    objetoEnInspeccion = false;
                    if (InspectionTracker.Instance != null)
                        InspectionTracker.Instance.ClearInspection();
                }

                RecogerObjeto(objetoDetectado, manoDerecha, ref itemEnManoDerecha);
            }
            //  QUITAR ESTO - Ya no arroja con E
            // else if (itemEnManoDerecha != null)
            // {
            //     ArrojarObjeto(itemEnManoDerecha, ref itemEnManoDerecha);
            // }

            // Interacción con NPC
            if (objetoDetectado != null && tagDetectado == "NPC")
            {
                Debug.Log("Intentando iniciar diálogo con: " + objetoDetectado.name);
                DialogueSystem dialogo = objetoDetectado.GetComponent<DialogueSystem>();
                if (dialogo != null)
                {
                    Debug.Log("Componente DialogueSystem encontrado. Iniciando diálogo...");
                    dialogo.IniciarDialogo();
                }
                else
                {
                    Debug.LogWarning("El NPC detectado no tiene componente DialogueSystem: " + objetoDetectado.name);
                }
            }
        }

        // --- MANO IZQUIERDA (herramientas) ---
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (herramientaEnManoIzquierda == null && objetoDetectado != null && tagDetectado == "Herramienta")
            {
                RecogerObjeto(objetoDetectado, manoIzquierda, ref herramientaEnManoIzquierda);
            }
            else if (herramientaEnManoIzquierda != null)
            {
                ArrojarObjeto(herramientaEnManoIzquierda, ref herramientaEnManoIzquierda);
            }
        }

        // --- Uso de herramientas (Lupa mágica) ---
        if (enModoInspeccion && Input.GetMouseButtonDown(0) && herramientaEnManoIzquierda != null)
        {
            if (herramientaEnManoIzquierda.GetComponent<MagnifiingGlassTool>() != null)
            {
                lupaTool = herramientaEnManoIzquierda.GetComponent<MagnifiingGlassTool>();
                lupaTool.UseStart();
            }
        }
        if (enModoInspeccion && Input.GetMouseButton(0) && herramientaEnManoIzquierda != null)
        {
            if (lupaTool != null) lupaTool.UseHold();
        }
        if (enModoInspeccion && Input.GetMouseButtonUp(0) && lupaTool != null)
        {
            lupaTool.UseRelease();
            lupaTool = null;
        }

        if (Input.GetMouseButtonDown(1) && enModoInspeccion && herramientaEnManoIzquierda != null)
        {
            UsarHerramientaEnObjeto();
        }

        // --- Colocar con clics (esto parece duplicado, revisar) ---
        if (Input.GetMouseButtonDown(0) && itemEnManoDerecha != null && !objetoEnInspeccion)
        {
            ColocarEnMesa(itemEnManoDerecha, ref itemEnManoDerecha);
        }

        if (Input.GetMouseButtonDown(1) && herramientaEnManoIzquierda != null && !enModoInspeccion)
        {
            ColocarEnMesaTrabajo(herramientaEnManoIzquierda, ref herramientaEnManoIzquierda);
        }

        // --- Rotar objeto en modo inspección ---
        if (enModoInspeccion && objetoEnInspeccion && objetoEnMesa != null)
        {
            float rotX = Input.GetAxis("Horizontal");
            float rotY = Input.GetAxis("Vertical");

            objetoEnMesa.transform.Rotate(Vector3.up, rotX * velocidadRotacion * Time.deltaTime, Space.World);
            objetoEnMesa.transform.Rotate(Vector3.right, -rotY * velocidadRotacion * Time.deltaTime, Space.World);
        }

        recogidaDesdeInspeccion = false;

        // --- Actualizar UI de estado de inspección ---
        if (enModoInspeccion && objetoEnInspeccion && InspectionTracker.Instance != null)
        {
            if (inspectionStatusText != null)
            {
                int discovered = InspectionTracker.Instance.GetDiscoveredCount();
                bool canClassify = InspectionTracker.Instance.CanClassify(out string reason);

                if (canClassify)
                    inspectionStatusText.text = $"Características: {discovered} ✓\nListo para clasificar";
                else
                    inspectionStatusText.text = $"Características: {discovered}\n{reason}";
            }
        }
        else if (inspectionStatusText != null)
        {
            inspectionStatusText.text = "";
        }

        // Al final del Update(), antes del cierre:
        ActualizarFeedbacksModoInspeccion();
    }

    #region Interaccion
    bool JugadorSeMueve()
    {
        if (characterController == null) return false;
        return characterController.velocity.magnitude > umbralMovimiento;
    }

    //  FIX PRINCIPAL: Detección separada para feedback UI
    void DetectarObjetoFrente()
    {
        // Si estamos en modo inspección, los feedbacks los maneja ActualizarFeedbacksModoInspeccion
        if (enModoInspeccion) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // PRIMERA PASADA: Solo detectar items, herramientas y NPCs para feedback
        int feedbackMask = itemLayer.value | npcLayer.value;

        if (Physics.Raycast(ray, out hit, interactionDistance, feedbackMask))
        {
            string tag = hit.collider.tag;

            // Items
            if (tag.StartsWith("Item"))
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(itemEnManoDerecha == null);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
                if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
                return;
            }
            // Herramientas
            else if (tag == "Herramienta")
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(herramientaEnManoIzquierda == null);
                if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
                return;
            }
            // NPC
            else if (tag == "NPC")
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = tag;

                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
                if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
                return;
            }
        }

        // SEGUNDA PASADA: Detectar punto de inspección (necesita estar en mesaLayer)
        if (Physics.Raycast(ray, out hit, interactionDistance, mesaLayer))
        {
            if (hit.collider.CompareTag("PuntoInspeccion"))
            {
                objetoDetectado = hit.collider.gameObject;
                tagDetectado = "PuntoInspeccion";

                // Mostrar F solo si está cerca de la mesa
                bool tieneItemEnMano = itemEnManoDerecha != null;
                bool hayObjetoEnMesa = objetoEnInspeccion;

                bool mostrarF = JugadorEnRangoDeFeedback() && (tieneItemEnMano || hayObjetoEnMesa || enModoInspeccion);

                if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(mostrarF);
                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
                if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
                return;
            }
        }

        // TERCERA PASADA: Detectar mesa de trabajo
        if (Physics.Raycast(ray, out hit, distanciaMesaTrabajo, mesaTrabajoLayer))
        {
            bool tieneHerramienta = herramientaEnManoIzquierda != null;
            Debug.Log($"[MesaTrabajo] Detectada. tieneHerramienta: {tieneHerramienta}, feedbackUI activo: {mesaTrabajoFeedbackUI.activeSelf}");
            mesaTrabajoFeedbackUI.SetActive(tieneHerramienta);
            Debug.Log($"[MesaTrabajo] Después de SetActive: {mesaTrabajoFeedbackUI.activeSelf}");
            return;
        }



        // Si no detecta nada válido
        objetoDetectado = null;
        tagDetectado = "";

        if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
        if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
        if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);
        if (mesaTrabajoFeedbackUI != null) mesaTrabajoFeedbackUI.SetActive(false);

    }

    void ActualizarFeedbacksModoInspeccion()
    {

        if (enModoInspeccion)
        {
            // Mostrar F-salir perpetuamente en esquina superior
            if (salirInspeccionFeedbackUI != null) salirInspeccionFeedbackUI.SetActive(true);

            // Ocultar el feedback de entrar a inspección
            if (iniciarInspeccionFeedbackUI != null) iniciarInspeccionFeedbackUI.SetActive(false);

            // Si hay objeto en mesa
            if (objetoEnInspeccion)
            {
                bool tieneHerramienta = herramientaEnManoIzquierda != null;
                bool mirandoObjeto = JugadorMirandoObjetoEnMesa();

                // Click derecho = usar herramienta SOLO si mira el objeto
                if (usarHerramientaFeedbackUI != null)
                    usarHerramientaFeedbackUI.SetActive(tieneHerramienta && mirandoObjeto);

                // E centrado = tomar objeto SOLO si mira el objeto y no tiene herramienta
                if (tomarObjetoInspeccionFeedbackUI != null)
                    tomarObjetoInspeccionFeedbackUI.SetActive(!tieneHerramienta && mirandoObjeto);

                // El feedback E normal siempre apagado en modo inspección
                if (itemFeedbackUI != null)
                    itemFeedbackUI.SetActive(false);

                // No hay objeto que colocar, apagar ese feedback
                if (colocarEnMesaFeedbackUI != null)
                    colocarEnMesaFeedbackUI.SetActive(false);

                // WASD siempre visible cuando hay objeto en mesa
                bool jugadorRotando = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ||
                                  Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
                if (wasdFeedbackUI != null)
                    wasdFeedbackUI.SetActive(!jugadorRotando);
            }
            else // No hay objeto en mesa todavía
            {
                if (usarHerramientaFeedbackUI != null) usarHerramientaFeedbackUI.SetActive(false);
                if (tomarObjetoInspeccionFeedbackUI != null) tomarObjetoInspeccionFeedbackUI.SetActive(false);
                if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);

                // Mostrar click izquierdo solo si tiene item en mano
                if (colocarEnMesaFeedbackUI != null)
                    colocarEnMesaFeedbackUI.SetActive(itemEnManoDerecha != null);

                // Sin objeto en mesa, apagar WASD
                if (wasdFeedbackUI != null)
                    wasdFeedbackUI.SetActive(false);
            }
        }
        else // Fuera de modo inspección
        {
            if (salirInspeccionFeedbackUI != null) salirInspeccionFeedbackUI.SetActive(false);
            if (usarHerramientaFeedbackUI != null) usarHerramientaFeedbackUI.SetActive(false);
            if (tomarObjetoInspeccionFeedbackUI != null) tomarObjetoInspeccionFeedbackUI.SetActive(false);
            if (colocarEnMesaFeedbackUI != null) colocarEnMesaFeedbackUI.SetActive(false);
            if (wasdFeedbackUI != null) wasdFeedbackUI.SetActive(false);
            if (contenedorFeedbackUI != null) contenedorFeedbackUI.SetActive(false);
        }
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

        // Desactivar feedback al recoger
        if (itemFeedbackUI != null) itemFeedbackUI.SetActive(false);
        if (herramientaFeedbackUI != null) herramientaFeedbackUI.SetActive(false);
    }

    //void ArrojarObjeto(GameObject obj, ref GameObject referencia)
    //{
    //  obj.transform.SetParent(null);
    //Collider col = obj.GetComponent<Collider>();
    //if (col) col.enabled = true;

    //Rigidbody rb = obj.GetComponent<Rigidbody>();
    //if (rb)
    //{
    //  rb.isKinematic = false;
    // rb.useGravity = true;

    // Vector3 direccion = cameraTransform.forward + new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(0f, 0.05f), 0);
    //rb.AddForce(direccion.normalized * fuerzaArrojar, ForceMode.Impulse);
    //rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
    //}

    //Debug.Log("Objeto arrojado: " + obj.name);
    //referencia = null;
    //}

    void ArrojarObjeto(GameObject obj, ref GameObject referencia)
    {
        Debug.Log($"[ArrojarObjeto] Arrojando: {obj.name}");

        obj.transform.SetParent(null);

        Collider col = obj.GetComponent<Collider>();
        if (col)
        {
            col.enabled = true;
            Debug.Log($"[ArrojarObjeto] Collider activado: {col.enabled}");
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            Debug.Log($"[ArrojarObjeto] Rigidbody configurado - isKinematic: {rb.isKinematic}, useGravity: {rb.useGravity}");

            Vector3 direccion = cameraTransform.forward;
            direccion += new Vector3(
                Random.Range(-0.05f, 0.05f),
                Random.Range(0.1f, 0.3f),  // ← siempre un poco hacia arriba
                Random.Range(-0.05f, 0.05f)
            );
            direccion.Normalize();

            rb.AddForce(direccion * fuerzaArrojar, ForceMode.Impulse);

            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

            ;

            //  NUEVO: Agregar componente temporal que evita detección
            ThrownObjectMarker marker = obj.AddComponent<ThrownObjectMarker>();
            marker.StartCooldown(0.5f); // 0.5 segundos de inmunidad

            StartCoroutine(CheckVelocityAfterThrow(rb));
        }
        else
        {
            Debug.LogError($"[ArrojarObjeto] NO HAY RIGIDBODY en {obj.name}!");
        }

        Debug.Log("Objeto arrojado: " + obj.name);
        referencia = null;
    }

    //  AGREGAR ESTA COROUTINE
    private IEnumerator CheckVelocityAfterThrow(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);

        if (rb != null)
        {
            Debug.Log($"[ArrojarObjeto] Velocidad después de arrojar: {rb.linearVelocity}, isKinematic: {rb.isKinematic}");
        }
    }

    void ColocarEnMesaTrabajo(GameObject obj, ref GameObject referencia)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaMesaTrabajo, mesaTrabajoLayer))
        {
            obj.transform.SetParent(null);
            obj.transform.position = hit.point + Vector3.up * 0.05f;
            obj.transform.rotation = Quaternion.identity;

            Collider col = obj.GetComponent<Collider>();
            if (col) col.enabled = true;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            referencia = null;
            Debug.Log("[ItemInteraction] Herramienta colocada en mesa de trabajo.");
        }
        else
        {
            Debug.Log("[ItemInteraction] No estás viendo la mesa de trabajo.");
        }
    }

    void ColocarEnMesa(GameObject obj, ref GameObject referencia)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, mesaLayer))
        {
            // Herramientas NO pueden ir al PuntoInspeccion
            if (hit.collider.CompareTag("PuntoInspeccion") && obj.CompareTag("Herramienta"))
            {
                Debug.Log("[ItemInteraction] Las herramientas no van al punto de inspección.");
                return;
            }

            Vector3 posicion = hit.point;
            Quaternion rotacion = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0);

            obj.transform.SetParent(null);
            obj.transform.position = posicion + Vector3.up * 0.05f;
            obj.transform.rotation = rotacion;

            Collider col = obj.GetComponent<Collider>();
            if (col) col.enabled = true;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            referencia = null;
        }
    }

    public void DepositHeldItemIntoContainer(ItemContainer container)
    {
        if (itemEnManoDerecha == null) return;
        if (container == null) return;

        // Ya no bloqueamos — solo mostramos advertencia si no ha inspeccionado suficiente
        if (InspectionTracker.Instance != null)
        {
            bool canClassify = InspectionTracker.Instance.CanClassify(out string reason);
            if (!canClassify && inspectionStatusText != null)
            {
                inspectionStatusText.color = Color.yellow;
                inspectionStatusText.fontSize = 20;
                inspectionStatusText.text = $"⚠️ Pocas características identificadas\n{reason}";
                CancelInvoke("ClearInspectionStatus");
                Invoke("ClearInspectionStatus", 2f);
                // No hacemos return — dejamos que continúe
            }
        }

        GameObject obj = itemEnManoDerecha;
        itemEnManoDerecha = null;

        obj.transform.SetParent(null);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.transform.position = container.transform.position + Vector3.up * 0.5f;
        obj.transform.rotation = Quaternion.identity;

        container.ProcessItemManual(obj);

        if (InspectionTracker.Instance != null)
            InspectionTracker.Instance.ClearInspection();

        LimpiarEstadoMesa();

        //if (itemEnManoDerecha == null) return;
        //if (container == null) return;

        //  VALIDAR si se puede clasificar
        //if (InspectionTracker.Instance != null)
        //{
        //bool canClassify = InspectionTracker.Instance.CanClassify(out string reason);

        //if (!canClassify)
        //{
        //Debug.LogWarning($"[ItemInteraction] No se puede clasificar: {reason}");
        // Mostrar feedback visual al jugador
        //if (inspectionStatusText != null)
        //{
        //inspectionStatusText.color = Color.red; // ← Texto rojo
        //inspectionStatusText.fontSize = 24; // ← Más grande
        //inspectionStatusText.text = $"⚠️ ¡ERROR!\n\n{reason}\n\nUsa las herramientas primero";

        //  CancelInvoke("ClearInspectionStatus");
        //    Invoke("ClearInspectionStatus", 3f); // ← Más tiempo visible
        //  }
        //    return;
        //  }
        //}

        //GameObject obj = itemEnManoDerecha;
        //itemEnManoDerecha = null;

        //obj.transform.SetParent(null);

        //Rigidbody rb = obj.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //  rb.isKinematic = false;
        //    rb.useGravity = true;
        //     rb.linearVelocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //}

        // obj.transform.position = container.transform.position + Vector3.up * 0.5f;
        //obj.transform.rotation = Quaternion.identity;

        //container.ProcessItemManual(obj);

        //  Limpiar tracking
        //if (InspectionTracker.Instance != null)
        //  InspectionTracker.Instance.ClearInspection();

        // ← AGREGAR:
        //LimpiarEstadoMesa();
    }

    private void ClearInspectionStatus()
    {
        if (inspectionStatusText != null)
        {
            inspectionStatusText.text = "";
            inspectionStatusText.color = Color.white; // ← Restaurar color
            inspectionStatusText.fontSize = 18; // ← Restaurar tamaño
        }
    }

    public bool HasItemInRightHand()
    {
        return itemEnManoDerecha != null;
    }
    #endregion

    #region ModoInspeccion
    private bool JugadorCercaDeMesa()
    {
        if (posicionInspeccion == null) return false;
        return Vector3.Distance(transform.position, posicionInspeccion.position) <= distanciaMaximaMesa;
    }

    private bool JugadorEnRangoDeFeedback()
    {
        if (posicionInspeccion == null) return false;
        return Vector3.Distance(transform.position, posicionInspeccion.position) <= distanciaFeedbackMesa;
    }

    void EntrarModoInspeccion()
    {
        // Verificar que el jugador esté cerca de la mesa
        if (posicionInspeccion == null) return;

        float distancia = Vector3.Distance(transform.position, posicionInspeccion.position);
        if (distancia > distanciaMaximaMesa)
        {
            Debug.Log("[ItemInteraction] Demasiado lejos de la mesa para inspeccionar.");
            return;
        }

        enModoInspeccion = true;
        transform.position = posicionInspeccion.position;
        transform.rotation = posicionInspeccion.rotation;

        if (characterController != null) characterController.enabled = false;
    }

    void SalirModoInspeccion()
    {
        enModoInspeccion = false;

        if (characterController != null) characterController.enabled = true;
    }

    private void UsarHerramientaEnObjeto()
    {
        if (herramientaEnManoIzquierda == null || objetoEnMesa == null) return;

        SimpleTool tool = herramientaEnManoIzquierda.GetComponent<SimpleTool>();
        if (tool == null) return;

        MagicItemBehaviour item = objetoEnMesa.GetComponent<MagicItemBehaviour>();
        if (item == null) return;

        tool.UseToolOnObject(item);
    }

    private bool JugadorMirandoObjetoEnMesa()
    {
        if (objetoEnMesa == null) return false;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        // Sin layer mask para detectar cualquier cosa, luego verificamos si es el objeto
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == objetoEnMesa ||
                hit.collider.transform.IsChildOf(objetoEnMesa.transform))
                return true;
        }
        return false;
    }

    public void LimpiarEstadoMesa()
    {
        objetoEnMesa = null;
        objetoEnInspeccion = false;
        itemEnManoDerecha = null;

        // Limpiar hijos del puntoDeInspeccion por si quedó algo
        if (puntoDeInspeccion != null)
        {
            foreach (Transform child in puntoDeInspeccion)
                Destroy(child.gameObject);
        }
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
