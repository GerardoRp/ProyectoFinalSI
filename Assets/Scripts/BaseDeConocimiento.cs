using UnityEngine;
using System.Collections.Generic; 
using System.IO; 

[System.Serializable]
public class Memoria
{
    public string idSituacion; 
    public string accionLeon;  
    public int puntaje;        
}

public class BaseDeConocimiento : MonoBehaviour
{
    public List<Memoria> listaMemorias = new List<Memoria>();
    private string rutaArchivo;

    void Start()
    {
        rutaArchivo = Application.persistentDataPath + "/cerebro_leon.json";
        CargarMemoria();
    }

    // --- GUARDAR EXPERIENCIA ---
    public void AgregarExperiencia(int posLeon, string accionImpala, string accionLeon, int puntosGanados)
    {
        string id = "Pos" + (posLeon + 1) + "_" + accionImpala;
        
        Memoria memoriaExistente = null;
        foreach (Memoria m in listaMemorias)
        {
            if (m.idSituacion == id && m.accionLeon == accionLeon)
            {
                memoriaExistente = m;
                break;
            }
        }

        if (memoriaExistente != null)
        {
            // Suma de los puntos al acumulado
            memoriaExistente.puntaje += puntosGanados;
        }
        else
        {
            Memoria nueva = new Memoria();
            nueva.idSituacion = id;
            nueva.accionLeon = accionLeon;
            nueva.puntaje = puntosGanados;
            listaMemorias.Add(nueva);
        }
        GuardarMemoria();
    }

    // --- CONSULTA INTELIGENTE ---
    public string ConsultarMejorAccion(int posLeon, string accionImpala)
    {
        string id = "Pos" + (posLeon + 1) + "_" + accionImpala;
        
        // Valores iniciales: 0 significa "No sé qué pasa"
        // Es mejor que -100 (Muerte), pero peor que +100 
        int scoreAvanzar = 0;
        int scoreEsconderse = 0;
        
        bool conozcoAvanzar = false;
        bool conozcoEsconderse = false;

        // 1. Buscamos en la memoria qué sabemos de cada acción
        foreach (Memoria m in listaMemorias)
        {
            if (m.idSituacion == id)
            {
                if (m.accionLeon == "Avanzar") 
                {
                    scoreAvanzar = m.puntaje;
                    conozcoAvanzar = true;
                }
                if (m.accionLeon == "Esconderse") 
                {
                    scoreEsconderse = m.puntaje;
                    conozcoEsconderse = true;
                }
            }
        }

        // 2. Si no conocemos NADA de esta situación devolvemos vacío para que explore al azar
        if (!conozcoAvanzar && !conozcoEsconderse) return "";

        // 3. COMPARACIÓN INTELIGENTE
        // Si una acción es muy mala (ej. -100) y la otra no la conocemos (0),
        // matemáticamente 0 > -100 así que elegirá la desconocida (Probar suerte).
        
        if (scoreAvanzar >= scoreEsconderse)
        {
            return "Avanzar";
        }
        else
        {
            return "Esconderse";
        }
    }

    public void GuardarMemoria()
    {
        // Guardamos solo si hay cambios importantes para no saturar el disco
        string json = JsonUtility.ToJson(this, true); 
        File.WriteAllText(rutaArchivo, json);
    }

    public void CargarMemoria()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }

    // Cuenta cuántas memorias tienen puntaje positivo (Jugadas maestras)
    public int ContarReglasGanadoras()
    {
        int cont = 0;
        foreach (Memoria m in listaMemorias)
        {
            if (m.puntaje > 100) cont++; // Consideramos "maestras" las que tienen muchos puntos
        }
        return cont;
    }
}