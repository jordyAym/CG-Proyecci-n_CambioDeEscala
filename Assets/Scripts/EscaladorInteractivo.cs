using UnityEngine;
using UnityEngine.Rendering;

public class EscaladorInteractivoGrupo : MonoBehaviour
{
    private bool siendoManipulado = false;
    private Vector3 escalaFinal;
    private Renderer[] renderers;
    private Collider[] colliders;

    public float escalaMinima = 0.2f;
    public float escalaMaxima = 5f;

    void Start()
    {
        escalaFinal = transform.localScale;
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void OnMouseDown()
    {
        siendoManipulado = true;

        foreach (var r in renderers)
            r.shadowCastingMode = ShadowCastingMode.Off;

        foreach (var c in colliders)
            c.enabled = false;

        Debug.Log("Interacción grupal iniciada.");
    }

    void OnMouseUp()
    {
        siendoManipulado = false;

        transform.localScale = escalaFinal;

        // Ajustar posición Y para mantener base del objeto en el suelo
        float alturaMax = ObtenerAlturaMaximaLocal();
        float nuevaPosY = alturaMax / 2f;
        transform.position = new Vector3(transform.position.x, nuevaPosY, transform.position.z);

        foreach (var r in renderers)
            r.shadowCastingMode = ShadowCastingMode.On;

        foreach (var c in colliders)
            c.enabled = true;

        Debug.Log("Interacción grupal terminada. Nueva escala: " + escalaFinal);
    }

    void Update()
    {
        if (siendoManipulado)
        {
            float factor = 1f;

            if (Input.GetKeyDown(KeyCode.E))
            {
                factor = 1.15f;
                Debug.Log("Tecla E presionada: aumento.");
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                factor = 0.85f;
                Debug.Log("Tecla Q presionada: reducción.");
            }

            if (factor != 1f)
            {
                Vector3 nuevaEscala = escalaFinal * factor;

                if (nuevaEscala.y < escalaMinima || nuevaEscala.y > escalaMaxima)
                {
                    Debug.LogWarning("Escala fuera de rango. Cancelado.");
                    return;
                }

                escalaFinal = nuevaEscala;
                Debug.Log("Escala provisional del grupo: " + escalaFinal);
            }
        }
    }

    float ObtenerAlturaMaximaLocal()
    {
        float alturaMax = 1f;

        foreach (var r in renderers)
        {
            float altura = r.bounds.size.y / transform.localScale.y;
            if (altura > alturaMax)
                alturaMax = altura;
        }

        return alturaMax * escalaFinal.y;
    }
}
