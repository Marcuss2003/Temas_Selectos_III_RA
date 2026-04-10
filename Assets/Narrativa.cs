using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NarrativaPersistente : MonoBehaviour
{
    public enum Estado { Inicio, BuscandoRigby, BuscandoPoder, Final, Victoria }
    public Estado estadoActual = Estado.Inicio;

    public GameObject[] targets; 
    public GameObject mordecai, rigby, elPoder, destructor, botonAccesorios;
    public TextMeshProUGUI textoDialogo;

    private string targetDondeViveMordecai = "";
    private Dictionary<string, string> mapaRoles = new Dictionary<string, string>();

    void Start() {
        ConfigurarAleatoriedad();
        // Mordecai empieza en el Target 0 (el que pusiste como World Center)
        targetDondeViveMordecai = targets[0].name;
        ActualizarPosicionMordecai(targets[0]);
        mordecai.SetActive(false); 
    }

    public void OnTargetFound(string nombreTarget) {
        if (!mapaRoles.ContainsKey(nombreTarget)) return;
        string rol = mapaRoles[nombreTarget];

        // 1. SIEMPRE lo encendemos si es su "casa" actual
        if (nombreTarget == targetDondeViveMordecai) {
            mordecai.SetActive(true);
        }

        // 2. REGLA DE TRASLADO: Solo se muda si la historia avanza
        EvaluarProgreso(nombreTarget, rol);
    }

    void EvaluarProgreso(string nombreTarget, string rol) {
        bool cambioDeCasa = false;

        if (estadoActual == Estado.Inicio && rol == "Inicio") {
            textoDialogo.text = "¡Viejo! Busca a Rigby.";
            estadoActual = Estado.BuscandoRigby;
        }
        else if (estadoActual == Estado.BuscandoRigby && rol == "Rigby") {
            rigby.SetActive(true);
            textoDialogo.text = "Rigby: ¡Ooooh! ¡Busca El Poder!";
            cambioDeCasa = true;
            estadoActual = Estado.BuscandoPoder;
        }
        else if (estadoActual == Estado.BuscandoPoder && rol == "Poder") {
            elPoder.SetActive(true);
            botonAccesorios.SetActive(true);
            textoDialogo.text = "¡SÍ! ¡Busca al Destructor!";
            cambioDeCasa = true;
            estadoActual = Estado.Final;
        }
        else if (estadoActual == Estado.Final && rol == "Destructor") {
            destructor.SetActive(true);
            textoDialogo.text = "¡TOMA ESTO! ¡LO VENCIMOS!";
            cambioDeCasa = true;
            estadoActual = Estado.Victoria;
        }

        if (cambioDeCasa) {
            targetDondeViveMordecai = nombreTarget;
            ActualizarPosicionMordecai(GameObject.Find(nombreTarget));
        }
    }

    void ActualizarPosicionMordecai(GameObject nuevoPadre) {
        // FORZAMOS EL TRASLADO FÍSICO
        mordecai.transform.SetParent(nuevoPadre.transform);
        mordecai.transform.localPosition = Vector3.zero; // Al centro del papel
        mordecai.transform.localRotation = Quaternion.identity; // Mirando al frente
        mordecai.SetActive(true);
    }

    public void OnTargetLost(string nombreTarget) {
        // Solo apagamos si NO es donde vive Mordecai
        if (nombreTarget != targetDondeViveMordecai) {
            DesactivarPorNombre(nombreTarget);
        }
    }

    void DesactivarPorNombre(string nombre) {
        string rol = mapaRoles[nombre];
        if (rol == "Rigby") rigby.SetActive(false);
        if (rol == "Poder") elPoder.SetActive(false);
        if (rol == "Destructor") destructor.SetActive(false);
    }

    void ConfigurarAleatoriedad() {
        List<string> roles = new List<string> { "Rigby", "Poder", "Destructor", "Nada" };
        mapaRoles.Add(targets[0].name, "Inicio");
        for (int i = 1; i < targets.Length; i++) {
            int rnd = Random.Range(0, roles.Count);
            mapaRoles.Add(targets[i].name, roles[rnd]);
            roles.RemoveAt(rnd);
        }
    }
}