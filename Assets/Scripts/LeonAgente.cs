using UnityEngine;

public class LeonAgente : MonoBehaviour
{
    public bool estaEscondido = false;
    public Transform objetivo; 

    [Header("Referencias Visuales")]
    public Renderer pielDelTigre; 
    public Transform cuerpoDelTigre; 

    private Color colorNormal = Color.white; 
    private Color colorEscondido = Color.grey; // Color del Tigre Escondido

    void Update()
    {
        // 1. Corrección de posición (Mismo error que tenia el Impala Correccion a cargo de Daniel)
        if(cuerpoDelTigre != null) cuerpoDelTigre.localPosition = Vector3.zero;

        // 2. Mirar al objetivo
        if (objetivo != null) transform.LookAt(objetivo);
    }

    public void Esconderse()
    {
        estaEscondido = true;
        
        // CAMBIO DE COLOR (Oscurecer)
        if(pielDelTigre != null)
        {
            pielDelTigre.material.color = colorEscondido;
        }

        // CAMBIO DE TAMAÑO (Agacharse / Aplastarse un poco)
        if(cuerpoDelTigre != null)
        {
            //la escala normal es 0.5 
            // Lo bajamos en Y a 0.25 para que parezca agazapado en la hierba
            cuerpoDelTigre.localScale = new Vector3(0.5f, 0.25f, 0.5f);
        }
    }

    public void HacerseVisible()
    {
        estaEscondido = false;
        
        // COLOR NORMAL
        if(pielDelTigre != null)
        {
            pielDelTigre.material.color = colorNormal;
        }

        // TAMAÑO NORMAL
        if(cuerpoDelTigre != null)
        {
            cuerpoDelTigre.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}