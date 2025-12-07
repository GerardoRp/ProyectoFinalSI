using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

[System.Serializable]
public class PasoHistoria
{
    public int indicePosicion;
    public string accionImpala;
    public string accionLeon;
}

public class ControladorJuego : MonoBehaviour
{
    [Header("Actores")]
    public LeonAgente leonScript; 
    public ImpalaAgente impalaScript; 

    [Header("Inteligencia")]
    public BaseDeConocimiento cerebro; 

    [Header("Interfaz Gráfica (UI)")]
    public TextMeshProUGUI textoMarcador; 
    public TextMeshProUGUI textoEstado;   

    [Header("Configuración del Mapa")]
    public Transform[] posiciones;   
    
    [Header("Configuración de Tiempos")]
    public float tiempoNormal = 1.0f;        
    public float tiempoEntrenamiento = 0.01f;
    
    private float tiempoActual;
    private bool esModoEntrenamiento = false; 
    private int numeroDeTurno = 0;
    
    private int caceriasGanadas = 0;
    private int caceriasPerdidas = 0;

    private List<PasoHistoria> historialIncursion = new List<PasoHistoria>();

    void Start()
    {
        tiempoActual = tiempoNormal; 
        ActualizarUI("Esperando inicio...");
        ResetearIncursion();
        StartCoroutine(CicloDeJuego());
    }

