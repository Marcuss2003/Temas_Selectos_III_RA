using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Clase final para la práctica de Realidad Aumentada: "Un Show Más".
/// Controla narrativa, movimiento, aleatoriedad y reinicio de ciclo.
/// </summary>
public class MOVE : MonoBehaviour
{
    public enum EstadoHistoria { BUSCANDO_RIGBY, BUSCANDO_PODER, BUSCANDO_DESTRUCTOR, VICTORIA }
    
    [Header("Progreso de la Historia")]
    public EstadoHistoria estadoActual = EstadoHistoria.BUSCANDO_RIGBY;

    [Header("Configuración de Movimiento")]
    public GameObject modelMordecai; 
    public float speed = 1.0f;
    public float rotationSpeed = 8.0f;
    public float offsetRotationY = 180f; 

    [Header("Efecto de Escritura Diálogos")]
    public float velocidadEscritura = 0.04f;

    [Header("Panel de Instrucciones (Benson)")]
    public GameObject panelInstrucciones; 
    public TextMeshProUGUI textoInstrucciones; 
    public Button botonEntendido;
    [TextArea(5, 10)]
    public string mensajeTutorial = "¡MIRA MORDECAI! El Destructor escapó.\n\n1. Enfoca el marcador de inicio.\n2. Busca a Rigby y El Poder en los otros marcadores.\n3. Presiona 'Siguiente' para caminar.\n4. ¡Derrota al gigante!";

    [Header("Paneles de Diálogo Juego")]
    public GameObject panelMordecai;
    public TextMeshProUGUI textoMordecai;
    public GameObject panelRigby;
    public TextMeshProUGUI textoRigby;

    [Header("Referencias AR")]
    public ObserverBehaviour[] ImageTargets; 
    public GameObject rigbyModel;
    public GameObject elPoderModel;
    public GameObject destructorModel;
    public Transform socketRigby;
    public Transform socketPoder;
    public GameObject botonUsarPoder;
    public GameObject botonRestart; 

    // Variables de control interno
    private Animator anim;
    private bool isMoving = false;
    private bool instruccionesCerradas = false;
    private bool dialogoInicialDicho = false;
    private ObserverBehaviour lastTarget;
    private Dictionary<ObserverBehaviour, string> contenidoTarget = new Dictionary<ObserverBehaviour, string>();
    
    private Coroutine corrutinaMordecai;
    private Coroutine corrutinaRigby;

    void Start()
    {
        anim = modelMordecai.GetComponentInChildren<Animator>();
        if (anim != null) anim.applyRootMotion = false;
        
        // Configuración inicial de la lógica aleatoria
        ConfigurarEscenaInicial();
        
        // Mostrar tutorial de Benson
        panelInstrucciones.SetActive(true);
        textoInstrucciones.text = mensajeTutorial;
        botonEntendido.gameObject.SetActive(true); 
    }

    /// <summary>
    /// Prepara los modelos y baraja los objetos en los marcadores.
    /// </summary>
    void ConfigurarEscenaInicial()
    {
        // Resetear posición de Mordecai al Target 0
        lastTarget = ImageTargets[0];
        modelMordecai.transform.SetParent(lastTarget.transform);
        modelMordecai.transform.localPosition = Vector3.zero;
        modelMordecai.transform.localRotation = Quaternion.Euler(0, offsetRotationY, 0);

        contenidoTarget.Clear();

        // Reparto aleatorio
        List<string> contenido = new List<string> { "Rigby", "ElPoder", "Destructor", "Vacio" };
        List<ObserverBehaviour> targetsLibres = new List<ObserverBehaviour>();
        for (int i = 1; i < ImageTargets.Length; i++) targetsLibres.Add(ImageTargets[i]);

        foreach (string item in contenido) {
            int r = Random.Range(0, targetsLibres.Count);
            contenidoTarget.Add(targetsLibres[r], item);
            MudarModeloInicial(item, targetsLibres[r]);
            targetsLibres.RemoveAt(r);
        }

        // Reset de estados y UI
        estadoActual = EstadoHistoria.BUSCANDO_RIGBY;
        dialogoInicialDicho = false;
        panelMordecai.SetActive(false);
        panelRigby.SetActive(false);
        botonUsarPoder.SetActive(false);
        botonRestart.SetActive(false);
    }

