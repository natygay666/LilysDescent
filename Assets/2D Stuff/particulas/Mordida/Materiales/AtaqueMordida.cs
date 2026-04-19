using UnityEngine;

public class AtaqueMordida : MonoBehaviour
{
    // Asegúrate de que ambas sean PUBLIC
    public ParticleSystem chispasRojas;
    public ParticleSystem chispasNuevas;

    public void EjecutarChispas()
    {
        if (chispasRojas != null) chispasRojas.Play();
        if (chispasNuevas != null) chispasNuevas.Play();
    }
}