    public void AlternarModoEntrenamiento()
    {
        esModoEntrenamiento = !esModoEntrenamiento;
        if (esModoEntrenamiento) {
            tiempoActual = tiempoEntrenamiento;
            ActualizarUI("MODO TURBO ACTIVADO");
        } else {
            tiempoActual = tiempoNormal;
            ActualizarUI("Velocidad Normal");
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---
    
    public void BotonReiniciar()
    {
        // Recarga la escena actual
        // Nota: El cerebro NO se borra porque se guarda en archivo .json
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Cierra el .exe
    }

    // -----------------------------------------

    void ResetearIncursion()
    {
        historialIncursion.Clear();

        if (posiciones.Length > 0 && leonScript != null)
        {
            int inicioRandom = Random.Range(0, posiciones.Length);
            if (Vector3.Distance(posiciones[inicioRandom].position, impalaScript.transform.position) < 6.0f)
            {
                inicioRandom = 0; 
            }
            leonScript.transform.position = posiciones[inicioRandom].position;
            leonScript.HacerseVisible(); 
        }
    }

    void ProcesarFinDeIncursion(int premioFinal)
    {
        float puntosAcumulados = premioFinal;
        float factorDeDescuento = 0.9f; 

        for (int i = historialIncursion.Count - 1; i >= 0; i--)
        {
            PasoHistoria paso = historialIncursion[i];
            cerebro.AgregarExperiencia(paso.indicePosicion, paso.accionImpala, paso.accionLeon, (int)puntosAcumulados);
            puntosAcumulados = puntosAcumulados * factorDeDescuento;
        }
    }

    IEnumerator CicloDeJuego()
    {
        Debug.Log("--- INICIO DE LA CAZERÍA ---");

        while (true) 
        {
            yield return new WaitForSeconds(tiempoActual);
            numeroDeTurno++;

            int indicePosAnterior = ObtenerIndicePosicionActual();
            
            // 1. IMPALA
            string accionImpala = "Nada";
            if (impalaScript != null) 
            {
                impalaScript.RealizarAccionAlAzar();
                accionImpala = impalaScript.accionActual;
            }

            // 2. LEÓN
            string accionLeon = "Esperar"; 
            if (leonScript != null && cerebro != null)
            {
                int dadoCuriosidad = Random.Range(0, 100);
                bool activarCuriosidad = dadoCuriosidad < 10; 

                string accionInteligente = cerebro.ConsultarMejorAccion(indicePosAnterior, accionImpala);

                if (accionInteligente != "" && !activarCuriosidad)
                {
                    if(!esModoEntrenamiento) ActualizarUI("Usando Inteligencia: " + accionInteligente);
                    EjecutarAccionEspecifica(accionInteligente);
                    accionLeon = accionInteligente;
                }
                else
                {
                    if(!esModoEntrenamiento) 
                    {
                        if(activarCuriosidad) ActualizarUI("¡CURIOSIDAD ACTIVADA!");
                        else ActualizarUI("Explorando...");
                    }
                    accionLeon = AccionLeonAleatoria();
                }
            }

            PasoHistoria nuevoPaso = new PasoHistoria();
            nuevoPaso.indicePosicion = indicePosAnterior;
            nuevoPaso.accionImpala = accionImpala;
            nuevoPaso.accionLeon = accionLeon;
            historialIncursion.Add(nuevoPaso);

            // 3. RESULTADOS
            if (impalaScript != null && leonScript != null)
            {
                bool impalaAsustado = impalaScript.DetectarLeon(leonScript);

                if (impalaAsustado)
                {
                    caceriasPerdidas++;
                    if(!esModoEntrenamiento) ActualizarUI("<color=red>¡HUIDA!</color>");
                    ProcesarFinDeIncursion(-100); 
                    ResetearIncursion();
                }
                else
                {
                    float distanciaNueva = Vector3.Distance(leonScript.transform.position, impalaScript.transform.position);

                    if (distanciaNueva < 6.0f && accionLeon != "Esconderse")
                    {
                        caceriasGanadas++;
                        
                        if(!esModoEntrenamiento)
                        {
                            ActualizarUI("<color=green>¡ATAQUE FINAL!</color>");
                            leonScript.HacerseVisible();
                            Vector3 posInicial = leonScript.transform.position;
                            Vector3 posFinal = impalaScript.transform.position;
                            float t = 0;
                            while(t < 1)
                            {
                                t += Time.deltaTime / 0.5f; 
                                leonScript.transform.position = Vector3.Lerp(posInicial, posFinal, t);
                                yield return null;
                            }
                            ActualizarUI("<color=green>¡PRESA CAPTURADA!</color>");
                            yield return new WaitForSeconds(1.0f); 
                        }

                        ProcesarFinDeIncursion(1000); 
                        ResetearIncursion();
                    }
                }
                ActualizarMarcador();
            }
        }
    }

    void ActualizarUI(string mensaje) { if (textoEstado != null) textoEstado.text = mensaje; }

    void ActualizarMarcador() {
        if (textoMarcador != null) {
            float total = caceriasGanadas + caceriasPerdidas;
            float porcentaje = 0f;
            if (total > 0) porcentaje = (caceriasGanadas / total) * 100f;
            int reglasSabias = 0;
            if(cerebro != null) reglasSabias = cerebro.ContarReglasGanadoras();

            textoMarcador.text = "Incursiones: " + total + "\n" +
                                 "Victorias: " + caceriasGanadas + " | Derrotas: " + caceriasPerdidas + "\n" +
                                 "<size=30>Conocimiento: " + reglasSabias + " Rutas Maestras</size>\n" +
                                 "<size=40><color=yellow>Win Rate: " + porcentaje.ToString("F1") + "%</color></size>";
        }
    }

    void EjecutarAccionEspecifica(string accion) {
        if (accion == "Avanzar") {
            leonScript.HacerseVisible(); 
            int indiceAleatorio = Random.Range(0, posiciones.Length);
            leonScript.transform.position = posiciones[indiceAleatorio].position;
        } else if (accion == "Esconderse") {
            leonScript.Esconderse();
        }
    }

    string AccionLeonAleatoria() {
        int probabilidad = Random.Range(0, 100);
        if (probabilidad < 50) { 
            EjecutarAccionEspecifica("Avanzar");
            return "Avanzar";
        } else {
            EjecutarAccionEspecifica("Esconderse");
            return "Esconderse";
        }
    }

    int ObtenerIndicePosicionActual() {
        float distanciaMinima = Mathf.Infinity;
        int indiceMasCercano = 0;
        for (int i = 0; i < posiciones.Length; i++) {
            float dist = Vector3.Distance(leonScript.transform.position, posiciones[i].position);
            if (dist < distanciaMinima) {
                distanciaMinima = dist;
                indiceMasCercano = i;
            }
        }
        return indiceMasCercano;
    }
}