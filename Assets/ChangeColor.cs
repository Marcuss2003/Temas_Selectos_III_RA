using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [Header("Configuración de Accesorios")]
    // Arreglo de los objetos de ropa (lentes, gorra, sombrero).
    public GameObject[] accesorios; 
    // Variable para rastrear cuál es el accesorio que está encendido actualmente.
    private int indexAccesorioActual = 0;

    [Header("Partes del Cuerpo de Mordecai")]
    // Las 7 mallas que separaste en Blender (LOD0 al LOD0.006).
    public GameObject[] partesCuerpo; 
    // Lista de colores definida en el Inspector.
    public Color[] colores; 

    // --- FUNCIÓN PARA ACCESORIOS ALEATORIOS ---
    public void SiguienteAccesorio()
    {
        if (accesorios.Length < 2) return; // Necesitamos al menos 2 para que haya cambio.

        // 1. Apagamos el accesorio que Mordecai tiene puesto ahora.
        accesorios[indexAccesorioActual].SetActive(false);

        // 2. Generamos un número al azar entre 0 y el total de accesorios.
        int nuevoIndex;
        do {
            nuevoIndex = Random.Range(0, accesorios.Length);
        } while (nuevoIndex == indexAccesorioActual); // Evita que salga el mismo accesorio dos veces seguidas.

        // 3. Actualizamos el índice y encendemos el accesorio aleatorio.
        indexAccesorioActual = nuevoIndex;
        accesorios[indexAccesorioActual].SetActive(true);
    }

    // --- FUNCIÓN PARA COLORES ALEATORIOS ---
    public void SiguienteColor()
    {
        // Seguridad: Verifica que la lista de colores no esté vacía.
        if (colores.Length == 0 || partesCuerpo.Length == 0) return;

        // 1. Elegimos un color al azar de la lista que configuraste en el Inspector.
        int indexAleatorio = Random.Range(0, colores.Length);
        Color colorAzar = colores[indexAleatorio];

        // 2. Recorremos las 7 piezas de Mordecai para aplicarles el color.
        foreach (GameObject parte in partesCuerpo)
        {
            Renderer rend = parte.GetComponent<Renderer>();
            if (rend != null)
            {
                // 3. Aplicamos el color aleatorio al material de la pieza.
                rend.material.color = colorAzar;
            }
        }
    }
}