    /// <summary>
    /// Limpia la pantalla de textos viejos y reinicia la narrativa.
    /// </summary>
    public void ReiniciarJuego()
    {
        StopAllCoroutines();
        isMoving = false;

        // LIMPIEZA TOTAL DE TEXTOS (Evita que se queden los "OHHHH")
        if (textoMordecai != null) textoMordecai.text = "";
        if (textoRigby != null) textoRigby.text = "";
        panelMordecai.SetActive(false);
        panelRigby.SetActive(false);

        // Despegar modelos de los sockets de Mordecai
        rigbyModel.transform.SetParent(null);
        elPoderModel.transform.SetParent(null);

        ConfigurarEscenaInicial();
        instruccionesCerradas = true; 
    }

    void Update()
    {
        if (instruccionesCerradas && !dialogoInicialDicho && lastTarget != null)
        {
            if (lastTarget.TargetStatus.Status == Status.TRACKED || lastTarget.TargetStatus.Status == Status.EXTENDED_TRACKED)
            {
                MostrarDialogo("Mordecai", "¡Viejo, el Destructor escapó! ¡Tengo que encontrar a Rigby!");
                dialogoInicialDicho = true;
            }
        }
    }

    public void CerrarInstrucciones()
    {
        textoInstrucciones.text = ""; 
        botonEntendido.gameObject.SetActive(false); 
        panelInstrucciones.SetActive(false);
        instruccionesCerradas = true; 
    }

    void MostrarDialogo(string personaje, string mensaje)
    {
        if (personaje == "Mordecai") {
            if (corrutinaMordecai != null) StopCoroutine(corrutinaMordecai);
            panelMordecai.SetActive(true);
            corrutinaMordecai = StartCoroutine(EscribirTexto(textoMordecai, mensaje));
        } 
        else if (personaje == "Rigby") {
            if (corrutinaRigby != null) StopCoroutine(corrutinaRigby);
            panelRigby.SetActive(true);
            corrutinaRigby = StartCoroutine(EscribirTexto(textoRigby, mensaje));
        }
    }

