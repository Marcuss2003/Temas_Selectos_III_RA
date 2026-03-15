using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [Header("Configuración de Accesorios")]
    public GameObject[] accesorios; 
    private int indexAccesorioActual = 0;

    [Header("Partes del Cuerpo de Mordecai")]
    public GameObject[] partesCuerpo; 
    public Color[] colores; 

    public void SiguienteAccesorio()
    {
        if (accesorios.Length < 2) return; 

        accesorios[indexAccesorioActual].SetActive(false);
        int nuevoIndex;
        do {
            nuevoIndex = Random.Range(0, accesorios.Length);
        } while (nuevoIndex == indexAccesorioActual); 

        indexAccesorioActual = nuevoIndex;
        accesorios[indexAccesorioActual].SetActive(true);
    }

    public void SiguienteColor()
    {
        if (colores.Length == 0 || partesCuerpo.Length == 0) return;

        int indexAleatorio = Random.Range(0, colores.Length);
        Color colorAzar = colores[indexAleatorio];

        foreach (GameObject parte in partesCuerpo)
        {
            Renderer rend = parte.GetComponent<Renderer>();
            if (rend != null)
            {
                
                rend.material.color = colorAzar;
            }
        }
    }
}