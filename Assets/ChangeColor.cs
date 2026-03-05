using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public GameObject model;
    public Color color;
    public Material ColorMaterial;

    void Start()
    {
         
    }
    public void ChangeColor_BTN()
    {
        model.GetComponent<Renderer>().material.color = color; 
        ColorMaterial.color = color;
    }
}