    IEnumerator EscribirTexto(TextMeshProUGUI tmpComponent, string mensajeCompleto)
    {
        tmpComponent.text = "";
        foreach (char letra in mensajeCompleto.ToCharArray())
        {
            tmpComponent.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    public void moveToNextMarker() {
        if (!isMoving && instruccionesCerradas) StartCoroutine(SequenceMovement());
    }

    private IEnumerator SequenceMovement()
    {
        ObserverBehaviour targetDestino = null;
        foreach (var t in ImageTargets) {
            if (t != lastTarget && (t.TargetStatus.Status == Status.TRACKED || t.TargetStatus.Status == Status.EXTENDED_TRACKED)) {
                targetDestino = t; break;
            }
        }

        if (targetDestino == null) {
            MostrarDialogo("Mordecai", "¡No veo otro marcador!");
            yield break;
        }

        // Detener diálogos al caminar
        if (corrutinaMordecai != null) StopCoroutine(corrutinaMordecai);
        if (corrutinaRigby != null) StopCoroutine(corrutinaRigby);
        textoMordecai.text = "";
        textoRigby.text = "";
        panelMordecai.SetActive(false);
        panelRigby.SetActive(false);

        isMoving = true;
        if (anim != null) anim.SetBool("isWalking", true);

        Vector3 startPos = modelMordecai.transform.position;
        float journey = 0;
        while (journey <= 1f) {
            journey += Time.deltaTime * speed;
            modelMordecai.transform.position = Vector3.Lerp(startPos, targetDestino.transform.position, journey);
            Vector3 dir = (targetDestino.transform.position - modelMordecai.transform.position);
            if (dir != Vector3.zero) 
            {
                Quaternion lookRot = Quaternion.LookRotation(dir, modelMordecai.transform.up);
                modelMordecai.transform.rotation = Quaternion.Slerp(modelMordecai.transform.rotation, lookRot * Quaternion.Euler(0, offsetRotationY, 0), Time.deltaTime * rotationSpeed);
            }
            yield return null;
        }

        if (anim != null) anim.SetBool("isWalking", false);
        modelMordecai.transform.SetParent(targetDestino.transform);
        modelMordecai.transform.localPosition = Vector3.zero;
        modelMordecai.transform.localRotation = Quaternion.Euler(0, offsetRotationY, 0);

        lastTarget = targetDestino;
        ProcesarLlegada(targetDestino);
        isMoving = false;
    }

    void ProcesarLlegada(ObserverBehaviour target)
    {
        if (!contenidoTarget.ContainsKey(target)) return;
        string queHay = contenidoTarget[target];

        if (queHay == "Destructor")
        {
            destructorModel.SetActive(true); 
            if (estadoActual == EstadoHistoria.VICTORIA) return;

            if (estadoActual == EstadoHistoria.BUSCANDO_DESTRUCTOR) {
                botonUsarPoder.SetActive(true);
                MostrarDialogo("Rigby", "¡Ahí está! ¡Saca El Teclado ya!");
            }
            else if (estadoActual == EstadoHistoria.BUSCANDO_PODER) {
                StartCoroutine(DialogoDoble("¡Es el Destructor! ¡Podemos vencerlo!", "¡No seas tonto viejo, necesitamos El Teclado!"));
            }
            else {
                MostrarDialogo("Mordecai", "¡Oh no, es el Destructor! ¡Tengo que encontrar a Rigby lo antes posible!");
            }
            return; 
        }

        switch (estadoActual)
        {
            case EstadoHistoria.BUSCANDO_RIGBY:
                if (queHay == "Rigby") {
                    rigbyModel.SetActive(true);
                    rigbyModel.transform.SetParent(socketRigby);
                    rigbyModel.transform.localPosition = Vector3.zero;
                    rigbyModel.transform.localRotation = Quaternion.identity; 
                    estadoActual = EstadoHistoria.BUSCANDO_PODER;
                    StartCoroutine(DialogoDoble("¡Rigby! si no arreglamos esto Benson nos despedira", "¡Viejo, casi me alcanza! ¡Busquemos El Teclado!"));
                } else if (queHay == "ElPoder") {
                    elPoderModel.SetActive(true);
                    elPoderModel.transform.localPosition = Vector3.zero;
                    elPoderModel.transform.localRotation = Quaternion.identity;
                    MostrarDialogo("Mordecai", "¡Es El Teclado! Pero no puedo usarlo sin Rigby.");
                } else {
                    MostrarDialogo("Mordecai", "¡Rayos! Aquí no hay nada.");
                }
                break;

            case EstadoHistoria.BUSCANDO_PODER:
                if (queHay == "ElPoder") {
                    elPoderModel.SetActive(true);
                    elPoderModel.transform.SetParent(socketPoder);
                    elPoderModel.transform.localPosition = Vector3.zero;
                    elPoderModel.transform.localRotation = Quaternion.identity;
                    estadoActual = EstadoHistoria.BUSCANDO_DESTRUCTOR;
                    StartCoroutine(DialogoDoble("¡Bien! ¡Ya tenemos El Poder!", "¡SÍIIIII! ¡Vayamos a derrotar al destructor!"));
                } else {
                    MostrarDialogo("Rigby", "¡Aquí no hay nada!");
                }
                break;
        }
    }

    void MudarModeloInicial(string item, ObserverBehaviour target)
    {
        GameObject go = (item == "Rigby") ? rigbyModel : (item == "ElPoder") ? elPoderModel : (item == "Destructor") ? destructorModel : null;
        if (go != null) {
            go.transform.SetParent(target.transform, false); 
            go.SetActive(false); 
        }
    }

    IEnumerator DialogoDoble(string txtMordecai, string txtRigby)
    {
        MostrarDialogo("Mordecai", txtMordecai);
        yield return new WaitForSeconds(txtMordecai.Length * velocidadEscritura + 0.5f); 
        MostrarDialogo("Rigby", txtRigby);
    }

    public void UsarElPoder()
    {
        botonUsarPoder.SetActive(false);
        destructorModel.SetActive(false);
        estadoActual = EstadoHistoria.VICTORIA;
        
        // MOSTRAR BOTÓN DE REINICIO AL GANAR
        botonRestart.SetActive(true); 

        MostrarDialogo("Mordecai", "¡OOOOOOOOOOOHHHHHHHH!");
        MostrarDialogo("Rigby", "¡OOOOOOOOOOOHHHHHHHH!");
    }
}