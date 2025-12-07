using UnityEngine;

public class ImpalaAgente : MonoBehaviour
{
    [Header("Configuración de Visión")]
    [Range(1, 30)] public float distanciaVision = 10f; 
    [Range(0, 10)] public float alturaOjos = 0.5f;     // Altura y pocision de las visión del impala
    [Range(10, 90)] public float anguloVision = 45f;   

    [Header("Referencias")]
    public LineRenderer lineaIzquierda;
    public LineRenderer lineaDerecha;
    public LineRenderer lineaCentro;

    public string accionActual = "Nada"; 

    // Se gaurda la rotación inicial (Norte)
    private Quaternion rotacionInicial;

    void Start()
    {
        // Asumimos que al empezar el juego mira al Norte (Frente)
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        DibujarLineasDeVision();
    }

    void DibujarLineasDeVision()
    {
        Vector3 origen = transform.position + (Vector3.up * alturaOjos);

        // Direcciones relativas al cuerpo
        Vector3 dirFrente = transform.forward * distanciaVision;
        Vector3 dirIzq = Quaternion.Euler(0, -anguloVision, 0) * transform.forward * distanciaVision;
        Vector3 dirDer = Quaternion.Euler(0, anguloVision, 0) * transform.forward * distanciaVision;

        if (lineaCentro != null) {
            lineaCentro.SetPosition(0, origen);
            lineaCentro.SetPosition(1, origen + dirFrente);
        }
        if (lineaIzquierda != null) {
            lineaIzquierda.SetPosition(0, origen);
            lineaIzquierda.SetPosition(1, origen + dirIzq);
        }
        if (lineaDerecha != null) {
            lineaDerecha.SetPosition(0, origen);
            lineaDerecha.SetPosition(1, origen + dirDer);
        }
    }

    public bool DetectarLeon(LeonAgente leon)
    {
        if (leon == null) return false;
        if (leon.estaEscondido) return false;

        Vector3 origen = transform.position + (Vector3.up * alturaOjos);
        Vector3 direccionHaciaLeon = leon.transform.position - transform.position; 
        float distanciaAlLeon = direccionHaciaLeon.magnitude;

        if (distanciaAlLeon > distanciaVision) return false; 

        // Angulo respecto a hacia donde estoy mirando AHORA
        float angulo = Vector3.Angle(transform.forward, direccionHaciaLeon);

        if (angulo < anguloVision)
        {
            return true; 
        }

        return false;
    }

    // --- Ancaldo de posición del impala --- (Daniel revisa esto, el imapala se mueve de lugar cuando se inicia el modo turbo)
    public void RealizarAccionAlAzar()
    {
        int azar = Random.Range(0, 3);
        
        if (azar == 0) 
        {
            accionActual = "MirandoFrente";
            // Volver a la rotación original (Norte)
            transform.rotation = rotacionInicial;
        }
        else if (azar == 1) 
        {
            accionActual = "MirandoIzquierda";
            // Girar -45 grados (Izquierda) respecto al Norte
            transform.rotation = rotacionInicial * Quaternion.Euler(0, -45, 0);
        }
        else 
        {
            accionActual = "MirandoDerecha";
            // Girar +45 grados (Derecha) respecto al Norte
            transform.rotation = rotacionInicial * Quaternion.Euler(0, 45, 0);
        }
    }
}